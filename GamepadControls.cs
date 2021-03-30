using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamepadControls : MonoBehaviour
{
    // Script maps controller joysticks to move the UI joystick, consequently deflecting the surfaces
    private const float joystickMultiplyer = 64f;  // Joystick input is 1 / -1. Need to make this 64 / -64 to map to UI joystick

    private RectTransform joystickHandle; // Get the joystick Handle object

    private PlayerControls controls;

    private ControlSurfaces.Rudder rudder;
    private Vector3 rudderStartingRotations;
    private const float rudderMultiplyer = 20f; // Rudder should move 20 degress + / -, so mutiply input of 1 / -1 by 20

    private Slider uiSlider;

    void Awake()
    {
        controls = new PlayerControls();
        controls.ControllerInput.RightThumbstick.performed += context => RightStickCoords(context.ReadValue<Vector2>());// context cant be used to get input information
        controls.ControllerInput.LeftThumbstick.performed += context => LeftStickCoords(context.ReadValue<Vector2>());// context cant be used to get input information
    }
    // Start is called before the first frame update
    void Start()
    {
        controls.ControllerInput.Enable();  // Start with the keyboard controls enabled
        joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();
        rudder = ControlSurfaces.rudder;
        rudderStartingRotations = rudder.GetStartingRotations();

        // Get the UI slider
        uiSlider = GameObject.FindObjectOfType<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        ControlsUtilityMethods.RotateSurfaces();  // Each frame we want to move elevators and ailerons based on joysticks location
    }

    // Move the UI Joystick based on input coords of gamepad right stick
    private void RightStickCoords(Vector2 context)
    {
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;

        Vector2 rightStickCoords = context;
        joystickHandle.transform.localPosition = new Vector3(rightStickCoords.x * joystickMultiplyer, rightStickCoords.y * joystickMultiplyer, joystickCurrentZ);
    }

    // Move the Rudder (x) and Throttle (y) based on input coords of gamepad left stick
    private void LeftStickCoords(Vector2 context)
    {
        Vector2 leftStickCoords = context;
        RotateRudder(leftStickCoords.x);
        MoveThrottle(leftStickCoords.y);
    }

    private void RotateRudder(float xAxisCoord)
    {
        Vector3 rudderCurrentRotations = rudder.GetCurrentRotations();
        Vector3 rudderRotation = new Vector3(rudderCurrentRotations.x, rudderCurrentRotations.y, -xAxisCoord * rudderMultiplyer);
        rudder.Rotate(rudderRotation);
    }

    private void MoveThrottle(float yAxisCoord)
    {
        //float coordValue = yAxisCoord + 2;  // add 2, making -1 (bottom result) become 1, and 1 (top result) become 3. Slider scale is 1-3
        //Debug.Log(coordValue);
        uiSlider.value = yAxisCoord + 2;
        Debug.Log(uiSlider.value);
    }
}
