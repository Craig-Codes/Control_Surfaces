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

    // Variables used to store rudder starting rotations
    float rudderX, rudderY;

    // Variables used to store elevator starting rotations
    float leftElevatorX, leftElevatorZ, rightElevatorX, rightElevatorZ;

    // Variables used to store aileron strating positions
    float leftAileronX, leftAileronZ, rightAileronX, rightAileronZ;

    static Rudder rudder;


    abstract class Surface
    {
        public GameObject surface;  // Reference to the GameObject manipulated by the class
           public Surface(GameObject controlSurface)  // Constructor
        {
            this.surface = controlSurface;  // pass in the Surface gameobject
        }

        public void test()
        {
            Debug.Log(this.surface);
        }

    }

    class Rudder : Surface
    {
        public Rudder(GameObject controlSurface) : base(controlSurface)
        {
        }

        public void MoveRudder()  // Overloaded method deals with keyboard / mouse input
        {
            //float degrees;

            //if (direction == "left")
            //{
            //    degrees = KEYBOARD_DEGRESS;
            //    this.surface.transform.localEulerAngles = new Vector3(rudderX, rudderY, degrees);

            //}
            //else if (direction == "right")
            //{
            //    degrees = -KEYBOARD_DEGRESS;
            //    this.surface.transform.localEulerAngles = new Vector3(rudderX, rudderY, degrees);
            //}
            //else if (direction == "start")
            //{
            //    this.surface.transform.localEulerAngles = new Vector3(rudderX, rudderY, 0f);  // return to start position
            //}
            //float degrees = currentJoystickCoords.y * degreesPerJoystickMove;
            // rotate around parents pivot point on the y axis to the required degrees out of 20
            Debug.Log(this.surface.transform.localEulerAngles);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Rudder
        rudderObject = GameObject.Find("Rudder");
        // Rudder starting rotations
        rudderX = rudderObject.transform.localEulerAngles.x;
        rudderY = rudderObject.transform.localEulerAngles.y;
        rudderObject.transform.localEulerAngles = new Vector3(rudderX, rudderY, 0);  // start rudder at 0

        // Elevators
        leftElevatorObject = GameObject.Find("LeftElevator");
        rightElevatorObject = GameObject.Find("RightElevator");
        // Elevator starting rotations
        leftElevatorX = leftElevatorObject.transform.localEulerAngles.x;
        leftElevatorZ = leftElevatorObject.transform.localEulerAngles.z;
        rightElevatorX = rightElevatorObject.transform.localEulerAngles.x;
        rightElevatorZ = rightElevatorObject.transform.localEulerAngles.z;
        leftElevatorObject.transform.localEulerAngles = new Vector3(leftElevatorX, 0, leftElevatorZ);  
        rightElevatorObject.transform.localEulerAngles = new Vector3(rightElevatorX, 0, rightElevatorZ);  

        // Ailerons
        leftAileronObject = GameObject.Find("L_Aileron");
        rightAileronObject = GameObject.Find("R_Aileron");
        // Aileron starting rotations
        leftAileronX = leftAileronObject.transform.localEulerAngles.x;
        leftAileronZ = leftAileronObject.transform.localEulerAngles.z;
        rightAileronX = rightAileronObject.transform.localEulerAngles.x;
        rightAileronZ = rightAileronObject.transform.localEulerAngles.z;
        leftAileronObject.transform.localEulerAngles = new Vector3(leftAileronX, 0, leftAileronZ);  
        rightAileronObject.transform.localEulerAngles = new Vector3(rightAileronX, 0, rightAileronZ);

        rudder = new Rudder(rudderObject);
    }

    // Update is called once per frame
    void Update()
    {
        rudder.MoveRudder();
    }
}
