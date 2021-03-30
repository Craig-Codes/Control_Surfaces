using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class KeyboardControls : MonoBehaviour
{
    //private GameObject rudder;
    private Button leftPedal;
    private Button rightPedal;

    // Joystick
    private RectTransform joystickHandle; // Get the joystick Handle object
    private const float joystickRadius = 64f;

    float keyboardDegrees = 20f;  // number of degrees to move a surface on keyboard press

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
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();

        // Joystick setup
        joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();

        controls.KeyboardInput.Enable();  // Start with the keyboard controls enabled
    }

    //////////////////////////////////////////////////////////////////////
    /////////////////////////// PEDALS //////////////////////////////////
    ////////////////////////////////////////////////////////////////////

    private void LeftPedalDownKeyboard()
    {
        ControlsUtilityMethods.PedalDown(leftPedal, keyboardDegrees);
        ControlsUtilityMethods.PedalUp(rightPedal);
    }

    private void LeftPedalUpKeyboard()
    {
        ControlsUtilityMethods.PedalBothUp();
    }

    private void RightPedalDownKeyboard()
    {
        ControlsUtilityMethods.PedalDown(rightPedal, keyboardDegrees);
        ControlsUtilityMethods.PedalUp(leftPedal);
    }

    private void RightPedalUpKeyboard()
    {
        ControlsUtilityMethods.PedalBothUp();
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
        if (direction == "down")
        {
            joystickHandle.transform.localPosition = new Vector3(joystickCurrentX, -yCoord, joystickCurrentZ);
        }
        if (direction == "reverse")
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
        if (direction == "down")
        {
            joystickHandle.transform.localPosition = new Vector3(-xCoord, joystickCurrentY, joystickCurrentZ);
        }
        if (direction == "reverse")
        {
            joystickHandle.transform.localPosition = new Vector3(0, joystickCurrentY, joystickCurrentZ);
        }
        ControlsUtilityMethods.RotateSurfaces();  // rotate surfaces based on joystick location
    }
}
