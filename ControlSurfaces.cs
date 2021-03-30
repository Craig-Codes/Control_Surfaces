using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSurfaces : MonoBehaviour
{
    // Control Surface Instantiated Objects
    public static Rudder rudder;  // Only one instance of class shared across all scripts - once created can be called from other scripts
    public static Surface leftElevator;
    public static Surface rightElevator;
    public static Surface leftAileron;
    public static Surface rightAileron;

    // Control Surface References used to instantiate objects:
    private GameObject rudderObject;
    private GameObject leftElevatorObject;
    private GameObject rightElevatorObject;
    private GameObject leftAileronObject;
    private GameObject rightAileronObject;

    public class Surface
    {
        // readonly property can only be set once, in the constructor
        // protected properties lets you access the property in derived classes (e.g. Rudder can use these in its instance). 
        protected readonly GameObject controlSurface;  // Reference to the GameObject manipulated by the class - internal means can be seen in derived classes
        protected readonly float startingPositionX, startingPositionY, startingPositionZ;  // angles of each axis
        protected readonly float currentPositionX, currentPositionY, currentPositionZ;  // current position

        public Surface(GameObject controlSurface)  // Constructor
        {
            this.controlSurface = controlSurface;  // pass in the Surface gameobject
            // Get the control surfaces current starting axis angles
            this.startingPositionX = this.controlSurface.transform.localEulerAngles.x;
            this.startingPositionY = this.controlSurface.transform.localEulerAngles.y;
            this.startingPositionZ = this.controlSurface.transform.localEulerAngles.z;
            Setup();  // Set the axis the object is rotated by to 0 at the start
        }

        public virtual void Setup()  // virtual method can be overwritten in derived classes, as different axis are used for each control surface
        {
            this.controlSurface.transform.localEulerAngles = new Vector3(this.startingPositionX, 0, this.startingPositionZ);  // start y rotation value at 0
        }

        public Vector3 GetStartingRotations()  // Returns the starting rotations set in constructor
        {
            return new Vector3(this.startingPositionX, this.startingPositionY, this.startingPositionZ);
        }

        public Vector3 GetCurrentRotations()  // Returns the current rotations set in constructor
        {
            return new Vector3(this.controlSurface.transform.localEulerAngles.x, this.controlSurface.transform.localEulerAngles.y, this.controlSurface.transform.localEulerAngles.z);
        }

        public void Rotate(Vector3 angle)  // rotate the surface by angle argument
        {
            this.controlSurface.transform.localEulerAngles = new Vector3(angle.x, angle.y, angle.z);
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
            this.controlSurface.transform.localEulerAngles = new Vector3(this.startingPositionX, this.startingPositionY, 0);  // start z rotation value at 0 (ailerons and elevators)
        }
    }

    void Awake()
    {
        // Rudder
        rudderObject = GameObject.Find("Rudder");  // Get reference to the rudder in Unity
        rudder = new Rudder(rudderObject);

        // Elevators
        leftElevatorObject = GameObject.Find("LeftElevator");
        leftElevator = new Surface(leftElevatorObject);
        rightElevatorObject = GameObject.Find("RightElevator");
        rightElevator = new Surface(rightElevatorObject);

        // Ailerons
        leftAileronObject = GameObject.Find("L_Aileron");
        leftAileron = new Surface(leftAileronObject);
        rightAileronObject = GameObject.Find("R_Aileron");
        rightAileron = new Surface(rightAileronObject);
    }
}
