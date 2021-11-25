/* Script controls gamepad interactions for the UI throttle, flaps, pedals and
Joystick */

using UnityEngine;

public class GamepadControls : MonoBehaviour
{
    /* Gamepad joystick input is 1 / -1. Need to make this 64 / -64 to map to UI
    joystick. We can simply multiply the input by 64 */
    private const float JOYSTICK_MULTIPLIER = 64f;

    // Get the joystick Handle object
    private RectTransform joystickHandle;

    // Get the reference to the player controller input from the Unity Engine
    private PlayerControls controls;

    void Awake()
    {
        /* Create the player controller object in the code, linking the Unity
        Engine input system to script functions */
        controls = new PlayerControls();
        /* context refers to any input data when a controlpad button is pressed,
        providng input information which can be used */
        controls.ControllerInput.RightThumbstick.performed += context =>
            MoveUiJoystick(context.ReadValue<Vector2>());
        controls.ControllerInput.RudderLeftDown.performed += context =>
            ControlsUtilityMethods.PedalDownKeyboard("left");
        controls.ControllerInput.RudderLeftUp.performed += context =>
            ControlsUtilityMethods.PedalBothUp();
        controls.ControllerInput.RudderRightDown.performed += context =>
            ControlsUtilityMethods.PedalDownKeyboard("right");
        controls.ControllerInput.RudderRightUp.performed += context =>
            ControlsUtilityMethods.PedalBothUp();
        controls.ControllerInput.FlapsDown.performed += context =>
            ControlsUtilityMethods.MoveFlapsDown();
        controls.ControllerInput.FlapsUp.performed += context =>
            ControlsUtilityMethods.MoveFlapsUp();
        controls.ControllerInput.LeftThumbstick.performed += context =>
            ControlsUtilityMethods.MoveUiThrottle(context.ReadValue<Vector2>());
    }
    // Start is called before the first frame update
    void Start()
    {
        controls.ControllerInput.Enable();  // Start with the gamepad controls enabled
        // Access the joystick object
        joystickHandle =
            GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();
    }

    // Move the UI Joystick based on input coords of gamepad right stick 
    private void MoveUiJoystick(Vector2 context)
    {
        // used to keep z axis position the same in the 3D space
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;

        Vector2 rightStickCoords = context;
        // UI Joystick follows the gamepad joystick each time its moved
        joystickHandle.transform.localPosition = new Vector3(rightStickCoords.x * JOYSTICK_MULTIPLIER,
                                                             rightStickCoords.y * JOYSTICK_MULTIPLIER,
                                                             joystickCurrentZ);
    }
}
