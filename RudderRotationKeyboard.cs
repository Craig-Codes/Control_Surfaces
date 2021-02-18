using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RudderRotationKeyboard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler   // Intefaces to interact with the Unity event system
{
    private GameObject rudder;
    private Button leftPedal;
    private Button rightPedal;

    private Vector3 pedalFullsize = new Vector3(0.75f, 0.75f, 0.75f);
    private Vector3 pedalSmallsize = new Vector3(0.6f, 0.6f, 0.6f);

    private InputType currentInput;

    private float SPEED = 100f;
    private float KEYBOARD_DEGRESS = 8f;  // change this is a much smaller amount for other input types to allow some sensitivity - 10f is 20 degrees approx
    private float MOUSE_DEGREES = 24f;  // degrees to move the rudder on mouse click - half of those to keyboard as rudder will always be in the middle on mouse click,

    private PlayerControls controls;

    private enum InputType  // enum used to potentially expand out class for HOTAS / gamepad
    {
        keyboard,
        mouse,
        none
    }

    void Awake()
    {
        controls = new PlayerControls();
        controls.KeyboardInput.RudderLeftDown.performed += context => LeftPedalDownKeyboard();// context cant be used to get input information
        controls.KeyboardInput.RudderLeftUp.performed += context => LeftPedalUpKeyboard();  // context cant be used to get input information
        controls.KeyboardInput.RudderRightDown.performed += context => RightPedalDownKeyboard();  // context cant be used to get input information
        controls.KeyboardInput.RudderRightUp.performed += context => RightPedalUpKeyboard();  // context cant be used to get input information

    }
    void Start()
    {
        rudder = GameObject.Find("RudderPivot");
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();
        currentInput = InputType.none;
        controls.KeyboardInput.Enable();  // Start with the keyboard controls enabled
    }

    private void LeftPedalDown()
    {
        PedalDown(leftPedal);
        MoveSurface("left", KEYBOARD_DEGRESS);
    }

    private void LeftPedalUp()
    {
        PedalUp(leftPedal);
        MoveSurface("right", KEYBOARD_DEGRESS);
    }

    private void RightPedalDown()
    {
        PedalDown(rightPedal);
        MoveSurface("right", KEYBOARD_DEGRESS);
    }

    private void RightPedalUp()
    {
        PedalUp(rightPedal);
        MoveSurface("left", KEYBOARD_DEGRESS);
    }


    private void PedalDown(Button pedal)
    {
        pedal.transform.localScale = pedalSmallsize; // make pedal smaller to visualise foot press
    }

    private void PedalUp(Button pedal)
    {
        pedal.transform.localScale = pedalFullsize;

    }

    //IEnumerator PedalCoroutine(Button pedal)  // Used to wait a little before pedal moves back up again
    //{
    //    yield return new WaitForSeconds(0.2f);  // wait 200ms
    //    pedal.transform.localScale = new Vector3(1f, 1f, 1f);
    //}


    // Event System and Standalone input module added to buttons in Unity to detect MouseDown and MouseUp events
    // Buttons must also have the script attatched
    public void OnPointerDown(PointerEventData eventData)
    {
        if(currentInput == InputType.none || currentInput == InputType.mouse)
        { // If currentInput is keyboard, do nothing

            if (eventData.selectedObject.name == "L_Pedal")
            {
                InputSystem.DisableDevice(Keyboard.current);
                currentInput = InputType.mouse;
                PedalDown(leftPedal);
                MoveSurface("left", MOUSE_DEGREES);
            }
            if (eventData.selectedObject.name == "R_Pedal")
            {
                InputSystem.DisableDevice(Keyboard.current);
                currentInput = InputType.mouse;
                PedalDown(rightPedal);
                MoveSurface("right", MOUSE_DEGREES);
            }
        }

    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentInput == InputType.mouse)
        {
            if (eventData.selectedObject.name == "L_Pedal")
            {
                PedalUp(leftPedal);
                MoveSurface("right", MOUSE_DEGREES);
                currentInput = InputType.none;
                InputSystem.EnableDevice(Keyboard.current);
            }
            if (eventData.selectedObject.name == "R_Pedal")
            {
                PedalUp(rightPedal);
                MoveSurface("left", MOUSE_DEGREES);
                currentInput = InputType.none;
                InputSystem.EnableDevice(Keyboard.current);
            }
        }    
    }

    private void MoveSurface(string direction, float degrees)
    {
        if (direction == "left")
        {
                Quaternion rotation = Quaternion.Euler(0f, 0f, degrees);
                RotationHelperMethods.RotateSurface(rudder, rotation, SPEED);
        }
        else if (direction == "right")
        {
                Quaternion rotation = Quaternion.Euler(0f, 0f, -degrees);
                RotationHelperMethods.RotateSurface(rudder, rotation, SPEED);
        }

    }

    private void LeftPedalDownKeyboard()
    {
        if (currentInput == InputType.none || currentInput == InputType.keyboard)
        {
            currentInput = InputType.keyboard;
            LeftPedalDown();
        }
    }

    private void LeftPedalUpKeyboard()
    {
        if (currentInput == InputType.none || currentInput == InputType.keyboard)
        {
            LeftPedalUp();
            currentInput = InputType.none;
        }
    }

    private void RightPedalDownKeyboard()
    {
        if (currentInput == InputType.none || currentInput == InputType.keyboard)
        {
            currentInput = InputType.keyboard;
            RightPedalDown();
        }
    }

    private void RightPedalUpKeyboard()
    {
        if (currentInput == InputType.none || currentInput == InputType.keyboard)
        {
            RightPedalUp();
            currentInput = InputType.none;
        }
    }
}




