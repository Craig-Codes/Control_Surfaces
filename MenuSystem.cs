// Script controls all Menu UI interactions across all control types (mouse, keyboard, touch, gamepad)
// Script is on all UI buttons to control which icon is displayed. Owing to this, static is used heavily
// to ensure there is only one reference to objects, not a new reference for each time the script is used
// This ensures variables (especially booleans) have one instance shared across all scripts

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuSystem : MonoBehaviour
{
    // Varaibles contain mouse pointer image references
    public Texture2D defaultPointer;
    public Texture2D controls;
    public Texture2D magnify;
    public Texture2D quit;
    public Texture2D info;
    public Texture2D reload;
    public Texture2D mouse;
    public Texture2D keyboard;

    private TextMeshProUGUI controlInputDescription;  // Variable contains reference to the text description box

    // UI menu system panels
    private RectTransform infoPanel;
    public static bool infoIsVisible;  // boolean used to see if infoPanel is visible. Static as script on many objects but we want one single truth

    private RectTransform controlsPanel;
    public static bool controlsIsVisible;

    private RectTransform mouseControlsText;
    private RectTransform keyboardControlsImage;
    private RectTransform gamepadControlsImage;

    private Vector3 uiPanelScale = new Vector3(1.34f, 0.94f, 1f);  // Scale UI Panels grow to when opened
    private Vector3 controlsInstructionsFullSize = new Vector3(1f, 1f, 1f);
    
    private CursorMode cursorMode = CursorMode.ForceSoftware;  // Allow mouse pointer to be software controlled for custom pointers
    private Vector2 offSetNone = Vector2.zero;
    private Vector2 customOffset = new Vector2(15, 8);   // Slight offset so that custom pointer icons line up correctly

    // Secondary view cameras setup
    // Variables control the smooth growth of the secondary cameras growing into main viewport when smaller icons are clicked
    private Vector3 scaleChange = new Vector3(0.05f, 0.05f, 0f); // Speed of growth per loop iteration
    private const float TOP_SCALE = 2.1f;  // Top scale varaible controls contains maximum size of secondary cameras

    private RawImage cockpitView;  // Reference to the cockpitView image
    static private bool cockpitIsFull = false;  // Boolean used as flag to determine size of cockpit view camera - static as script on many objects
    private RectTransform cockpitViewLarge;  // Reference to the large version of image which shows in main viewport on click

    private RawImage chaseView; // Reference to the chase / behind image
    static private bool chaseIsFull;
    private RectTransform chaseViewLarge;

    private static bool imageResize = false; // boolean value used as a flag for when we want an image to be able to resize

    private static Slider throttleSlider;  // Varaibles contain references to UI Sliders
    private static Slider flapSlider;

    private Button startSelectedButton;  // Used to start a button as selected so that keyboard users can scroll correctly
    // In this case, we start the INFO button as the selected button

    // References to the different control type menu buttons
    private static Button mouseButton;
    private static Button keyboardButton;
    private static Button gamepadButton;

    // Reference to the different control systems, allowing them to be used to select Menu items.
    private static PlayerControls playerControlsKeyboard;
    private static PlayerControls playerControlsGamepad;

    // Currently selected UI Button
    private static GameObject currentSelected;

    // Array of UI button gameobjects so that we can scroll through them.
    // All UI buttons have been tagged as 'UiButton' in Unity Engine
    // Non UI buttons used to stop the user from being able to select other UI objects using the
    // the menu scroll button - this avoids bugs where the user cannot see which button is selected
    private static List<Button> uiButtons = new List<Button>();
    private static List<Button> nonUiButtons = new List<Button>();  

    private static int menuCounter;  // varible keeps track of which menu item is currently highlighted

    private void Awake()
    {
        // Get access to the Player Controllers from the Unity Engine
        playerControlsKeyboard = new PlayerControls();
        playerControlsGamepad = new PlayerControls();
        // Functions called when buttons pressed on keyboard and gamepad
        playerControlsKeyboard.KeyboardInput.ButtonSelect.performed += context => OnButtonSelect();
        playerControlsGamepad.ControllerInput.SelectMenu.performed += context => OnButtonSelect();
        playerControlsGamepad.ControllerInput.MoveMenu.performed += context => MoveMenuSelection();
        playerControlsGamepad.ControllerInput.ChaseView.performed += context => CameraActions("chase");
        playerControlsGamepad.ControllerInput.CockpitView.performed += context => CameraActions("cockpit");

        // Access the controls buttons
        mouseButton = GameObject.Find("Mouse").GetComponent<Button>();
        keyboardButton = GameObject.Find("Keyboard").GetComponent<Button>();
        gamepadButton = GameObject.Find("Gamepad").GetComponent<Button>();

        // Access the sliders so that values can be reset on reset button click
        throttleSlider = GameObject.FindGameObjectWithTag("ThrottleSlider").GetComponent<Slider>();
        flapSlider = GameObject.FindGameObjectWithTag("FlapSlider").GetComponent<Slider>();
    }

    void Start()
    {
        // Enable keyboard and gamepad controls
        playerControlsKeyboard.KeyboardInput.Enable();  
        playerControlsGamepad.ControllerInput.Enable();

        // Access the Info Panels dimensions, setting it to start invisible and setting the boolean to reflect
        infoPanel = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<RectTransform>();  
        infoPanel.localScale = Vector3.zero;
        infoIsVisible = false;

        controlsPanel = GameObject.FindGameObjectWithTag("ControlsPanel").GetComponent<RectTransform>(); 
        controlsPanel.localScale = Vector3.zero;
        controlsIsVisible = false;

        startSelectedButton = GameObject.Find("Info_Button").GetComponent<Button>();  // Start Info button as selected
        startSelectedButton.Select();  // start off with the info button selected

        // Get the TextMeshPro elements through code - looping through them all until the correct tag is found
        var textArray = FindObjectsOfType<TextMeshProUGUI>();
        foreach(var element in textArray)
        {
            if(element.tag == "ControlsDescription")
            {
                // assign found element into variabe providing access to the controls description box
                controlInputDescription = element; 
            }   
        }

        // Access the images for each control type, allowing them to be toggled between
        mouseControlsText = GameObject.FindGameObjectWithTag("ControlsText").GetComponent<RectTransform>();
        keyboardControlsImage = GameObject.FindGameObjectWithTag("KeyboardControlsImage").GetComponent<RectTransform>(); 
        gamepadControlsImage = GameObject.FindGameObjectWithTag("GamepadControlsImage").GetComponent<RectTransform>();
        // Hide all images except mouse controls which is the shown to the user by default
        mouseControlsText.localScale = controlsInstructionsFullSize;  // Set mouse controls image to be full size
        gamepadControlsImage.localScale = Vector3.zero;
        keyboardControlsImage.localScale = Vector3.zero;

        cockpitView = GameObject.FindGameObjectWithTag("CockpitView").GetComponent<RawImage>();  // Access the image
        cockpitIsFull = false;  // Set bool to false, indicating that it is currently set to its normal small position
        cockpitViewLarge = GameObject.FindGameObjectWithTag("CockpitViewLarge").GetComponent<RectTransform>();

        chaseView = GameObject.FindGameObjectWithTag("BehindView").GetComponent<RawImage>();
        chaseIsFull = false;
        chaseViewLarge = GameObject.FindGameObjectWithTag("BehindViewLarge").GetComponent<RectTransform>();

        // Method controls which menu buttons are avalaible to th user
        ToggleUiButtons();  // Start extra control buttons as disabled so user cannot scroll to them to prevent bugs
    }

    // Method called each frame
    // Checks to see if either of the alternate camera buttons have been selected
    // If selected and already at full screen, minimise
    // If selected and not at full screen, make full screen
    private void Update()
    {
        // On each frame, if imageResize is true, then we want to make either the cockpit or chase image fullscreen
        // update method is used so that the image can be scaled up with a smooth animation slowly growing
        if (imageResize) {
            ResizeCameraImage(cockpitIsFull, cockpitViewLarge);
            ResizeCameraImage(chaseIsFull, chaseViewLarge);
        }
    }

    // Method resizes the input image based on the boolean value
    private void ResizeCameraImage(bool isFull, RectTransform largeView)
    {
        if (isFull)  // We want to expand the image until it is full
        {
            if (largeView.transform.localScale.x < TOP_SCALE) // Check to ensure it is at full size
            {
                imageResize = true;  // allow update method to resize over time
                largeView.transform.localScale += scaleChange;
            }
            else
            {
                imageResize = false;  // Stop the resizing through the update method
            }
        }
        else if (!isFull)  // We want to shrink the image back
        {
            if (largeView.transform.localScale.x > 0)
            {
                imageResize = true;
                largeView.transform.localScale -= scaleChange;
            }
            if (largeView.transform.localScale.x < 0)  // Clause put in to prevent bugs
            {
                largeView.transform.localScale = new Vector3(0f, 0f, 1f);  // Ensure scale doesnt become a minus, causing a mark on game canvas
                imageResize = false;  // resizing complete, stop update from running the resizer
            }
        }
    }

    // Method reverses the isFull boolean, triggering the camera image to grow or shrink
    public void CameraActions(string camera)
    {
        if (camera == "cockpit")
        {
            cockpitIsFull = !cockpitIsFull;  // Reverse the boolean
            chaseIsFull = false;  // Other camera cannot be full screen if another was clicked
            ResizeCameraImage(cockpitIsFull, cockpitViewLarge);
            OnMouseEnterAlternateCamera("cockpit");  // ensure mouse pointer is in the correct state after clicking
        }
        else if (camera == "chase")
        {
            chaseIsFull = !chaseIsFull;
            cockpitIsFull = false;
            ResizeCameraImage(chaseIsFull, chaseViewLarge);
            OnMouseEnterAlternateCamera("chase");
        }
    }

    // UI Icons - Mouse controls

    // Method shows a cross if the camera view is already on screen when the mouse goes over it
    public void OnMouseEnterAlternateCamera(string camera)
    {
        // If input parameter is chase, use the chaseIsFull bool, if not use cockpits bool
        bool isFull = camera == "chase" ? chaseIsFull : cockpitIsFull;

        if (!isFull)
        {
            Cursor.SetCursor(magnify, customOffset, cursorMode);
        }
        else
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
        }
    }

    // Method controls how cursor appears when over different buttons depending on their state
    public void OnMouseEnterButton(string button)
    {
        switch (button)
        {
            case "reset":
                Cursor.SetCursor(reload, customOffset, cursorMode);
                break;
            case "controls":
                if (controlsIsVisible)
                {
                    Cursor.SetCursor(quit, customOffset, cursorMode);
                }
                else
                {
                    Cursor.SetCursor(controls, customOffset, cursorMode);
                }
                break;
            case "info":
                if (infoIsVisible)
                {
                    Cursor.SetCursor(quit, customOffset, cursorMode);
                }
                else
                {
                    Cursor.SetCursor(info, customOffset, cursorMode);
                }
                break;
            case "flaps":
                if (throttleSlider.value <= 1)
                {
                    Cursor.SetCursor(defaultPointer, customOffset, cursorMode);
                }
                else
                {
                    Cursor.SetCursor(quit, customOffset, cursorMode);
                }
                break;
            case "keyboard":
                Cursor.SetCursor(keyboard, customOffset, cursorMode);
                break;
            case "gamepad":
                Cursor.SetCursor(controls, customOffset, cursorMode);
                break;
            case "mouse":
                Cursor.SetCursor(mouse, customOffset, cursorMode);
                break;
            default:
                Cursor.SetCursor(defaultPointer, customOffset, cursorMode);
                break;
        }
    }

    // Method sets cursor back to default when mouse exits a button
    public void OnMouseExit()
    {
        Cursor.SetCursor(default, offSetNone, cursorMode);
    }


    // Method shows or hides the info menu panel on click
    public void OnMouseClickInfo()
    {
        controlsPanel.localScale = Vector3.zero;  // close the controls panel if its open
        controlsIsVisible = false;  // change the controls bolean
        ToggleUiButtons();  // Remove the additional controls buttons (keyboard, mouse, gamepad)

        if (infoIsVisible)  // If info panel is visible we want to close it
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
            infoPanel.localScale = Vector3.zero;  
            Cursor.SetCursor(info, customOffset, cursorMode);
            infoIsVisible = false;
        }
        else  // If no visible, we want to open the info panel
        {
            Cursor.SetCursor(info, customOffset, cursorMode);
            infoPanel.localScale = uiPanelScale; 
            Cursor.SetCursor(quit, customOffset, cursorMode);
            infoIsVisible = true;
        }
        ShowHideControlSurfaceDescriptions(); // Show or hide the description box depending on panels state
    }

    // Method shows or hides the info control panel on click
    // Ensures extra control buttons are selectable or hidden to prevent bugs
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
            ToggleUiButtons();  // Remove the extra controls menu buttons
        }
        else
        {
            Cursor.SetCursor(controls, customOffset, cursorMode);
            controlsPanel.localScale = uiPanelScale; // show panel
            Cursor.SetCursor(quit, customOffset, cursorMode);
            controlsIsVisible = true;
            ToggleUiButtons();  // show the extra controls menu buttons
        }
        ShowHideControlSurfaceDescriptions();
    }

    // Method controls what happens when controller type buttons are selected
    // Whichever control type is selected, the correspondng image is displayed
    public void OnMouseClickControlType(string control)
    {
        mouseControlsText.localScale = Vector3.zero;  // Set all controls images 
        keyboardControlsImage.localScale = Vector3.zero;
        gamepadControlsImage.localScale = Vector3.zero;

        switch (control)  // Only show the image for the selected control type
        {
            case "mouse":
                mouseControlsText.localScale = controlsInstructionsFullSize;
                break;
            case "keyboard":
                keyboardControlsImage.localScale = controlsInstructionsFullSize;
                break;
            case "gamepad":
                gamepadControlsImage.localScale = controlsInstructionsFullSize;
                break;
        }
    }

    // Method resets the entire scene, including all control surfaces, all UI elements and the Menu system
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

        AircraftMovement.aircraft.ResetAircraft();  // Move aircraft back to starting position

        // Reset UI elements
        infoIsVisible = false;
        cockpitIsFull = false;
        ResizeCameraImage(cockpitIsFull, cockpitViewLarge);
        chaseIsFull = false;
        ResizeCameraImage(chaseIsFull, chaseViewLarge);
        controlsIsVisible = false;
        ToggleUiButtons();

        infoPanel.localScale = Vector3.zero;  // hide panels
        controlsPanel.localScale = Vector3.zero;  

        throttleSlider.value = 2f;  // Reset the airspeed needle
        flapSlider.value = 0;  // Reset the flaps slider

        ToggleUiButtons(); // reset buttons so that correct ones are hidden
    }

    // Method controls if the control description box is shown
    // The text box is hidden when the menu is displayed to prevent overlap and cluttering
    public void ShowHideControlSurfaceDescriptions()
    {
        // hide the control surface description text if either the info or control pannels are open
        if (infoIsVisible || controlsIsVisible)
        {
            controlInputDescription.alpha = 0;  // Use alpha property to make box invisible
        }
        else
        {
            controlInputDescription.alpha = 1;
        }
    }

    //Method adds or removes buttons from the UI button array to make them selectable
    private void ToggleUiButtons()
    {
        uiButtons.Clear();  // Empty the current button lists
        nonUiButtons.Clear();

        // Always include these buttons, adding them to the list
        var info = GameObject.Find("Info_Button").GetComponent<Button>();
        var controls = GameObject.Find("Controls_Button").GetComponent<Button>();
        var reset = GameObject.Find("Reset_Button").GetComponent<Button>();
        uiButtons.Add(info);
        uiButtons.Add(controls);
        uiButtons.Add(reset);

        // These buttons are added to one of the lists based on the IF statement
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

    // Method controls which methods trigger based on keyboard on controlpad clicks
    private void OnButtonSelect()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;  // Get the selected button name

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
                OnMouseClickControlType("mouse");
                break;
            case "Keyboard":
                OnMouseClickControlType("keyboard");
                break;
            case "Gamepad":
                OnMouseClickControlType("gamepad");
                break;
            case "CockpitImage":
                CameraActions("cockpit");
                break;
            case "BackImage":
                CameraActions("chase");
                break;
            default:
                break;
        }
    }

    // Method allows a single gamepad button to scroll through the selectable buttons
    private void MoveMenuSelection()
    {
        ToggleUiButtons(); // ensure the UI button list is currently correct
        menuCounter++;  // add one to menuCounter
        // if counter is higher than current avalaible list entries, reset counter number to avoid errors
        if (menuCounter >= uiButtons.Count)  
        {
            menuCounter = 0;
        }
        // Get the next UI element in the list, then make it selected
        uiButtons[menuCounter].Select();
    }
}
