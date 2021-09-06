using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Script controls mouse inputs

// All UI Joystick mouse movements are controlled via the ControlsUtilityMethods script, via the update method
// All UI throttle mouse movements are controlled via the Throttle.cs script

public class MouseControls : MonoBehaviour // Intefaces to interact with the Unity event system
{
    private Button leftPedal;
    private Button rightPedal;

    private float mouseDegrees = 20f;  // number of degrees to move a surface on keyboard press

    void Awake()
    {
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();
    }

    public void OnLeftPedalDown()
    {
            ControlsUtilityMethods.PedalDown(leftPedal, mouseDegrees);
            ControlsUtilityMethods.PedalUp(rightPedal);
    }

    public void OnRightPedalDown()
    {
            ControlsUtilityMethods.PedalDown(rightPedal, mouseDegrees);
            ControlsUtilityMethods.PedalUp(leftPedal);
    }

    public void OnPointerUp()
    {
        ControlsUtilityMethods.PedalBothUp();
    }

    // Joystick Controls, based on joysticks current user moved location


}
