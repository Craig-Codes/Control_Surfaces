﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDescriptions : MonoBehaviour
{

    private TextMeshProUGUI controlInputDescription;

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
        rudderZAngle = WrapAngle(rudder.GetCurrentRotations().z);
        aileronYAngle = WrapAngle(leftAileron.GetCurrentRotations().y);
        elevatorYAngle = WrapAngle(leftElevator.GetCurrentRotations().y);

        // Get the TextMeshPro via code
        var textArray = FindObjectsOfType<TextMeshProUGUI>();
        foreach (var element in textArray)
        {
            if (element.tag == "ControlsDescription")
            {
                controlInputDescription = element;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Rudder 
        rudderPreviousValue = rudderZAngle;  // Put current position value into variable
        rudderZAngle = WrapAngle(rudder.GetCurrentRotations().z);  // update the position value
        rudderCurrentValue = rudderZAngle;  // Put new position value into varaible
        GetRudderPositionChange(rudderPreviousValue, rudderCurrentValue);  // compare the values, building an output string based on results

        // Ailerons
        aileronPreviousValue = aileronYAngle;
        aileronYAngle = WrapAngle(leftAileron.GetCurrentRotations().y);
        aileronCurrentValue = aileronYAngle;
        GetAileronPositionChange(aileronPreviousValue, aileronCurrentValue);

        // Elevators
        elevatorPreviousValue = elevatorYAngle;
        elevatorYAngle = WrapAngle(leftElevator.GetCurrentRotations().y);
        elevatorCurrentValue = elevatorYAngle;
        GetElevatorPositionChange(elevatorPreviousValue, elevatorCurrentValue);

        TextOutput();
        
    }

    // Generate the string based on control surfaces movement
    private void TextOutput()
    {
        controlInputDescription.text =
     $"{elevatorString}\n" +
     $"{aileronString}\n" +
     $"{rudderString}\n";
    }

    private void GetRudderPositionChange(float start, float current)
    {
        if(current > -1 && current < 1)
        {
            rudderString = "The rudder is in the neutral position";
        }
        else if(current > 1)
        {
            rudderString = "The rudder is deflecting to starboard";
        }
        else
        {
            rudderString = "The rudder is deflecting to port";
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
            aileronString = "Portside Aileron is deflecting downwards, starboard is deflecting upwards";
        }
        else
        {
            aileronString = "Portside Aileron is deflecting upwards, starboard is deflecting downwards";
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

    private float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
}