// Script controls gamepad interactions for the UI throttle, flaps, pedals and Joystick

using UnityEngine;

public class GamepadControls : MonoBehaviour
{
    // Gamepad joystick input is 1 / -1. Need to make this 64 / -64 to map to UI joystick
    private const float JOYSTICK_MULTIPLIER = 64f;  

    private RectTransform joystickHandle; // Get the joystick Handle object

    private PlayerControls controls;  // Get the reference to the player controller input from the Unity Engine

    void Awake()
    {
        // Create the player controller object in the code, linking the various control operations to functions
        controls = new PlayerControls();
        // context refers to any input data when a controlpad button is pressed, providng input information which can be used
        controls.ControllerInput.RightThumbstick.performed += context => MoveUiJoystick(context.ReadValue<Vector2>());
        controls.ControllerInput.RudderLeftDown.performed += context => ControlsUtilityMethods.PedalDownKeyboard("left");// context cant be used to get input information
        controls.ControllerInput.RudderLeftUp.performed += context => ControlsUtilityMethods.PedalBothUp();
        controls.ControllerInput.RudderRightDown.performed += context => ControlsUtilityMethods.PedalDownKeyboard("right");
        controls.ControllerInput.RudderRightUp.performed += context => ControlsUtilityMethods.PedalBothUp();
        controls.ControllerInput.FlapsDown.performed += context => ControlsUtilityMethods.MoveFlapsDown();
        controls.ControllerInput.FlapsUp.performed += context => ControlsUtilityMethods.MoveFlapsUp();
        controls.ControllerInput.LeftThumbstick.performed += context => ControlsUtilityMethods.MoveUiThrottle(context.ReadValue<Vector2>());// context cant be used to get input information
    }
    // Start is called before the first frame update
    void Start()
    {
        controls.ControllerInput.Enable();  // Start with the gamepad controls enabled
        joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();  // Access the joystick object
    }

    // Move the UI Joystick based on input coords of gamepad right stick - Moves Elevators and Ailerons
    private void MoveUiJoystick(Vector2 context)
    {
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;  // used to keep z axis position the same in the 3D space

        Vector2 rightStickCoords = context;
        // UI Joystick follows the gamepad joystick each time its moved
        joystickHandle.transform.localPosition = new Vector3(rightStickCoords.x * JOYSTICK_MULTIPLIER, rightStickCoords.y * JOYSTICK_MULTIPLIER, joystickCurrentZ);
    }
}
