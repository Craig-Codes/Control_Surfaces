using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextDescriptions : MonoBehaviour
{

    private TextMeshProUGUI controlInputDescription;
    private Image controlInputDescriptionBackground;

    // variables from UI script
    private bool infoIsVisible;
    private bool controlsIsVisible;

    // Reference to control surface objects
    ControlSurfaces.Rudder rudder;
    ControlSurfaces.Surface leftAileron;
    ControlSurfaces.Surface leftElevator;

    // control surface capture vars
    // Rudder
    //private GameObject rudder;
    float rudderZAngle;
    float rudderPreviousValue;
    float rudderCurrentValue;
    string rudderString;  // variable stores the current direction of the rudder


    // Ailerons
    //private GameObject leftAileron;
    float aileronYAngle;
    float aileronPreviousValue;
    float aileronCurrentValue;
    string aileronString;  // variable stores the current direction of the rudder

    // Elevators
    //private GameObject leftElevator;
    float elevatorYAngle;
    float elevatorPreviousValue;
    float elevatorCurrentValue;
    string elevatorString;  // variable stores the current direction of the rudder

    // Start is called before the first frame update
    void Start()
    {
        // Get the control surfaces
        rudder = ControlSurfaces.rudder;
        leftAileron = ControlSurfaces.leftAileron;
        leftElevator = ControlSurfaces.leftElevator;

        // starting message
        rudderZAngle = ControlsUtilityMethods.WrapAngle(rudder.GetCurrentRotations().z);
        aileronYAngle = ControlsUtilityMethods.WrapAngle(leftAileron.GetCurrentRotations().y);
        elevatorYAngle = ControlsUtilityMethods.WrapAngle(leftElevator.GetCurrentRotations().y);

        // Get the TextMeshPro via code
        var textArray = FindObjectsOfType<TextMeshProUGUI>();
        foreach (var element in textArray)
        {
            if (element.tag == "ControlsDescription")
            {
                controlInputDescription = element;
            }
        }

        // Get the UI image background
        controlInputDescriptionBackground = GameObject.Find("ControlsDescriptionBackground").GetComponent<Image>();

        infoIsVisible = MenuSystem.infoIsVisible;
        controlsIsVisible = MenuSystem.controlsIsVisible;
    }

    // Update is called once per frame
    void Update()
    {
        // Rudder 
        rudderPreviousValue = rudderZAngle;  // Put current position value into variable
        rudderZAngle = ControlsUtilityMethods.WrapAngle(rudder.GetCurrentRotations().z);  // update the position value
        rudderCurrentValue = rudderZAngle;  // Put new position value into varaible
        GetRudderPositionChange(rudderPreviousValue, rudderCurrentValue);  // compare the values, building an output string based on results

        // Ailerons
        aileronPreviousValue = aileronYAngle;
        aileronYAngle = ControlsUtilityMethods.WrapAngle(leftAileron.GetCurrentRotations().y);
        aileronCurrentValue = aileronYAngle;
        GetAileronPositionChange(aileronPreviousValue, aileronCurrentValue);

        // Elevators
        elevatorPreviousValue = elevatorYAngle;
        elevatorYAngle = ControlsUtilityMethods.WrapAngle(leftElevator.GetCurrentRotations().y);
        elevatorCurrentValue = elevatorYAngle;
        GetElevatorPositionChange(elevatorPreviousValue, elevatorCurrentValue);

        TextOutput();
        
    }

    // Generate the string based on control surfaces movement
    private void TextOutput()
    {
        // If nothing else is visible on screen
        infoIsVisible = MenuSystem.infoIsVisible;
        controlsIsVisible = MenuSystem.controlsIsVisible;
        if (controlsIsVisible || infoIsVisible)
        {
            // Hide text and background if no deflection
            controlInputDescription.alpha = 0;
            controlInputDescriptionBackground.enabled = false;
        }
        // If a surface is deflecting, show text to explain what is going on to user
        else if (SurfaceIsDeflecting()) {
            controlInputDescription.text =
             $"{elevatorString}\n" +
             $"{aileronString}\n" +
             $"{rudderString}\n";

            // show the text and background
            controlInputDescription.alpha = 1;
            controlInputDescriptionBackground.enabled = true;
        }
        else
        {
            // Hide text and background if no deflection
            controlInputDescription.alpha = 0;
            controlInputDescriptionBackground.enabled = false;
        }
    }

    // Function detects if any of the surfaces are currently deflecting, returning a bool
    private bool SurfaceIsDeflecting()
    {
        // If angles are between -1 and 1, then surface is not reflecting
        if (rudderZAngle > -1 && rudderZAngle < 1 && elevatorYAngle > -1 && elevatorYAngle < 1 && aileronYAngle > -1 && aileronYAngle < 1)
        {
            return false;
        }
        else
        {
            return true;
        }   
    }

    private void GetRudderPositionChange(float start, float current)
    {
        if(current > -1 && current < 1)
        {
            rudderString = "The rudder is in the neutral position";
        }
        else if(current > 1)
        {
            rudderString = "The rudder is deflecting to the right";
        }
        else
        {
            rudderString = "The rudder is deflecting to the left";
        }
    }

    private void GetAileronPositionChange(float start, float current)
    {
        if (current > -1 && current < 1)
        {
            aileronString = "The elevators are in the neutral position";
        }
        else if (current > 1)
        {
            aileronString = "Left Aileron is deflecting downwards, right Aileron is deflecting upwards";
        }
        else
        {
            aileronString = "Left Aileron is deflecting upwards, right Aileron is deflecting downwards";
        }
    }

    private void GetElevatorPositionChange(float start, float current)
    {
        if (current > -1 && current < 1)
        {
            elevatorString = "The ailerons are in the neutral position";
        }
        else if (current > 1)
        {
            elevatorString = "Elevators are deflecting downwards";
        }
        else
        {
            elevatorString = "Elevators are deflecting upwards";
        }
    }

}
