using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler   // Intefaces to interact with the Unity event system
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

    // Start is called before the first frame update
    void Awake()
    {
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();
    }
    void Start()
    {
        rudder = ControlSurfaces.rudder;
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
    }


    //////////////////////////////////////////////////////////////////////
    /////////////////////////// PEDALS //////////////////////////////////
    ////////////////////////////////////////////////////////////////////

    private void PedalDown(Button pedal)
    {
        pedal.transform.localScale = pedalSmallsize; // make pedal smaller to visualise foot press
    }

    private void PedalUp(Button pedal)
    {
        pedal.transform.localScale = pedalFullsize;

    }


    // Event System and Standalone input module added to buttons in Unity to detect MouseDown and MouseUp events
    // Buttons must also have the script attatched
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.selectedObject.name == "L_Pedal")
        {
            PedalDown(leftPedal);
            rightPedal.transform.localScale = pedalFullsize;
            rudder.MoveRudder("left");
        }
        if (eventData.selectedObject.name == "R_Pedal")
        {
            PedalDown(rightPedal);
            leftPedal.transform.localScale = pedalFullsize;
            rudder.MoveRudder("right");
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.selectedObject.name == "L_Pedal")
        {
            PedalUp(leftPedal);
            PedalUp(rightPedal);
            rudder.MoveRudder("start");
        }
        else if (eventData.selectedObject.name == "R_Pedal")
        {
            PedalUp(rightPedal);
            PedalUp(leftPedal);
            rudder.MoveRudder("start");
        }
        else
        {
            rightPedal.transform.localScale = pedalFullsize;
            leftPedal.transform.localScale = pedalFullsize;
        }
    }


    //////////////////////////////////////////////////////////////////////
    ///////////////////////// JOYSTICK //////////////////////////////////
    ////////////////////////////////////////////////////////////////////

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

    // Update is called once per frame
    void Update()
    {
        MoveSurfaces();  // Each frame we want to move elevators and ailerons based on joysticks location
    }

}
