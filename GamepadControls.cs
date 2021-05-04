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

    // Array of UI button gameobjects so that we can scroll through them. All UI buttons have been tagged as 'UiButton' in Unity Engine
    private List<Button> uiButtons = new List<Button>();

    private int menuCounter;

    void Awake()
    {
        controls = new PlayerControls();
        controls.ControllerInput.RightThumbstick.performed += context => MoveUiJoystick(context.ReadValue<Vector2>());// context cant be used to get input information
        controls.ControllerInput.RudderLeftDown.performed += context => ControlsUtilityMethods.LeftPedalDownKeyboard();// context cant be used to get input information
        controls.ControllerInput.RudderLeftUp.performed += context => ControlsUtilityMethods.LeftPedalUpKeyboard();
        controls.ControllerInput.RudderRightDown.performed += context => ControlsUtilityMethods.RightPedalDownKeyboard();
        controls.ControllerInput.RudderRightUp.performed += context => ControlsUtilityMethods.RightPedalUpKeyboard();
        controls.ControllerInput.MoveMenu.performed += context => MoveMenuSelection();
        controls.ControllerInput.SelectMenu.performed += context => SelectMenuSelection();

    }
    // Start is called before the first frame update
    void Start()
    {
        controls.ControllerInput.Enable();  // Start with the keyboard controls enabled
        joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();

        // local references
        // Manually add in buttons so they are in the correct order!
        GetUiButtons();  // Get array of current avaliable UI Buttons for selection
        menuCounter = 0;  // Set the Ui Menu counter to 0.
    }

    // Update is called once per frame
    void Update()
    {
        ControlsUtilityMethods.RotateSurfaces();  // Each frame we want to move elevators and ailerons based on joysticks location
        GetUiButtons();
    }

    // Move the UI Joystick based on input coords of gamepad right stick - Moves Elevators and Ailerons
    private void MoveUiJoystick(Vector2 context)
    {
        float joystickCurrentZ = joystickHandle.transform.localPosition.z;

        Vector2 rightStickCoords = context;
        joystickHandle.transform.localPosition = new Vector3(rightStickCoords.x * joystickMultiplyer, rightStickCoords.y * joystickMultiplyer, joystickCurrentZ);
    }

    private void MoveMenuSelection(string direction)
    {
        if(direction == "left")
        {

        }
    }

    // Get the current UI buttons - this varies based on what Menu is currently on screen
    public void GetUiButtons()
    {
        uiButtons.Clear();  // Empty the current button list

        var info = GameObject.Find("Info_Button").GetComponent<Button>();
        var controls = GameObject.Find("Controls_Button").GetComponent<Button>();
        var reset = GameObject.Find("Reset_Button").GetComponent<Button>();

        // Always add these buttons
        uiButtons.Add(info);
        uiButtons.Add(controls);
        uiButtons.Add(reset);

        if (UserInterfaceActions.fullUiButtonList)
        {
            // Only add in the three buttons
            var mouse = GameObject.Find("Mouse").GetComponent<Button>();
            var keyboard = GameObject.Find("Keyboard").GetComponent<Button>();
            var gamepad = GameObject.Find("Gamepad").GetComponent<Button>();

            uiButtons.Add(mouse);
            uiButtons.Add(keyboard);
            uiButtons.Add(gamepad);
        }
    }

    private void MoveMenuSelection()
    {
        menuCounter++;  // add one to menuCounter
        if (menuCounter > uiButtons.Count - 1)  // if counter is higher than current avalaible list entries, reset counter number to avoid errors
        {
            menuCounter = 0;
        }
        // Get the next UI element in the list, then make it selected
        Debug.Log(uiButtons[menuCounter]);
        uiButtons[menuCounter].Select();
    }

    private void SelectMenuSelection()
    {

    }

}
