using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Controls mouse inputs
// All UI Joystick mouse movements are controlled via the ControlsUtilityMethods script, via the update method
// In this case, the Joysticks current location is calculated, and surfaces are moved accordingly
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

    //////////////////////////////////////////////////////////////////////
    /////////////////////////// PEDALS //////////////////////////////////
    ////////////////////////////////////////////////////////////////////

    // Event System and Standalone input module added to buttons in Unity to detect MouseDown and MouseUp events
    // Buttons must also have the script attatched
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (eventData.selectedObject.name == "L_Pedal")
    //    {
    //        ControlsUtilityMethods.PedalDown(leftPedal, mouseDegrees);
    //        ControlsUtilityMethods.PedalUp(rightPedal);
    //    }
    //    if (eventData.selectedObject.name == "R_Pedal")
    //    {
    //        ControlsUtilityMethods.PedalDown(rightPedal, mouseDegrees);
    //        ControlsUtilityMethods.PedalUp(leftPedal);
    //    }
    //}

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
    void Update()
    {
        ControlsUtilityMethods.RotateSurfaces();  // Each frame we want to move elevators and ailerons based on joysticks location
    }

}
