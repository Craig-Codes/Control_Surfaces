using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;


// Script deals with the Menu system
public class UserInterfaceActions : MonoBehaviour
{
    // Mouse pointer images
    public Texture2D defaultPointer;
    public Texture2D controls;
    public Texture2D magnify;
    public Texture2D quit;
    public Texture2D info;
    public Texture2D reload;
    public Texture2D mouse;
    public Texture2D keyboard;

    private TextMeshProUGUI controlInputDescription;

    // UI menu system
    private RectTransform infoPanel;
    public static bool infoIsVisible;  // boolean used to see if infoPanel is visible. Static as script on many objects but we want one single truth

    private RectTransform controlsPanel;
    public static bool controlsIsVisible;

    private RectTransform mouseControlsText;
    private RectTransform keyboardControlsImage;
    private RectTransform gamepadControlsImage;

    private Vector3 uiPanelScale = new Vector3(1.34f, 0.94f, 1f);  // Scale UI Panels grow to when opened
    private Vector3 controlsInstructionsScale = new Vector3(1f, 1f, 1f);
    
    private CursorMode cursorMode = CursorMode.ForceSoftware;
    private Vector2 offSetNone = Vector2.zero;
    private Vector2 customOffset = new Vector2(15, 8);

    // Popup secondary cameras
    private Vector3 scaleChange;
    private float topScale = 2.1f;

    private RawImage cockpitView;  // Access the image
    static private bool cockpitIsFull = false;  // Have more than one of this script, need one true source!!! Shared by all instances of the class
    private Vector3 cockpitPositionChange;  // Position change per tick on update
    private RectTransform cockpitViewLarge;

    private RawImage chaseView;
    static private bool chaseIsFull;
    private Vector3 chasePositionChange;
    private RectTransform chaseViewLarge;

    private static bool imageResize = false; // boolean value used as a flag for when we want an image to be able to resize

    public Slider throttleSlider;
    public Slider flapSlider;

    private Button startSelectedButton;  // Used to start a button selected, so that keyboard users can scroll correctly

    private static Button mouseButton;
    private static Button keyboardButton;
    private static Button gamepadButton;

    private static PlayerControls controlsSchema;
    private static PlayerControls gamepadSchema;

    // Currently selected UI Button
    private static GameObject currentSelected;

    // Array of UI button gameobjects so that we can scroll through them. All UI buttons have been tagged as 'UiButton' in Unity Engine
    private static List<Button> uiButtons = new List<Button>();
    private static List<Button> nonUiButtons = new List<Button>();

    private static int menuCounter;

    private void Awake()
    {
        // Select UI Button
        controlsSchema = new PlayerControls();
        gamepadSchema = new PlayerControls();
        controlsSchema.KeyboardInput.ButtonSelect.performed += context => OnButtonSelect();
        gamepadSchema.ControllerInput.SelectMenu.performed += context => OnButtonSelect();
        gamepadSchema.ControllerInput.MoveMenu.performed += context => MoveMenuSelection();
        gamepadSchema.ControllerInput.ChaseView.performed += context => ChaseSelect();
        gamepadSchema.ControllerInput.CockpitView.performed += context => CockpitSelect();

        mouseButton = GameObject.Find("Mouse").GetComponent<Button>();
        keyboardButton = GameObject.Find("Keyboard").GetComponent<Button>();
        gamepadButton = GameObject.Find("Gamepad").GetComponent<Button>();
    }

    void Start()
    {
        controlsSchema.KeyboardInput.Enable();  // Start with the keyboard controls enabled
        gamepadSchema.ControllerInput.Enable();

        infoPanel = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<RectTransform>();  // Get the Info Panel
        infoPanel.localScale = Vector3.zero;
        infoIsVisible = false;

        controlsPanel = GameObject.FindGameObjectWithTag("ControlsPanel").GetComponent<RectTransform>();  // Get the controls Panel
        controlsPanel.localScale = Vector3.zero;
        controlsIsVisible = false;

        startSelectedButton = GameObject.Find("Info_Button").GetComponent<Button>();

        startSelectedButton.Select();  // start off with the info button selected

        // Get the TextMeshPro via code
        var textArray = FindObjectsOfType<TextMeshProUGUI>();
        foreach(var element in textArray)
        {
            if(element.tag == "ControlsDescription")
            {
                controlInputDescription = element;
            }   
        }

        mouseControlsText = GameObject.FindGameObjectWithTag("ControlsText").GetComponent<RectTransform>();
        keyboardControlsImage = GameObject.FindGameObjectWithTag("KeyboardControlsImage").GetComponent<RectTransform>(); 
        gamepadControlsImage = GameObject.FindGameObjectWithTag("GamepadControlsImage").GetComponent<RectTransform>();
        mouseControlsText.localScale = controlsInstructionsScale;  // Set all controls images / text to be hidden at the start except for mouse instructions
        keyboardControlsImage.localScale = Vector3.zero;
        gamepadControlsImage.localScale = Vector3.zero;

        scaleChange = new Vector3(0.05f, 0.05f, 0f);  // speed the camera images scale up to size

        cockpitView = GameObject.FindGameObjectWithTag("CockpitView").GetComponent<RawImage>();  // Access the image
        cockpitIsFull = false;  // Set bool to false, indicating that it is currently set to its normal small position
        cockpitViewLarge = GameObject.FindGameObjectWithTag("CockpitViewLarge").GetComponent<RectTransform>();

        chaseView = GameObject.FindGameObjectWithTag("BehindView").GetComponent<RawImage>();
        chaseIsFull = false;
        chaseViewLarge = GameObject.FindGameObjectWithTag("BehindViewLarge").GetComponent<RectTransform>();

        ToggleUiButtons();  // Loop through list and start extra control buttons as disabled so user cannot scroll to them
    }

    private void Update()
    {
        if (imageResize) {
            CockpitPositionChange();
            ChasePositionChange();
        }
    }

    // Reverse the boolean values on click
    public void CockpitActions()
    {
        //if (!chaseIsFull)  // check to ensure other camera isnt in large mode
        //{
            cockpitIsFull = !cockpitIsFull;
        chaseIsFull = false;
        //}
        OnMouseEnterCockpitImage();  // ensure mouse pointer is in the correct state after clicking
    }

    public void ChaseActions()
    {
        //if (!cockpitIsFull)
        //{
            chaseIsFull = !chaseIsFull;
        cockpitIsFull = false;
        //}
        OnMouseEnterChaseImage();


    }

    public void CockpitPositionChange()
    {
        if (cockpitIsFull)  // We want to expand the image
        {
            if (cockpitViewLarge.transform.localScale.x < topScale)
            {
                imageResize = true;  // allow update method to resize over time
                cockpitViewLarge.transform.localScale += scaleChange;
            }
            else
            {
                imageResize = false;
            }
        }
        else if (!cockpitIsFull)  // We want to shrink the image back
        {
            if (cockpitViewLarge.transform.localScale.x > 0)
            {
                imageResize = true;  // allow update method to resize over time
                cockpitViewLarge.transform.localScale -= scaleChange;
            }
            if(cockpitViewLarge.transform.localScale.x < 0)
            {
                cockpitViewLarge.transform.localScale = new Vector3(0f,0f,1f);  // Ensure scale doesnt become a minus, causing a mark on game canvas
                imageResize = false;  // resizing complete, stop update from running the resizer
            }
        }
    }

    public void ChasePositionChange()
    {
        if (chaseIsFull)  // We want to expand the image
        {
            if (chaseViewLarge.transform.localScale.x < topScale)
            {
                imageResize = true;  // allow update method to resize over time
                chaseViewLarge.transform.localScale += scaleChange;
            }
            else
            {
                imageResize = false;
            }
        }
        else if (!chaseIsFull)  // We want to shrink the image back
        {
            if (chaseViewLarge.transform.localScale.x > 0)
            {
                imageResize = true;  // allow update method to resize over time
                chaseViewLarge.transform.localScale -= scaleChange;
            }
            else if (chaseViewLarge.transform.localScale.x < 0)
            {
                chaseViewLarge.transform.localScale = new Vector3(0f, 0f, 1f);  // Ensure scale doesnt become a minus, causing a mark on game canvas
                imageResize = false;  // resize has finished, stop update method from resizing
            }
        }
    }

    // UI Icons

   public void OnMouseEnterChaseImage()
    {
        if (!chaseIsFull)
        {
            Cursor.SetCursor(magnify, customOffset, cursorMode);
        }
        else
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
        }
        
    }

    public void OnMouseEnterCockpitImage()
    {
        if (!cockpitIsFull)
        {
            Cursor.SetCursor(magnify, customOffset, cursorMode);
        }
        else
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
        }
  

    }

    public void OnMouseEnterReset()
    {
        Cursor.SetCursor(reload, customOffset, cursorMode);
    }

    public void OnMouseEnterControls()
    {
        if (controlsIsVisible)
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
        }
        else
        {
            Cursor.SetCursor(controls, customOffset, cursorMode);
        }
    }

    public void OnMouseEnterInfo()
    {
        if (infoIsVisible)
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
        }
        else
        {
            Cursor.SetCursor(info, customOffset, cursorMode);
        }

    }

    public void OnMouseEnterFlap()
    {
        if (throttleSlider.value <= 1)
        {
            Cursor.SetCursor(defaultPointer, customOffset, cursorMode);
        }
        else
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
        }
    }

    public void OnMouseEnterGeneric()
    {
        Cursor.SetCursor(defaultPointer, customOffset, cursorMode);
    }


    public void OnMouseExit()
    {
        Cursor.SetCursor(default, offSetNone, cursorMode);
    }


    public void OnMouseClickInfo()
    {
        controlsPanel.localScale = Vector3.zero;  // close the controls panel if its open
        controlsIsVisible = false;
        ToggleUiButtons();

        if (infoIsVisible)
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
            infoPanel.localScale = Vector3.zero;  // hide panel
            Cursor.SetCursor(info, customOffset, cursorMode);
            infoIsVisible = false;
        }
        else
        {
            Cursor.SetCursor(info, customOffset, cursorMode);
            infoPanel.localScale = uiPanelScale; // show panel
            Cursor.SetCursor(quit, customOffset, cursorMode);
            infoIsVisible = true;
        }
        ShowHideControlSurfaceDescriptions();
    }

    public void OnMouseClickControls()
    {
        infoPanel.localScale = Vector3.zero;  // close the info panel if its open
        infoIsVisible = false;

        if (controlsIsVisible)
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
            controlsPanel.localScale = Vector3.zero;  // hide panel
            Cursor.SetCursor(controls, customOffset, cursorMode);
            controlsIsVisible = false;
            ToggleUiButtons();
        }
        else
        {
            Cursor.SetCursor(controls, customOffset, cursorMode);
            controlsPanel.localScale = uiPanelScale; // show panel
            Cursor.SetCursor(quit, customOffset, cursorMode);
            controlsIsVisible = true;
            ToggleUiButtons();
        }
        ShowHideControlSurfaceDescriptions();
    }

    public void OnMouseEnterGamepad()
    {
            Cursor.SetCursor(controls, customOffset, cursorMode);
    }

    public void OnMouseClickGamepad()
    {
        mouseControlsText.localScale = Vector3.zero;  // Set all controls images / text to be hidden at the start
        keyboardControlsImage.localScale = Vector3.zero;
        gamepadControlsImage.localScale = controlsInstructionsScale;
    }

    public void OnMouseClickMouse()
    {
        mouseControlsText.localScale = controlsInstructionsScale;  // Set all controls images / text to be hidden at the start
        keyboardControlsImage.localScale = Vector3.zero;
        gamepadControlsImage.localScale = Vector3.zero;
    }

    public void OnMouseClickKeyboard()
    {
        mouseControlsText.localScale = Vector3.zero;  // Set all controls images / text to be hidden at the start
        keyboardControlsImage.localScale = controlsInstructionsScale;
        gamepadControlsImage.localScale = Vector3.zero;
    }

    public void OnMouseEnterMouse()
    {
        Cursor.SetCursor(mouse, customOffset, cursorMode);
    }

    public void OnMouseEnterKeyboard()
    {
        Cursor.SetCursor(keyboard, customOffset, cursorMode);
    }

    public void OnMouseClickReset()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Resets the current scene
 
        // Rotate control surfaces back to starting locations
        ControlSurfaces.rudder.Rotate(ControlSurfaces.rudder.GetStartingRotations());
        ControlSurfaces.rightAileron.Rotate(ControlSurfaces.rightAileron.GetStartingRotations());
        ControlSurfaces.leftAileron.Rotate(ControlSurfaces.leftAileron.GetStartingRotations());
        ControlSurfaces.rightElevator.Rotate(ControlSurfaces.rightElevator.GetStartingRotations());
        ControlSurfaces.leftElevator.Rotate(ControlSurfaces.leftElevator.GetStartingRotations());

        // Get the joystick and move it back to the center
        var joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();
        joystickHandle.transform.localPosition = new Vector3(0, 0, 0);

        // Move aircraft back to starting position
        AircraftMovement.aircraft.ResetAircraft();

        // Reset UI elements
        infoIsVisible = false;
        cockpitIsFull = false;
        CockpitPositionChange();
        chaseIsFull = false;
        ChasePositionChange();
        controlsIsVisible = false;
        ToggleUiButtons();

        infoPanel.localScale = Vector3.zero;
        controlsPanel.localScale = Vector3.zero;  // hide panel

        // Reset the airspeed needle
        throttleSlider.value = 2f;

        // Reset the flaps slider
        flapSlider.value = 0;

        ToggleUiButtons(); // reset buttons so that correct ones are hidden
    }

    public void ShowHideControlSurfaceDescriptions()
    {
        // hide the control surface description text if info or control pannels are open
        if (infoIsVisible || controlsIsVisible)
        {
            controlInputDescription.alpha = 0;
        }
        else
        {
            controlInputDescription.alpha = 1;
        }

    }

    // Go thorugh each button in the list and allow it to be interactable / not interactable
    private void ToggleUiButtons()
    {
        // If items are to be shown, change bool to true, else make it false. Other scripts use this bool to trigger events.
        uiButtons.Clear();  // Empty the current button list#
        nonUiButtons.Clear();

        // Always include these buttons
        var info = GameObject.Find("Info_Button").GetComponent<Button>();
        var controls = GameObject.Find("Controls_Button").GetComponent<Button>();
        var reset = GameObject.Find("Reset_Button").GetComponent<Button>();
        // Always add in the primary buttons
        uiButtons.Add(info);
        uiButtons.Add(controls);
        uiButtons.Add(reset);

        var mouse = GameObject.Find("Mouse").GetComponent<Button>();
        var keyboard = GameObject.Find("Keyboard").GetComponent<Button>();
        var gamepad = GameObject.Find("Gamepad").GetComponent<Button>();

        if (controlsIsVisible)
        {
            // Only add in the three buttons if we want the full list of buttons - whilst control button panel is open
            uiButtons.Add(mouse);
            uiButtons.Add(keyboard);
            uiButtons.Add(gamepad);
        }
        else
        {
            nonUiButtons.Add(mouse);
            nonUiButtons.Add(keyboard);
            nonUiButtons.Add(gamepad);
        }

        // Make all buttons in uiButton list interactable
        foreach (Button button in uiButtons)
        {
            button.interactable = true;
        }

        // Make all buttons in nonUiButton list non-interactable
        foreach(Button button in nonUiButtons)
        {
            button.interactable = false;
        }
    }

    // Button Selection Switch statment to control what happens when a UI Button is selected with the keyboard
    private void OnButtonSelect()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;

        switch (currentSelected.name)
        {
            case "Reset_Button":
                OnMouseClickReset();
                break;
            case "Info_Button":
                OnMouseClickInfo();
                break;
            case "Controls_Button":
                OnMouseClickControls();
                break;
            case "Mouse":
                OnMouseClickMouse();
                break;
            case "Keyboard":
                OnMouseClickKeyboard();
                break;
            case "Gamepad":
                OnMouseClickGamepad();
                break;
            case "CockpitImage":
                CockpitSelect();
                break;
            case "BackImage":
                ChaseSelect();
                break;
            default:
                break;
        }
    }

    private void CockpitSelect()
    {
        CockpitActions();
        CockpitPositionChange();
    }

    private void ChaseSelect()
    {
        ChaseActions();
        ChasePositionChange();
    }

    private void MoveMenuSelection()
    {
        ToggleUiButtons(); // ensure the UI button list is currently correct
        menuCounter++;  // add one to menuCounter
        if (menuCounter >= uiButtons.Count)  // if counter is higher than current avalaible list entries, reset counter number to avoid errors
        {
            menuCounter = 0;
        }
        // Get the next UI element in the list, then make it selected
        uiButtons[menuCounter].Select();
    }
}
