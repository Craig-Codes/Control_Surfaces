// Script creates the control surface objects and provides methods for them

using UnityEngine;

public class ControlSurfaces : MonoBehaviour
{
    /* Control Surface Instantiated Object references. Static modifier used so that only 
    one instance of each object shared across all scripts after a single instantiation */
    public static Rudder rudder;
    public static Surface leftElevator;
    public static Surface rightElevator;
    public static Surface leftAileron;
    public static Surface rightAileron;
    public static Surface leftFlap;
    public static Surface rightFlap;

    // Control Surface References used to instantiate surface objects
    private GameObject rudderObject;
    private GameObject leftElevatorObject;
    private GameObject rightElevatorObject;
    private GameObject leftAileronObject;
    private GameObject rightAileronObject;
    private GameObject leftFlapObject;
    private GameObject rightFlapObject;

    public class Surface
    {
        /* protected readonly properties can be accessed in derived classes (e.g. Rudder
        can use these in its instance) and cannot be modified after the object 
        is instantiated. */

        // Reference to the GameObject manipulated by the class
        protected readonly GameObject controlSurface;
        // starting angles of each axis
        protected readonly float startingPositionX, startingPositionY, startingPositionZ;

        public Surface(GameObject controlSurface)  // Constructor
        {
            this.controlSurface = controlSurface;  // pass in the Surface gameobject
            // Get the control surfaces current starting axis angles
            this.startingPositionX = this.controlSurface.transform.localEulerAngles.x;
            this.startingPositionY = this.controlSurface.transform.localEulerAngles.y;
            this.startingPositionZ = this.controlSurface.transform.localEulerAngles.z;
            Setup();  // Ensure the starting axis positions are correct to remove bugs
        }

        /* Virtual methods can be overwritten in derived classes, as different axis are 
        used for each control surface. This ensures we can setup other Surface types 
        differently if they rotate on a different axis.Setup method ensures the y axis 
        starts at 0 to avoid any bugs */
        public virtual void Setup()
        {
            this.controlSurface.transform.localEulerAngles =
                new Vector3(this.startingPositionX, 0, this.startingPositionZ);
        }

        // Method returns the starting rotations set in the constructor
        public Vector3 GetStartingRotations()
        {
            return new Vector3(this.startingPositionX,
                this.startingPositionY, this.startingPositionZ);
        }

        // Method returns a Surfaces current rotation values 
        public Vector3 GetCurrentRotations()
        {
            return new Vector3(this.controlSurface.transform.localEulerAngles.x,
                this.controlSurface.transform.localEulerAngles.y,
                this.controlSurface.transform.localEulerAngles.z);
        }

        // Method rotates the surface to the angle provided in the argument
        public void Rotate(Vector3 angle)  // rotate the surface by angle argument
        {
            this.controlSurface.transform.localEulerAngles =
                new Vector3(angle.x, angle.y, angle.z);
        }
    }

    /* Rudder class is derived from the Surface class.
    Rudder uses override methods to change how Surface virtual methods are implemented */
    public class Rudder : Surface
    {
        public Rudder(GameObject controlSurface) : base(controlSurface)
        {
            /* The base keyword is required to access member variables from the base class
            This ensures Rudder contains the same variables Surface contains */
            Setup();
        }
        public override void Setup()
        /* Overide method ensures the z axis is set to 0, 
        rather than the y axis in generic surfaces which are horizontal */
        {
            this.controlSurface.transform.localEulerAngles =
                new Vector3(this.startingPositionX, this.startingPositionY, 0);
        }
    }

    /* Awake method is inbuild in Unity MonoBehaviour derived classes
    The method is triggered first, ensuring each Surface object is correctly created */
    void Awake()
    {
        // Rudder
        // Get reference to the rudder in Unity
        rudderObject = GameObject.Find("Rudder");
        // Create the Surface object using the obtained reference
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

        //Flaps
        leftFlapObject = GameObject.Find("L_Flap");
        leftFlap = new Surface(leftFlapObject);
        rightFlapObject = GameObject.Find("R_Flap");
        rightFlap = new Surface(rightFlapObject);
    }
}
