using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class KeyboardControls : MonoBehaviour
{
    //private GameObject rudder;
    private static Button leftPedal;
    private static Button rightPedal;
    private Vector3 pedalFullsize = new Vector3(0.75f, 0.75f, 0.75f);
    private Vector3 pedalSmallsize = new Vector3(0.6f, 0.6f, 0.6f);

    ControlSurfaces.Rudder rudder;
    ControlSurfaces.Aileron leftAileron;
    ControlSurfaces.Aileron rightAileron;
    ControlSurfaces.Elevator leftElevator;
    ControlSurfaces.Elevator rightElevator;


    // Joystick
    RectTransform joystickHandle; // Get the joystick Handle object
    // Joystick starting positions
    public static float joystickX, joystickY;
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

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        // Rudder Keys
        controls.KeyboardInput.RudderLeftDown.performed += context => LeftPedalDownKeyboard();// context cant be used to get input information
        controls.KeyboardInput.RudderLeftUp.performed += context => LeftPedalUpKeyboard();  
        controls.KeyboardInput.RudderRightDown.performed += context => RightPedalDownKeyboard(); 
        controls.KeyboardInput.RudderRightUp.performed += context => RightPedalUpKeyboard();  
        // Elevator Keys                                                                                      
        controls.KeyboardInput.ElevatorsUp.performed += context => ButtonJoystickMoveElevators("down");  
        controls.KeyboardInput.ElevatorsDown.performed += context => ButtonJoystickMoveElevators("up");  
        controls.KeyboardInput.ElevatorsUpReverse.performed += context => ButtonJoystickMoveElevators("reverse");  
        controls.KeyboardInput.ElevatorsDownReverse.performed += context => ButtonJoystickMoveElevators("reverse"); 
        // Aileron Keys
        controls.KeyboardInput.AileronsUp.performed += context => ButtonJoystickMoveAilerons("up"); 
        controls.KeyboardInput.AileronsDown.performed += context => ButtonJoystickMoveAilerons("down"); 
        controls.KeyboardInput.AileronsUpReverse.performed += context => ButtonJoystickMoveAilerons("reverse"); 
        controls.KeyboardInput.AileronsDownReverse.performed += context => ButtonJoystickMoveAilerons("reverse");  

    }
    // Start is called before the first frame update
    void Start()
    {
        rudder = ControlSurfaces.rudder;
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();

        leftAileron = ControlSurfaces.leftAileron;
        rightAileron = ControlSurfaces.rightAileron;

        leftElevator = ControlSurfaces.leftElevator;
        rightElevator = ControlSurfaces.rightElevator;

        // Joystick setup
        joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();
        joystickX = joystickHandle.transform.localPosition.x;
        joystickY = joystickHandle.transform.localPosition.y;
        currentJoystickCoords = new Vector2(0f, 0f); // Joystick starts in the middle
        /* We want the jotstick y to go to the edge of the circle, in relation to the current x position
        * A circle follows the rule x2 + y2 = r2
        * We know that the radius is 64, and we can get the current x position that the joystick is in
        * From this information we know where to move the y postion to, getting the joystick to the edge of the circle
 
        * we re-arrange the equation -> y2 = r2 - x2
        * the answer then becomes the square route of y2
        */

        controls.KeyboardInput.Enable();  // Start with the keyboard controls enabled
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //////////////////////////////////////////////////////////////////////
    /////////////////////////// PEDALS //////////////////////////////////
    ////////////////////////////////////////////////////////////////////

    private void LeftPedalDownKeyboard()
    {
        PedalDown(leftPedal);
        rudder.MoveRudder("left");
        rightPedal.transform.localScale = pedalFullsize;
    }

    private void LeftPedalUpKeyboard()
    {
        PedalUp(leftPedal);
        rudder.MoveRudder("start");
        rightPedal.transform.localScale = pedalFullsize;
    }

    private void RightPedalDownKeyboard()
    {
        PedalDown(rightPedal);
        rudder.MoveRudder("right");
        leftPedal.transform.localScale = pedalFullsize;
    }

    private void RightPedalUpKeyboard()
    {
        PedalUp(rightPedal);
        rudder.MoveRudder("start");
        leftPedal.transform.localScale = pedalFullsize;
    }


    private void PedalDown(Button pedal)
    {
        pedal.transform.localScale = pedalSmallsize; // make pedal smaller to visualise foot press
    }

    private void PedalUp(Button pedal)
    {
        pedal.transform.localScale = pedalFullsize;

    }


    //////////////////////////////////////////////////////////////////////
    ///////////////////////// JOYSTICK //////////////////////////////////
    ////////////////////////////////////////////////////////////////////
    public void GetCurrentJoystickCoords()  // Get the coords from the Joystick.cs script each time the joystick is moved.
    {
        joystickX = joystickHandle.localPosition.x;
        joystickY = joystickHandle.localPosition.y;
        currentJoystickCoords = new Vector2(joystickX, joystickY);
        //Debug.Log(currentJoystickCoords);
    }

    private void ButtonJoystickMoveElevators(string direction)
    {
        // get joystick current positions
        float joystickCurrentX = joystickHandle.transform.localPosition.x;
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;

        float xCoord = joystickCurrentX;
        float radius = joystickRadius;

        float yCoord = (radius * radius) - (xCoord * xCoord);
        yCoord = Mathf.Sqrt(yCoord);

        // Move the Joystick
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
        RotateSurfaces();  // Move surface based on the Joystick Position
    }

    private void ButtonJoystickMoveAilerons(string direction)
    {
        float joystickCurrentY = joystickHandle.transform.localPosition.y;
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;

        float yCoord = joystickCurrentY;
        float radius = joystickRadius;

        float xCoord = (radius * radius) - (yCoord * yCoord);
        xCoord = Mathf.Sqrt(xCoord);


        // Move the Joystick
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
        RotateSurfaces();  // move surfaces based on joystick location
    }

    private void RotateSurfaces()
    {
        GetCurrentJoystickCoords();
        MoveElevators();
        MoveAilerons();
    }

    private void MoveElevators()
    {
        float degrees = currentJoystickCoords.y * degreesPerJoystickMove;
        // rotate around parents pivot point on the y axis to the required degrees out of 20
        Vector3 leftElevatorRotation = new Vector3(leftElevator.x, degrees, leftElevator.z);
        leftElevator.Rotate(leftElevatorRotation);
        Vector3 rightElevatorRotation = new Vector3(rightElevator.x, -degrees, rightElevator.z);
        rightElevator.Rotate(rightElevatorRotation);
    }

    private void MoveAilerons()
    {
        float degrees = currentJoystickCoords.x * degreesPerJoystickMove;
        Vector3 leftAileronRotation = new Vector3(leftAileron.x, -degrees, leftAileron.z);
        leftAileron.Rotate(leftAileronRotation);
        Vector3 rightAileronRotation = new Vector3(rightAileron.x, degrees, rightAileron.z);
        rightAileron.Rotate(rightAileronRotation);
    }
}
