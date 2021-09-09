// Script controls keyboard interactions for the UI throttle, flaps, pedals and Joystick

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class KeyboardControls : MonoBehaviour
{
    private RectTransform joystickHandle; // Variable stores a reference to the UI joystick
    private const float JOYSTICK_RADIUS = 64f;  // UI Joystick has a radius of 64 which is used in calculations

    private PlayerControls controls;  // Varaible stores Player controller object from Unity engine

    private Slider throttleSlider;  // Reference to Throttle SLIDER
    private const float SLIDER_INCREMENT_VALUE = 1;  // Float controls how much to move the throttle slider each key press

    // Boolean values used as flags to find half way positions where both elevator and aileron keys are pressed
    private bool elevatorsUp = false;
    private bool aileronsUp = false;
    private bool elevatorsDown = false;
    private bool aileronsDown = false;

    float joystickCurrentZ;  // Varaible stores the UI joystick Z coord, which always remains the same

    void Awake()
    {
        controls = new PlayerControls();  // Get a reference to the Unity engine Player Controls
        // Each keyboard input is mapped to a function call
        // Rudder Keys
        controls.KeyboardInput.RudderLeftDown.performed += context => ControlsUtilityMethods.PedalDownKeyboard("left");// context cant be used to get input information
        controls.KeyboardInput.RudderLeftUp.performed += context => ControlsUtilityMethods.PedalBothUp();  
        controls.KeyboardInput.RudderRightDown.performed += context => ControlsUtilityMethods.PedalDownKeyboard("right"); 
        controls.KeyboardInput.RudderRightUp.performed += context => ControlsUtilityMethods.PedalBothUp();  
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
        // Flaps Keys
        controls.KeyboardInput.FlapsDown.performed += context => ControlsUtilityMethods.MoveFlapsDown();
        controls.KeyboardInput.FlapsUp.performed += context => ControlsUtilityMethods.MoveFlapsUp();
        // Throttle Keys
        controls.KeyboardInput.ThrottleDown.performed += context => MoveThrottle("down");
        controls.KeyboardInput.ThrottleUp.performed += context => MoveThrottle("up");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Joystick setup
        joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();
        // Throttle setup
        throttleSlider = GameObject.FindGameObjectWithTag("ThrottleSlider").GetComponent<Slider>();
        controls.KeyboardInput.Enable();  // Start with the keyboard controls enabled

        joystickCurrentZ = joystickHandle.transform.localPosition.z;  // get the UI Joystick Z coord
    }


    // Joystick Calculations

    /* Joystick works on a circle, so the edge position varies based on the relationship
       between the x and y points. If x and y are both max (64) then the joystick needs
       to be sitting between both point, on the edge of the circle base rather than the corner of a square shape.
     
       A circle follows the rule x2 + y2 = r2

       We know that the radius is 64, and we can get the current x or y position that the joystick is in.
       From this information we know where to move the opposing x or y postion to, 
       getting the joystick to the edge of the circle base.

       E.g to get the y coord we re-arrange the equation -> y2 = r2 - x2
       the answer then becomes the square route of y2 to get y.
      */

    // Move the joystick position based on key press - this then causes the control surfaces to deflect
    // Elevators deflect based on UI joystick x axis position
    private void ButtonJoystickMoveElevators(string direction)
    {
        // get joystick current positions
        float joystickCurrentX = joystickHandle.transform.localPosition.x;
        float xCoord = joystickCurrentX;  // Always use the joystick x coordinate to workout the y coordinate
        float radius = JOYSTICK_RADIUS;

        // Calculate the corresponding y coordinate
        float yCoord = (radius * radius) - (xCoord * xCoord);
        yCoord = Mathf.Sqrt(yCoord);  // y coordinate on the circle based on the x value

        // Move the Joystick
        if (direction == "up")
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, yCoord, joystickCurrentZ);
            elevatorsUp = true;
        }
        else if (direction == "down")
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, -yCoord, joystickCurrentZ);
            elevatorsDown = true;
        }
        else if (direction == "reverse")  // Return to centre
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, 0, joystickCurrentZ);
            elevatorsUp = false;
            elevatorsDown = false;
        }

        JoystickAileronsAndElevators(); // Actions if both ailerons and elevators are pressed
    }

    private void ButtonJoystickMoveAilerons(string direction)
    {
        float joystickCurrentY = joystickHandle.transform.localPosition.y;
        float yCoord = joystickCurrentY;
        float radius = JOYSTICK_RADIUS;

        float xCoord = (radius * radius) - (yCoord * yCoord);
        xCoord = Mathf.Sqrt(xCoord);

        // Move the Joystick
        if (direction == "up")
        {
            joystickHandle.transform.localPosition = new Vector3(xCoord, joystickCurrentY, joystickCurrentZ);
            aileronsUp = true;
        }
        else if (direction == "down")
        {
            joystickHandle.transform.localPosition = new Vector3(-xCoord, joystickCurrentY, joystickCurrentZ);
            aileronsDown = true;
        }
        else if (direction == "reverse")
        {
            joystickHandle.transform.localPosition = new Vector3(0, joystickCurrentY, joystickCurrentZ);
            aileronsUp = false;
            aileronsDown = false;
        }

        JoystickAileronsAndElevators(); // Actions if both ailerons and elevators are pressed
    }

    // Method checks to see if both ailerons and elevators are pressed, moving the UI joystick accordingly
    private void JoystickAileronsAndElevators()
    {
        if (aileronsUp && elevatorsUp)  // If both Elevators and ailerons are deflecting 'up'
        {
            joystickHandle.transform.localPosition = new Vector3(JOYSTICK_RADIUS / 2, JOYSTICK_RADIUS / 2, joystickCurrentZ);
        }
        else if (aileronsUp && elevatorsDown)  // If both Elevators and ailerons are deflecting 'up'
        {
            joystickHandle.transform.localPosition = new Vector3(JOYSTICK_RADIUS / 2, -JOYSTICK_RADIUS / 2, joystickCurrentZ);
        }
        else if (aileronsDown && elevatorsUp)  // If both Elevators and ailerons are deflecting 'up'
        {
            joystickHandle.transform.localPosition = new Vector3(-JOYSTICK_RADIUS / 2, JOYSTICK_RADIUS / 2, joystickCurrentZ);
        }
        else if (aileronsDown && elevatorsDown)  // If both Elevators and ailerons are deflecting 'up'
        {
            joystickHandle.transform.localPosition = new Vector3(-JOYSTICK_RADIUS / 2, -JOYSTICK_RADIUS / 2, joystickCurrentZ);
        }
    }

    // Method controls throttle slider value (position), either moving it up or down based on which key is pressed
    public void MoveThrottle(string direction)
    {
        if (direction == "up")  // Faster
        {
            throttleSlider.value += SLIDER_INCREMENT_VALUE;
        }

        if (direction == "down")  // Slower
        {
            throttleSlider.value -= SLIDER_INCREMENT_VALUE;
        }
    }  
}
