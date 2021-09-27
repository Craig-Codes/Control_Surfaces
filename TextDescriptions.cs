// Script provides the caption box at the bottom of the UI allowing trainees to read
// which control surfaces are being defelected based on which controls are being used

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextDescriptions : MonoBehaviour
{
    // Reference to the Text box and its background, used to display the current aircraft state
    private TextMeshProUGUI controlSurfaceDescriptions;
    private Image controlInputDescriptionBackground;

    // Variables from MenuSystem.cs script - provide access to static properties
    // Booleans used to determine if any other UI is displayed over the top of the text descriptions
    // allowing the text discription to be hidden
    private bool infoIsVisible;  
    private bool controlsIsVisible;

    // Reference to control surface objects 
    // Variables used to read the state / deflection of each control surface
    ControlSurfaces.Rudder rudder;
    ControlSurfaces.Surface leftAileron;
    ControlSurfaces.Surface leftElevator;

    // Control surface capture vars, store references to the current positions of each surface
    // Rudder
    private float rudderZAngle;
    private string rudderString;  // variable stores the current text description of the rudder surface position
    // Ailerons
    private float aileronYAngle;
    private string aileronString;  
    // Elevators
    private float elevatorYAngle;
    private string elevatorString;  

    // Start is called before the first frame update
    void Start()
    {
        // Get references to the control surface objects
        rudder = ControlSurfaces.rudder;
        leftAileron = ControlSurfaces.leftAileron;
        leftElevator = ControlSurfaces.leftElevator;

        // Get the relevant start angles for each surface
        rudderZAngle = ControlsUtilityMethods.WrapAngle(rudder.GetCurrentRotations().z);
        aileronYAngle = ControlsUtilityMethods.WrapAngle(leftAileron.GetCurrentRotations().y);
        elevatorYAngle = ControlsUtilityMethods.WrapAngle(leftElevator.GetCurrentRotations().y);

        // Get the TextMeshPro via code. This text area is where description text is shown to the user
        var textArray = FindObjectsOfType<TextMeshProUGUI>();
        foreach (var element in textArray)  // loop through all text mesh pro objects
        {
            if (element.tag == "ControlsDescription")
            {   // If the object is tagged as "ControlsDescription" the reference is then stored in the variable
                // allowing us to print directly into the text box through code
                controlSurfaceDescriptions = element;
            }
        }

        // Get the UI text box background, enabling us to show ans hide it when required
        controlInputDescriptionBackground = GameObject.Find("ControlsDescriptionBackground").GetComponent<Image>();

        // Store initial values of MenuSystem script booleans
        infoIsVisible = MenuSystem.infoIsVisible;
        controlsIsVisible = MenuSystem.controlsIsVisible;
    }

    // Update is called once per frame
    // Each frame the update method is used to determin the current position of each
    // control surface, updating the text box with the correct user information
    void Update()
    {
        // Rudder 
        rudderZAngle = ControlsUtilityMethods.WrapAngle(rudder.GetCurrentRotations().z);  // update the position value
        GenerateTextDescription("rudder", rudderZAngle);  // Generate a text string for the control surface based on position
        // Ailerons
        aileronYAngle = ControlsUtilityMethods.WrapAngle(leftAileron.GetCurrentRotations().y);
        GenerateTextDescription("ailerons", aileronYAngle);
        // Elevators
        elevatorYAngle = ControlsUtilityMethods.WrapAngle(leftElevator.GetCurrentRotations().y);
        GenerateTextDescription("elevators", elevatorYAngle);

        TextOutput();  // Method is called to check if another UI component is covering the text box     
    }

    // Method controls when text is displayed on the screen
    private void TextOutput()
    {
        // Check to see if any other UI element which covers up the text box is visible
        infoIsVisible = MenuSystem.infoIsVisible;
        controlsIsVisible = MenuSystem.controlsIsVisible;
        if (controlsIsVisible || infoIsVisible)  // If other UI is showing, hide the text description box
        {
            controlSurfaceDescriptions.alpha = 0;  // Make text invisible
            controlInputDescriptionBackground.enabled = false;  // disable background box
        }
        // Check to see if a control surface is deflecting
        // Text is hidden if no surfaces are deflected. If any are deflected, show text so user 
        // gets a text description of how the surfaces have moved
        else if (SurfaceIsDeflecting()) {  // Method returns a boolean value
            // Use string interpolation to input the content stored in each control surfaces string variable
            controlSurfaceDescriptions.text =
             $"{elevatorString}\n" +
             $"{aileronString}\n" +
             $"{rudderString}\n";

            // show the text and background
            controlSurfaceDescriptions.alpha = 1;
            controlInputDescriptionBackground.enabled = true;
        }
        else
        {
            // Hide text and background if no deflection
            controlSurfaceDescriptions.alpha = 0;
            controlInputDescriptionBackground.enabled = false;
        }
    }

    // Method detects if any of the surfaces are currently deflecting
    private bool SurfaceIsDeflecting()
    {
        // If angles are between -1 and 1, then surface is not deflecting
        if (rudderZAngle > -1 && rudderZAngle < 1 && elevatorYAngle > -1 && elevatorYAngle < 1 && aileronYAngle > -1 && aileronYAngle < 1)
        {
            return false;
        }
        else  // Any other values outside -1/1 mean one of the surfaces is deflecting
        {
            return true;
        }   
    }

    // Method generates the text string for each control surface based on its current rotation value
    private void GenerateTextDescription(string controlSurface, float currentPosition)
    {
        switch (controlSurface)
        {
            case "rudder":
                if (currentPosition > -1 && currentPosition < 1)
                {
                    rudderString = "Rudder in neutral position - No Yaw";
                }
                else if (currentPosition > 1)
                {
                    rudderString = "Rudder deflecting to the Right - Yaw Right";
                }
                else
                {
                    rudderString = "Rudder deflecting to the left - Yaw Left";
                }
                break;

            case "ailerons":
                if (currentPosition > -1 && currentPosition < 1)
                {
                    aileronString = "Ailerons in neutral position - No Roll";
                }
                else if (currentPosition > 1)
                {
                    aileronString = "Left Aileron down, right Aileron up - Roll Right";
                }
                else
                {
                    aileronString = "Left Aileron up, right Aileron down - Roll Left";
                }
                break;
            case "elevators":
                if (currentPosition > -1 && currentPosition < 1)
                {
                    elevatorString = "Elevators in neutral position - No Nose Pitch";
                }
                else if (currentPosition > 1)
                {
                    elevatorString = "Elevators deflecting down - Nose Pitches Down";
                }
                else
                {
                    elevatorString = "Elevators deflecting up - Nose Pitches Up";
                }
                break;

            default:
                break;
        }
    }
}
