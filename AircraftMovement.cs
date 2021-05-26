using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AircraftMovement : MonoBehaviour
{
    public Slider uiSlider;
    private GameObject aircraftObject;
    public static Aircraft aircraft;
 

    public class Aircraft
    {
        internal GameObject aircraft;  // Reference to the GameObject manipulated by the class - internal means can be seen in derived classes
        internal float rotationLogFigure;
        internal Slider throttleSlider;

        // Access to the control surfaces
        GameObject rudder;
        GameObject rightAileron;
        GameObject rightElevator;
        GameObject cloudPivot;

        float startPosX;
        float startPosY;
        float startPosZ;
        float startPosW;

        public Aircraft(GameObject aircraft, Slider throttleSlider)
        {
            this.aircraft = aircraft;  // pass in the Surface gameobject
            this.throttleSlider = throttleSlider;
            this.rotationLogFigure = throttleSlider.value;

            // Assign the control surfaces
            this.rudder = GameObject.Find("Rudder");
            this.rightAileron = GameObject.Find("R_Aileron");
            this.rightElevator = GameObject.Find("RightElevator");
            this.cloudPivot = GameObject.Find("CloudPivot");

            this.startPosX = aircraft.transform.rotation.x;
            this.startPosY = aircraft.transform.rotation.y;
            this.startPosZ = aircraft.transform.rotation.z;
            this.startPosW = aircraft.transform.rotation.w;

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
                this.aircraft.transform.localRotation *= Quaternion.AngleAxis((axis / 3) * rotationLogFigure * Time.deltaTime, directionOne);
            }
            else if (axis < 0)
            {
                this.aircraft.transform.localRotation *= Quaternion.AngleAxis((-axis / 3) * rotationLogFigure * Time.deltaTime, directionTwo);
            }

        }

        public void RotateAxis()
        {
            rotationLogFigure = RotationSpeedLogCalc();
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

        // Rotate clouds based on aircrafts Z rotation movement, so they are always moving towards aircraft, making it appear to always be travelling forward
        public void RotateClouds()
        {
            float rudderInspectorFloat = WrapAngle(this.rudder.transform.localEulerAngles.z); // current aircraft Z rotation
            if (rudderInspectorFloat >= 1)
            {
                this.cloudPivot.transform.localRotation *= Quaternion.AngleAxis((rudderInspectorFloat / 3) * rotationLogFigure * Time.deltaTime, Vector3.back);
            }
            else if (rudderInspectorFloat < 0)
            {
                this.cloudPivot.transform.localRotation *= Quaternion.AngleAxis((-rudderInspectorFloat / 3) * rotationLogFigure * Time.deltaTime, Vector3.forward);
            }
        }

        // Logorithmic maths to get aircraft to rotate in a logorithmic speed curve, based on throttle slider figure (1 to 3).
        private float RotationSpeedLogCalc()
        {
            //3 - 17seconds = 3.12ish
            //2.5 - 25seconds = 2.1
            //2 - 40seconds = 1.32
            //1 - 50seconds = 1.08ish

            //500 = 3
            //400 = 2.5
            //300 = 2
            //200 = 1.5
            //100 = 1

            float rotationValue;
            if (throttleSlider.value <= 1.25)
            {
                rotationValue = 1.08f;
            }
            else if (throttleSlider.value <= 1.5)
            {
                rotationValue = 1.18f;
            }
            else if (throttleSlider.value <= 1.75)
            {
                rotationValue = 1.25f;
            }
            else if (throttleSlider.value <= 2)
            {
                rotationValue = 1.32f;
            }
            else if (throttleSlider.value <= 2.25)
            {
                rotationValue = 1.85f;
            }
            else if (throttleSlider.value <= 2.5)
            {
                rotationValue = 2.1f;
            }
            else if (throttleSlider.value <= 2.75)
            {
                rotationValue = 2.65f;
            }
            else
            {
                rotationValue = 3.12f;
            }
            return rotationValue;

            //Debug.Log((float)System.Math.Sqrt(throttleSlider.value) * rotationMultiplyFactor);
            //return (float)System.Math.Sqrt(throttleSlider.value) * rotationMultiplyFactor;
            //return (float)rotationValue;  // Return value of the equation
        }


        public void ResetAircraft()
        {
            this.aircraft.transform.rotation = new Quaternion(startPosX, startPosY, startPosZ, startPosW);
        }


    }
    void Start()
    {
        // Create the aircraft object
        aircraftObject = GameObject.Find("AircraftPivot");
        aircraft = new Aircraft(aircraftObject, uiSlider);
    }

    // Update is called once per frame
    void Update()
    {
       aircraft.RotateAxis();  // Rotate the aircraft depending on the control surface settings
       aircraft.RotateClouds(); // Rotate cloud pivot based on aircrafts Z axis (yaw)
    }
}
