/* Script controls keyboard interactions for the UI throttle, flaps, pedals and
Joystick */

using UnityEngine;
using UnityEngine.UI;

public class KeyboardControls : MonoBehaviour
{
    // Variable stores a reference to the UI joystick
    private RectTransform joystickHandle;
    // UI Joystick has a radius of 64 which is used in calculations
    private const float JOYSTICK_RADIUS = 64f;
    // Variable stores Player controller object from Unity engine
    private PlayerControls controls;
    // Reference to Throttle Slider
    private Slider throttleSlider;
    // Float controls how much to move the throttle slider each key press
    private const float SLIDER_INCREMENT_VALUE = 1;

    /* Boolean values used as flags to find half way positions where both
    elevator and aileron keys are pressed at the same time*/
    private bool elevatorsUp = false;
    private bool aileronsUp = false;
    private bool elevatorsDown = false;
    private bool aileronsDown = false;

    // Varaible stores the UI joystick Z coord, which always remains the same
    float joystickCurrentZ;

    // Awake method called when game loads
    void Awake()
    {
        /* Get a reference to the Unity engine Player Controls.
        Unity Input system maps performed function to key press */
        controls = new PlayerControls();
        // Each keyboard input is mapped to a function call Rudder Keys
        controls.KeyboardInput.RudderLeftDown.performed += context =>
            ControlsUtilityMethods.PedalDownKeyboard("left");
        controls.KeyboardInput.RudderLeftUp.performed += context =>
            ControlsUtilityMethods.PedalBothUp();
        controls.KeyboardInput.RudderRightDown.performed += context =>
            ControlsUtilityMethods.PedalDownKeyboard("right");
        controls.KeyboardInput.RudderRightUp.performed += context =>
            ControlsUtilityMethods.PedalBothUp();
        // Elevator Keys
        controls.KeyboardInput.ElevatorsUp.performed += context =>
            ButtonJoystickMoveElevators("down");
        controls.KeyboardInput.ElevatorsDown.performed += context =>
            ButtonJoystickMoveElevators("up");
        controls.KeyboardInput.ElevatorsUpReverse.performed += context =>
            ButtonJoystickMoveElevators("reverse");
        controls.KeyboardInput.ElevatorsDownReverse.performed += context =>
            ButtonJoystickMoveElevators("reverse");
        // Aileron Keys
        controls.KeyboardInput.AileronsUp.performed += context =>
            ButtonJoystickMoveAilerons("up");
        controls.KeyboardInput.AileronsDown.performed += context =>
            ButtonJoystickMoveAilerons("down");
        controls.KeyboardInput.AileronsUpReverse.performed += context =>
            ButtonJoystickMoveAilerons("reverse");
        controls.KeyboardInput.AileronsDownReverse.performed += context =>
            ButtonJoystickMoveAilerons("reverse");
        // Flaps Keys
        controls.KeyboardInput.FlapsDown.performed += context =>
            ControlsUtilityMethods.MoveFlapsDown();
        controls.KeyboardInput.FlapsUp.performed += context =>
            ControlsUtilityMethods.MoveFlapsUp();
        // Throttle Keys
        controls.KeyboardInput.ThrottleDown.performed += context =>
            MoveThrottle("down");
        controls.KeyboardInput.ThrottleUp.performed += context =>
            MoveThrottle("up");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Joystick setup
        joystickHandle =
            GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();
        // Throttle setup
        throttleSlider =
            GameObject.FindGameObjectWithTag("ThrottleSlider").GetComponent<Slider>();
        // Start with the keyboard controls enabled
        controls.KeyboardInput.Enable();
        // get the UI Joystick Z starting coord
        joystickCurrentZ = joystickHandle.transform.localPosition.z;
    }

    /* Joystick Calculations
        
       Joystick works on a circle, so the edge position varies based on the
       relationship between the x and y points. If x and y are both max (64)
       then the joystick needs to be sitting between both points, on the edge of
       the circle base rather than the corner of a square shape.

       A circle follows the rule x2 + y2 = r2

       We know that the radius is 64, and we can get the current x or y position
       that the joystick is in. From this information we know where to move the
       opposing x or y postion to, getting the joystick to the edge of the
       circle base.

       E.g to get the y coord we re-arrange the equation -> y2 = r2 - x2 the
       answer then becomes the square route of y2 to get y.
      */

    /* Move the joystick position based on key press - this then causes the
    control surfaces to deflect. Elevators deflect based on UI joystick x axis
    position */
    private void ButtonJoystickMoveElevators(string direction)
    {
        // get joystick current positions
        float joystickCurrentX = joystickHandle.transform.localPosition.x;
        // Use the joystick x coordinate to workout the y coordinate
        float xCoord = joystickCurrentX;
        float radius = JOYSTICK_RADIUS;

        // Calculate the corresponding y coordinate
        float yCoord = (radius * radius) - (xCoord * xCoord);
        // y coordinate on the circle based on the x value
        yCoord = Mathf.Sqrt(yCoord);

        // Move the Joystick
        if (direction == "up")
        {
            joystickHandle.transform.localPosition =
                new Vector3(joystickCurrentX, yCoord, joystickCurrentZ);
            elevatorsUp = true;
        }
        else if (direction == "down")
        {
            joystickHandle.transform.localPosition =
                 new Vector3(joystickCurrentX, -yCoord, joystickCurrentZ);
            elevatorsDown = true;
        }
        else if (direction == "reverse")  // Return to centre
        {
            joystickHandle.transform.localPosition =
                new Vector3(joystickCurrentX, 0, joystickCurrentZ);
            elevatorsUp = false;
            elevatorsDown = false;
        }
        // Actions if both ailerons and elevators are pressed
        JoystickAileronsAndElevators();
    }

    private void ButtonJoystickMoveAilerons(string direction)
    {
        float joystickCurrentY = joystickHandle.transform.localPosition.y;
        float yCoord = joystickCurrentY;
        float radius = JOYSTICK_RADIUS;

        float xCoord = (radius * radius) - (yCoord * yCoord);
        xCoord = Mathf.Sqrt(xCoord);

        if (direction == "up")
        {
            joystickHandle.transform.localPosition =
                new Vector3(xCoord, joystickCurrentY, joystickCurrentZ);
            aileronsUp = true;
        }
        else if (direction == "down")
        {
            joystickHandle.transform.localPosition =
                new Vector3(-xCoord, joystickCurrentY, joystickCurrentZ);
            aileronsDown = true;
        }
        else if (direction == "reverse")
        {
            joystickHandle.transform.localPosition =
                new Vector3(0, joystickCurrentY, joystickCurrentZ);
            aileronsUp = false;
            aileronsDown = false;
        }
        JoystickAileronsAndElevators();
    }

    /* Method checks to see if both ailerons and elevators are pressed, moving
    the UI joystick accordingly within a circle */
    private void JoystickAileronsAndElevators()
    {
        if (aileronsUp && elevatorsUp)
        {
            joystickHandle.transform.localPosition =
                new Vector3(JOYSTICK_RADIUS / 2, JOYSTICK_RADIUS / 2, joystickCurrentZ);
        }
        else if (aileronsUp && elevatorsDown)
        {
            joystickHandle.transform.localPosition =
                new Vector3(JOYSTICK_RADIUS / 2, -JOYSTICK_RADIUS / 2, joystickCurrentZ);
        }
        else if (aileronsDown && elevatorsUp)
        {
            joystickHandle.transform.localPosition =
                new Vector3(-JOYSTICK_RADIUS / 2, JOYSTICK_RADIUS / 2, joystickCurrentZ);
        }
        else if (aileronsDown && elevatorsDown)
        {
            joystickHandle.transform.localPosition =
                new Vector3(-JOYSTICK_RADIUS / 2, -JOYSTICK_RADIUS / 2, joystickCurrentZ);
        }
    }

    /* Method controls throttle slider value (position), either moving it up or
    down based on which key is pressed */
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
