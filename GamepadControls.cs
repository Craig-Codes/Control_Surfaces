// Script controls gamepad interactions for the UI throttle, flaps, peddals and Joystick

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GamepadControls : MonoBehaviour
{
    // Script maps controller joysticks to move the UI joystick, consequently deflecting the surfaces
    private const float joystickMultiplyer = 64f;  // Joystick input is 1 / -1. Need to make this 64 / -64 to map to UI joystick

    private RectTransform joystickHandle; // Get the joystick Handle object

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.ControllerInput.RightThumbstick.performed += context => MoveUiJoystick(context.ReadValue<Vector2>());// context cant be used to get input information
        controls.ControllerInput.RudderLeftDown.performed += context => ControlsUtilityMethods.LeftPedalDownKeyboard();// context cant be used to get input information
        controls.ControllerInput.RudderLeftUp.performed += context => ControlsUtilityMethods.LeftPedalUpKeyboard();
        controls.ControllerInput.RudderRightDown.performed += context => ControlsUtilityMethods.RightPedalDownKeyboard();
        controls.ControllerInput.RudderRightUp.performed += context => ControlsUtilityMethods.RightPedalUpKeyboard();
        controls.ControllerInput.FlapsDown.performed += context => ControlsUtilityMethods.MoveFlapsDown();
        controls.ControllerInput.FlapsUp.performed += context => ControlsUtilityMethods.MoveFlapsUp();
        controls.ControllerInput.LeftThumbstick.performed += context => ControlsUtilityMethods.MoveUiThrottle(context.ReadValue<Vector2>());// context cant be used to get input information
    }
    // Start is called before the first frame update
    void Start()
    {
        controls.ControllerInput.Enable();  // Start with the keyboard controls enabled
        joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        ControlsUtilityMethods.RotateSurfaces();  // Each frame we want to move elevators and ailerons based on joysticks location
        // Static class ControlsUtilityMethods cannot have an update method, so we do this here
    }

    // Move the UI Joystick based on input coords of gamepad right stick - Moves Elevators and Ailerons
    private void MoveUiJoystick(Vector2 context)
    {
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;

        Vector2 rightStickCoords = context;
        joystickHandle.transform.localPosition = new Vector3(rightStickCoords.x * joystickMultiplyer, rightStickCoords.y * joystickMultiplyer, joystickCurrentZ);
    }
}
