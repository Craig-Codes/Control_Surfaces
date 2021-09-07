using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class KeyboardControls : MonoBehaviour
{
    // Joystick
    private RectTransform joystickHandle; // Get the joystick Handle object
    private const float joystickRadius = 64f;

    private PlayerControls controls;

    // Reference to Throttle
    private Slider throttleSlider;
    private float sliderIncrementValue = 1;

    void Awake()
    {
        controls = new PlayerControls();
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
    }


    //////////////////////////////////////////////////////////////////////
    ///////////////////////// JOYSTICK //////////////////////////////////
    ////////////////////////////////////////////////////////////////////

    /* Joystick works on a circle, so the edge position varies based on the relationship
     * between the x and y points. If x and y are both max (64) then the joystick needs
     * to be sitting between both point, on the edge of the circle base.
     
     * A circle follows the rule x2 + y2 = r2
     * We know that the radius is 64, and we can get the current x or y position that the joystick is in
     * From this information we know where to move the opposing x or y postion to, 
     * getting the joystick to the edge of the circle.

     * E.g to get the y coord we re-arrange the equation -> y2 = r2 - x2
     * the answer then becomes the square route of y2 to get y.
      */

    // Move the joystick position based on key press - this then causes the control surfaces to deflect
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
        else if (direction == "down")
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, -yCoord, joystickCurrentZ);
        }
        else if (direction == "reverse")
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, 0, joystickCurrentZ);
        }
        ControlsUtilityMethods.RotateSurfaces();  // rotate surfaces based on joystick location
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
        else if (direction == "down")
        {
            joystickHandle.transform.localPosition = new Vector3(-xCoord, joystickCurrentY, joystickCurrentZ);
        }
        else if (direction == "reverse")
        {
            joystickHandle.transform.localPosition = new Vector3(0, joystickCurrentY, joystickCurrentZ);
        }
        ControlsUtilityMethods.RotateSurfaces();  // rotate surfaces based on joystick location
    }

    // Throttle
    public void MoveThrottle(string direction)
    {
        if (direction == "up")  // Faster
        {
            throttleSlider.value += sliderIncrementValue;
        }

        if (direction == "down")  // Slower
        {
            throttleSlider.value -= sliderIncrementValue;
        }
    }
    
}
