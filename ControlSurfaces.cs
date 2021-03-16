using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSurfaces : MonoBehaviour
{
    // Control Surface References:
    private GameObject rudderObject;
    private GameObject leftElevatorObject;
    private GameObject rightElevatorObject;
    private GameObject leftAileronObject;
    private GameObject rightAileronObject;

    public static Rudder rudder;  // Only one instance of class shared across all scripts - once created can be called from other scripts
    public static Elevator leftElevator;
    public static Elevator rightElevator;
    public static Aileron leftAileron;
    public static Aileron rightAileron;

    public abstract class Surface
    {
        internal GameObject surface;  // Reference to the GameObject manipulated by the class - internal means can be seen in derived classes
        internal float x, y, z;  // angles of each axis
        internal float degreesPerJoystickMove = 0.3125f;
        internal float joystickRadius = 64f;
        public Surface(GameObject controlSurface)  // Constructor
        {
            this.surface = controlSurface;  // pass in the Surface gameobject
            // Get the control surfaces current starting axis angles
            this.x = this.surface.transform.localEulerAngles.x;
            this.y = this.surface.transform.localEulerAngles.y;
            this.z = this.surface.transform.localEulerAngles.z;
            Setup();  // Set the axis the object is rotated by to 0 at the start
        }

        public virtual void Setup()  // virtual method can be overwritten in derived classes, as different axis are used for each control surface
        {
            this.surface.transform.localEulerAngles = new Vector3(x, 0, z);  // start y rotation value at 0
        }

        public void Rotate(Vector3 angle)  // rotate the surface by angle argument
        {
            this.surface.transform.localEulerAngles = new Vector3(angle.x, angle.y, angle.z);
        }

    }

    public class Rudder : Surface
    {
        public Rudder(GameObject controlSurface) : base(controlSurface)
        {
            Setup();
        }
        public override void Setup() 
        {
            this.surface.transform.localEulerAngles = new Vector3(x, y, 0);  // start z rotation value at 0 (ailerons and elevators)
        }

        public void MoveRudder(string direction)  // Overloaded method deals with keyboard / mouse input
        {
            float degrees;
            float KEYBOARD_DEGREES = 20f;  // amount of degrees to move when using the keyboard

            if (direction == "left")
            {
                degrees = KEYBOARD_DEGREES;
                this.surface.transform.localEulerAngles = new Vector3(this.x, this.y, degrees);

            }
            else if (direction == "right")
            {
                degrees = -KEYBOARD_DEGREES;
                this.surface.transform.localEulerAngles = new Vector3(this.x, this.y, degrees);
            }
            else if (direction == "start")
            {
                this.surface.transform.localEulerAngles = new Vector3(this.x, this.y, 0f);  // return to start position
            }
            //float degrees = currentJoystickCoords.y * degreesPerJoystickMove;
            //rotate around parents pivot point on the y axis to the required degrees out of 20
            Debug.Log(this.surface.transform.localEulerAngles);
        }

    }

    public class Elevator : Surface
    {

        public Elevator(GameObject controlSurface) : base(controlSurface)
        {
            Setup();
        }

    }

    public class Aileron : Surface
    {

        public Aileron(GameObject controlSurface) : base(controlSurface)
        {
            Setup();
        }
    }

    void Awake()
    {
        // Rudder
        rudderObject = GameObject.Find("Rudder");  // Get reference to the rudder in Unity
        rudder = new Rudder(rudderObject);

        // Elevators
        leftElevatorObject = GameObject.Find("LeftElevator");
        leftElevator = new Elevator(leftElevatorObject);
        rightElevatorObject = GameObject.Find("RightElevator");
        rightElevator = new Elevator(rightElevatorObject);

        // Ailerons
        leftAileronObject = GameObject.Find("L_Aileron");
        leftAileron = new Aileron(leftAileronObject);
        rightAileronObject = GameObject.Find("R_Aileron");
        rightAileron = new Aileron(rightAileronObject);
    }
}
