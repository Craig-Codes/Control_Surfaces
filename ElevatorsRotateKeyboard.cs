using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Gives access to UI methods

public class ElevatorsRotateKeyboard : MonoBehaviour
{
    GameObject leftElevator;
    GameObject rightElevator;

    private float SPEED = 100f;
    private float KEYBOARD_DEGRESS = 25f;  // change this is a much smaller amount for other input types to allow some sensitivity
    private Vector3 inspectorAngle;
    private float TOPCLAMP = 20f;  // inspector angle at top end of rotation
    private float BOTTOMCLAMP = -20f;  // inspector angle at bottom end of rotation
    float inspectorFloat;

    public static KeyboardRotationHelperMethods.Position surfacePosition = KeyboardRotationHelperMethods.Position.neutral;  // Static so it always holds a value

    void Start()
    {
        leftElevator = GameObject.Find("LeftElevator");
        rightElevator = GameObject.Find("RightElevator");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            MoveSurface("up");  // Move the surface
            // Get changed angle of the control surface
            inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);  // wrap the y angle to stop it going over 180 or bellow -180
            surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
            KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
        }
        if (Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            MoveSurface("down");  // Move the surface
            // Get changed angle of the control surface
            inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);  // wrap the y angle to stop it going over 180 or bellow -180
            surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
            KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
        }

        if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            MoveSurface("down");
            // Get changed angle of the control surface
            inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);
            surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
            KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
        }
        if (Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            MoveSurface("up");
            // Get changed angle of the control surface
            inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);
            surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
            KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
        }

    }

    private void MoveSurface(string direction)
    {
        if(direction == "up")
        {
            if (inspectorFloat < TOPCLAMP)
            {
                Quaternion rotation = Quaternion.Euler(0f, KEYBOARD_DEGRESS, 0f);
                KeyboardRotationHelperMethods.RotateSurface(rightElevator, rotation, SPEED);
                KeyboardRotationHelperMethods.RotateSurface(leftElevator, rotation, SPEED);
            }
        }
        else if(direction == "down")
        {
            if (inspectorFloat > BOTTOMCLAMP)
            {
                Quaternion rotation = Quaternion.Euler(0f, -KEYBOARD_DEGRESS, 0f);
                KeyboardRotationHelperMethods.RotateSurface(rightElevator, rotation, SPEED);
                KeyboardRotationHelperMethods.RotateSurface(leftElevator, rotation, SPEED);
            }
        }

    }

}
