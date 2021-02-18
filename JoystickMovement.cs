using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickMovement : MonoBehaviour
{
    RectTransform joystickHandle; // Get the joystick Handle object
    // Joystick starting positions
    float joystickX, joystickY;
    Vector2 currentJoystickCoords;

    /* Joystick moves from +64 / -64 on both x and y axis
        For y axis - fully up at 64, fully down at -64 
        On each move, need to know the distance from the center, with the control surface being moved (20 degrees) enough to
        map +20 / -20 with the +64 / -64 
        20 / 64 = 0.3125
        We need to rotate by +/-0.3125 for each joystick movement
    */
    float degreesPerJoystickMove = 0.3125f;
    float joystickRadius = 64f;

    private PlayerControls controls; // Get the unity input manager

    // Elevator variables
    private  GameObject leftElevator;
    private  GameObject rightElevator;

    // Aileron variables
    private GameObject leftAileron;
    private GameObject rightAileron;

    // Variables used to store elevator starting rotations
    float leftElevatorX, leftElevatorZ, rightElevatorX, rightElevatorZ;

    // Variables used to store aileron strating positions
    float leftAileronX, leftAileronZ, rightAileronX, rightAileronZ;

    void Awake()
    {
        controls = new PlayerControls();
        // Elevator Keys
        controls.KeyboardInput.ElevatorsUp.performed += context => ButtonJoystickMoveElevators("up");  // context cant be used to get input information
        controls.KeyboardInput.ElevatorsDown.performed += context => ButtonJoystickMoveElevators("down");  // context cant be used to get input information
        controls.KeyboardInput.ElevatorsUpReverse.performed += context => ButtonJoystickMoveElevators("reverse");  // context cant be used to get input information
        controls.KeyboardInput.ElevatorsDownReverse.performed += context => ButtonJoystickMoveElevators("reverse");  // context cant be used to get input information
        // Aileron Keys
        controls.KeyboardInput.AileronsUp.performed += context => ButtonJoystickMoveAilerons("up"); // context cant be used to get input information
        controls.KeyboardInput.AileronsDown.performed += context => ButtonJoystickMoveAilerons("down"); // context cant be used to get input information
        controls.KeyboardInput.AileronsUpReverse.performed += context => ButtonJoystickMoveAilerons("reverse");  // context cant be used to get input information
        controls.KeyboardInput.AileronsDownReverse.performed += context => ButtonJoystickMoveAilerons("reverse");  // context cant be used to get input information

    }

    void Start()
    {
        joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();
        joystickX = joystickHandle.transform.localPosition.x;
        joystickY = joystickHandle.transform.localPosition.y;

        controls.KeyboardInput.Enable();  // Start with the keyboard controls enabled

        currentJoystickCoords = new Vector2(0f, 0f); // Joystick starts in the middle

        // Elevators
        leftElevator = GameObject.Find("LeftElevator");
        rightElevator = GameObject.Find("RightElevator");

        // Elevator starting rotations
        leftElevatorX = leftElevator.transform.localEulerAngles.x;
        leftElevatorZ = leftElevator.transform.localEulerAngles.z;
        rightElevatorX = rightElevator.transform.localEulerAngles.x;
        rightElevatorZ = rightElevator.transform.localEulerAngles.z;

        // Ailerons
        leftAileron = GameObject.Find("L_Aileron");
        rightAileron = GameObject.Find("R_Aileron");

        // Aileron starting rotations
        leftAileronX = leftAileron.transform.localEulerAngles.x;
        leftAileronZ = leftAileron.transform.localEulerAngles.z;
        rightAileronX = rightAileron.transform.localEulerAngles.x;
        rightAileronZ = rightAileron.transform.localEulerAngles.z;

    }

    // Update is called once per frame
    void Update()
    {
        MoveSurfaces();  // Move surfaces based on joystick location
    }

    // MOUSE CONTROL CODE

    private void MoveSurfaces()
    {
        GetCurrentJoystickCoords();
        MoveElevators();
        MoveAilerons();
    }

    public void GetCurrentJoystickCoords()  // Get the coords from the Joystick.cs script each time the joystick is moved.
    {
        joystickX = joystickHandle.localPosition.x;
        joystickY = joystickHandle.localPosition.y;
        currentJoystickCoords = new Vector2(joystickX, joystickY);
        //Debug.Log(currentJoystickCoords);
    }

    private void MoveElevators()
    {

        float degrees = currentJoystickCoords.y * degreesPerJoystickMove;
        // rotate around parents pivot point on the y axis to the required degrees out of 20
        leftElevator.transform.localEulerAngles = new Vector3(leftElevatorX, degrees, leftElevatorZ);
        rightElevator.transform.localEulerAngles = new Vector3(rightElevatorX, -degrees, rightElevatorZ); 
    }

    private void MoveAilerons()
    {

        float degrees = currentJoystickCoords.x * degreesPerJoystickMove;

        leftAileron.transform.localEulerAngles = new Vector3(leftAileronX, -degrees, leftAileronZ);  
        rightAileron.transform.localEulerAngles = new Vector3(rightAileronX, degrees, rightAileronZ);
    }


    //KEYBOARD CONTROL CODE

    private void ButtonJoystickMoveElevators(string direction)
    {
        /* We want the jotstick y to go to the edge of the circle, in relation to the current x position
         * 
         * A circle follows the rule x2 + y2 = r2
         * We know that the radius is 64, and we can get the current x position that the joystick is in
         * From this information we know where to move the y postion to, getting the joystick to the edge of the circle
         * 
         * we re-arrange the equation -> y2 = r2 - x2
         * the answer then becomes the square route of y2
         */

        // get joystick current positions
        float joystickCurrentX = joystickHandle.transform.localPosition.x;
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;

        float xCoord = joystickCurrentX;
        float radius = joystickRadius;

        float yCoord = (radius * radius) - (xCoord * xCoord);
        yCoord = Mathf.Sqrt(yCoord);

        if (direction == "up")
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, yCoord, joystickCurrentZ);
        }
        if (direction == "down")
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, -yCoord, joystickCurrentZ);
        }
        if (direction == "reverse")
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, 0, joystickCurrentZ);
        }
    }

    private void ButtonJoystickMoveAilerons(string direction)
    {
        float joystickCurrentY = joystickHandle.transform.localPosition.y;
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;

        float yCoord = joystickCurrentY;
        float radius = joystickRadius;

        float xCoord = (radius * radius) - (yCoord * yCoord);
        xCoord = Mathf.Sqrt(xCoord);

        if (direction == "up")
        {
            joystickHandle.transform.localPosition = new Vector3(xCoord, joystickCurrentY, joystickCurrentZ);
        }
        if (direction == "down")
        {
            joystickHandle.transform.localPosition = new Vector3(-xCoord, joystickCurrentY, joystickCurrentZ);
        }
        if (direction == "reverse")
        {
            joystickHandle.transform.localPosition = new Vector3(0, joystickCurrentY, joystickCurrentZ);
        }
        MoveSurfaces();  // move surfaces based on joystick location
    }
}
