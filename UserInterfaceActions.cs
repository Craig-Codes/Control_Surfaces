using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

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

    private Color32 white = new Color32(255, 255, 255, 255); // Default colour over images - clear white
    private Color32 grey = new Color32(100, 100, 100, 255);  // Grey tint over images, used when selected

    private RawImage cockpitView;  // Access the image
    static private bool cockpitIsFull = false;  // Have more than one of this script, need one true source!!! Shared by all instances of the class
    private Vector3 cockpitPositionChange;  // Position change per tick on update
    private RectTransform cockpitViewLarge;

    private RawImage chaseView;
    static private bool chaseIsFull;
    private Vector3 chasePositionChange;
    private RectTransform chaseViewLarge;

    private static bool imageResize = false; // boolean value used as a flag for when we want an image to be able to resize


    void Start()
    {
        infoPanel = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<RectTransform>();  // Get the Info Panel
        infoPanel.localScale = Vector3.zero;
        infoIsVisible = false;

        controlsPanel = GameObject.FindGameObjectWithTag("ControlsPanel").GetComponent<RectTransform>();  // Get the controls Panel
        controlsPanel.localScale = Vector3.zero;
        controlsIsVisible = false;


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
        if (!chaseIsFull)  // check to ensure other camera isnt in large mode
        {
            cockpitIsFull = !cockpitIsFull;
        }
        OnMouseEnterCockpitImage();  // ensure mouse pointer is in the correct state after clicking
    }

    public void ChaseActions()
    {
        if (!cockpitIsFull)
        {
            chaseIsFull = !chaseIsFull;
        }
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
                cockpitView.color = grey;
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
                cockpitView.color = white;
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
                chaseView.color = grey;
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
                chaseView.color = white;
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
        if (!chaseIsFull && !cockpitIsFull)
        {
            Cursor.SetCursor(magnify, customOffset, cursorMode);
        }
        else if (chaseIsFull)
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
        }
        else
        {
            Cursor.SetCursor(default, offSetNone, cursorMode);
        }
        
    }

    public void OnMouseEnterCockpitImage()
    {
        if (!chaseIsFull && !cockpitIsFull)
        {
            Cursor.SetCursor(magnify, customOffset, cursorMode);
        }
        else if (cockpitIsFull)
        {
            Cursor.SetCursor(quit, customOffset, cursorMode);
        }
        else
        {
            Cursor.SetCursor(default, offSetNone, cursorMode);
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
        }
        else
        {
            Cursor.SetCursor(controls, customOffset, cursorMode);
            controlsPanel.localScale = uiPanelScale; // show panel
            Cursor.SetCursor(quit, customOffset, cursorMode);
            controlsIsVisible = true;
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

        infoPanel.localScale = Vector3.zero;
        controlsPanel.localScale = Vector3.zero;  // hide panel

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
}
