/* Script controls the Aircraft 3D model movement Movement is based on the
position of each control surface (axis) every frame, moving the aircraft based
on control surface rotation position */

using UnityEngine;
using UnityEngine.UI;  // Gives access to the Slider datatype

public class Aircraft : MonoBehaviour
{
    private GameObject aircraftObject;  // Reference to the aircraft 3D Model object
    public static AircraftObject aircraft;
    /* Reference to the aircraft Object created in the Start Method. Public
    object allows aircraft to be used by other scripts THe static modifier is
    used to prevent the need to instantiate the aircraft object in each script
    requiring it */

    void Start()
    {
        // Get the reference to the aircraft 3D model
        aircraftObject = GameObject.Find("AircraftPivot");
        // Create the Aircraft Object - Passing in the 3D model as a reference
        aircraft = new AircraftObject(aircraftObject);
    }

    /* Update is called once per frame Each method is called from the
    ControlsUtilityMethods.cs script as that script contains all of the cockpit
    control postions Update method controls how the surfaces are moved based on
    there current rotation values, changed by user input on the cockpit controls
    Cloud speed is also controlled per frame here */
    void Update()
    {
        // Rotate surfaces based on UI Joystick location
        ControlsUtilityMethods.RotateSurfaces();
        // Rotate flaps based on UI Flap Slider position
        ControlsUtilityMethods.MoveFlaps();
        // Update the cloud speed based on throttle position 
        ControlsUtilityMethods.UpdateCloudSpeed();
        // Rotate the aircraft depending on the control surface defelcted
        // positions
        aircraft.RotateAxis();
        // Rotate cloud pivot based on aircrafts Z axis (yaw)
        aircraft.RotateClouds();
    }

    /* The class is used to keep all of the variables and methods relating to
    the aircraft encapsulated */
    public class AircraftObject
    {
        private GameObject aircraft;
        // Reference to the 3D aircraft model manipulated by the class
        private float rotationSpeed;
        // Float value stores the current rotation speed for the aircraft  
        private Slider throttleSlider;
        // Reference to the UI Throttle Slider controlling rotation speed  

        // Reference to each control surface required by the class to calculate
        // rotation
        private ControlSurfaces.Rudder rudder;
        private ControlSurfaces.Surface rightAileron;
        private ControlSurfaces.Surface rightElevator;
        private GameObject cloudPivot;

        // Variables store the aircrafts starting rotation positions for each
        // axis
        private float startPosX;
        private float startPosY;
        private float startPosZ;
        private float startPosW;

        public AircraftObject(GameObject aircraft)  // Aircraft class constructor
        {
            this.aircraft = aircraft;  // Reference to the aircraft 3D model
            this.throttleSlider =
                GameObject.FindGameObjectWithTag("ThrottleSlider").GetComponent<Slider>();
            // Reference to the UI Throttle Slider  
            this.rotationSpeed = throttleSlider.value;
            // Reference to the current value of the UI Throttle Slider  

            // Assign the control surfaces
            this.rudder = ControlSurfaces.rudder;
            this.rightAileron = ControlSurfaces.rightAileron;
            this.rightElevator = ControlSurfaces.rightElevator;
            this.cloudPivot = GameObject.Find("CloudPivot");

            // Assign the aircraft rotation start positions
            this.startPosX = aircraft.transform.rotation.x;
            this.startPosY = aircraft.transform.rotation.y;
            this.startPosZ = aircraft.transform.rotation.z;
            this.startPosW = aircraft.transform.rotation.w;
        }

        // Aircraft Methods

        /* Method updates the rotation speed and aquires current control surface
        values for each axis (x,y,z) */
        public void RotateAxis()
        {
            // Calculate rotation speed based on UI Slider value
            rotationSpeed = RotationSpeedCalc();
            // Get all of the control surfaces relevant angles normalised to
            // values between 180/-180
            float rudderInspectorFloat =
                ControlsUtilityMethods.WrapAngle(this.rudder.GetCurrentRotations().z);
            float aileronInspectorFloat =
                ControlsUtilityMethods.WrapAngle(this.rightAileron.GetCurrentRotations().y);
            float elevatorInspectorFloat =
                ControlsUtilityMethods.WrapAngle(this.rightElevator.GetCurrentRotations().y);
            // Map variables to the corresponding control surfaces
            float roll = aileronInspectorFloat;  // ailerons (x axis)
            float pitch = elevatorInspectorFloat;  // elevators (y axis)
            float yaw = rudderInspectorFloat;  // rudder (z axis)
            // AxisRotate method rotates the aircraft on each axis individually
            // in the correct order
            AxisRotate(pitch, Vector3.up, Vector3.down);
            // y axis movement rotating either up or down
            AxisRotate(roll, Vector3.right, Vector3.left);
            // x axis movement rotating either right or left
            AxisRotate(yaw, Vector3.back, Vector3.forward);
            // z axis movment, rotating either back or forward
        }

        /* Axis Rotate method rotates the aircraft on each axis (x,y,z) locally
        based on the current angle of the input surface */
        private void AxisRotate(float surface, Vector3 directionOne, Vector3 directionTwo)
        {
            if (surface >= 1)
            // If surface is in a positive position, rotate positive angle at
            // speed set by UI Slider
            {
                this.aircraft.transform.localRotation *=
                    Quaternion.AngleAxis(surface * rotationSpeed
                        * Time.deltaTime, directionOne);
            }
            else if (surface < 0)  // If surface is in a negative position, rotate negative angle
            {
                this.aircraft.transform.localRotation *=
                    Quaternion.AngleAxis(-surface * rotationSpeed
                        * Time.deltaTime, directionTwo);
            }
        }

        // Rotate clouds based on aircrafts Z rotation movement, so they are
        // always moving towards aircraft
        public void RotateClouds()
        {
            float rudderInspectorFloat =
                ControlsUtilityMethods.WrapAngle(this.rudder.GetCurrentRotations().z);
            // current aircraft Z rotation
            if (rudderInspectorFloat >= 1)
            // Rotate the clouds in conjunction with the rudder, so they follow
            // the aircrafts rotation
            {
                this.cloudPivot.transform.localRotation *=
                    Quaternion.AngleAxis((rudderInspectorFloat)
                        * rotationSpeed * Time.deltaTime, Vector3.back);
            }
            else if (rudderInspectorFloat < 0)
            {
                this.cloudPivot.transform.localRotation *=
                    Quaternion.AngleAxis((-rudderInspectorFloat)
                        * rotationSpeed * Time.deltaTime, Vector3.forward);
            }
        }

        /* Method calculates the speed the aircraft should rotate based on the
         throttle slider (value between 1 and 3) */
        private float RotationSpeedCalc()
        {
            /* Speed aims given by customer 3 - 17seconds = 3.12ish 2.5 -
            25seconds = 2.1 2 - 40seconds = 1.32 1 - 50seconds = 1.08ish

            How speed aims corespond to the airspeed indicator in the UI 500 = 3
            400 = 2.5 300 = 2 200 = 1.5 100 = 1

            Rotation values worked out to match the speed the customer wanted
            the aircraft to rotate */
            float rotationValue;
            if (throttleSlider.value <= 1.25)
            {
                rotationValue = 0.36f;
            }
            else if (throttleSlider.value <= 1.5)
            {
                rotationValue = 0.394f;
            }
            else if (throttleSlider.value <= 1.75)
            {
                rotationValue = 0.4167f;
            }
            else if (throttleSlider.value <= 2)
            {
                rotationValue = 0.44f;
            }
            else if (throttleSlider.value <= 2.25)
            {
                rotationValue = 0.6167f;
            }
            else if (throttleSlider.value <= 2.5)
            {
                rotationValue = 0.7f;
            }
            else if (throttleSlider.value <= 2.75)
            {
                rotationValue = 0.8834f;
            }
            else
            {
                rotationValue = 1.04f;
            }
            return rotationValue;
        }

        /* Method uses the initial aircraft position to return the aircraft 3D
        model back to its starting position */
        public void ResetAircraft()
        {
            this.aircraft.transform.rotation =
                new Quaternion(startPosX, startPosY, startPosZ, startPosW);
        }
    }
}
