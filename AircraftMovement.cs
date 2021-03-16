using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AircraftMovement : MonoBehaviour
{
    private GameObject aircraftObject;
    static Aircraft aircraft;
    class Aircraft
    {
        internal GameObject aircraft;  // Reference to the GameObject manipulated by the class - internal means can be seen in derived classes
        internal Slider uiSlider;
        internal float sliderValue;

        // Access to the control surfaces
        GameObject rudder;
        GameObject rightAileron;
        GameObject rightElevator;

        public Aircraft(GameObject aircraft)
        {
            this.aircraft = aircraft;  // pass in the Surface gameobject
            this.uiSlider = GameObject.FindObjectOfType<Slider>();
            this.sliderValue = uiSlider.value;

            // Assign the control surfaces
            this.rudder = GameObject.Find("Rudder");
            this.rightAileron = GameObject.Find("R_Aileron");
            this.rightElevator = GameObject.Find("RightElevator");
        }

        private float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        private void AxisRotate(float axis, Vector3 directionOne, Vector3 directionTwo)
        {
            if (axis >= 1)
            {
                this.aircraft.transform.localRotation *= Quaternion.AngleAxis((axis / 3) * sliderValue * Time.deltaTime, directionOne);
            }
            else if (axis < 0)
            {
                this.aircraft.transform.localRotation *= Quaternion.AngleAxis((-axis / 3) * sliderValue * Time.deltaTime, directionTwo);
            }

        }

        public void RotateAxis()
        {
            sliderValue = uiSlider.value;
            // Get all of the control surfaces relevant angles
            float rudderInspectorFloat = WrapAngle(this.rudder.transform.localEulerAngles.z);
            float aileronInspectorFloat = WrapAngle(this.rightAileron.transform.localEulerAngles.y);
            float elevatorInspectorFloat = WrapAngle(this.rightElevator.transform.localEulerAngles.y);
            // Get all of the surface relevant locations
            float roll = aileronInspectorFloat;  // ailerons (x axis)
            float pitch = elevatorInspectorFloat;  // elevators (y axis)
            float yaw = rudderInspectorFloat;  // rudder (z axis)

            AxisRotate(pitch, Vector3.up, Vector3.down);
            AxisRotate(roll, Vector3.right, Vector3.left);
            AxisRotate(yaw, Vector3.back, Vector3.forward);
        }
    }
    void Start()
    {
        // Create the aircraft object
        aircraftObject = GameObject.Find("AircraftPivot");
        aircraft = new Aircraft(aircraftObject);
    }

    // Update is called once per frame
    void Update()
    {
       aircraft.RotateAxis();  // Rotate the aircraft depending on the control surface settings
    }
}
