using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AileronRotationKeyboard : MonoBehaviour
{
    GameObject leftAileron;
    GameObject rightAileron;

    private float SPEED = 100f;
    private float KEYBOARD_DEGRESS = 25f;  // change this is a much smaller amount for other input types to allow some sensitivity
    private Vector3 inspectorAngle;
    private float TOPCLAMP = 20f;  // inspector angle at top end of rotation
    private float BOTTOMCLAMP = -20f;  // inspector angle at bottom end of rotation
    float inspectorFloat;

    public static KeyboardRotationHelperMethods.Position surfacePosition = KeyboardRotationHelperMethods.Position.neutral;  // start at neutral

    void Start()
    {
        leftAileron = GameObject.Find("L_Aileron");
        rightAileron = GameObject.Find("R_Aileron");
    }

    // Update is called once per frame
    void Update()
    {

        // Left joystick
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            MoveSurface("left");  // move the surface
            // Get changed angle of the control surface
            inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);  
            surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
            KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
        }

        if (Input.GetKeyUp(KeyCode.Keypad4) || Input.GetKeyUp(KeyCode.Alpha4)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            MoveSurface("right");  // move the surface
            // Get changed angle of the control surface
            inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);
            surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
            KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
        }

        // Right Joystick
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            MoveSurface("right");  // move the surface
            // Get changed angle of the control surface
            inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);
            surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
            KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
        }

        if (Input.GetKeyUp(KeyCode.Keypad6) || Input.GetKeyUp(KeyCode.Alpha6)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            MoveSurface("left");  // move the surface
            // Get changed angle of the control surface
            inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);
            surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
            KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
        }
    }

    private void MoveSurface(string direction)
    {
        if(direction == "left")
        {
            if (inspectorFloat > BOTTOMCLAMP)
            {
                Quaternion rightRotation = Quaternion.Euler(0f, -KEYBOARD_DEGRESS, 0f);
                KeyboardRotationHelperMethods.RotateSurface(rightAileron, rightRotation, SPEED); // right moves down
                Quaternion leftRotation = Quaternion.Euler(0f, KEYBOARD_DEGRESS, 0f);
                KeyboardRotationHelperMethods.RotateSurface(leftAileron, leftRotation, SPEED); // left moves up

            }
        }
        else if(direction == "right")
        {
            if (inspectorFloat < TOPCLAMP)
            {
                Quaternion rightRotation = Quaternion.Euler(0f, KEYBOARD_DEGRESS, 0f);
                KeyboardRotationHelperMethods.RotateSurface(rightAileron, rightRotation, SPEED); // right moves up
                Quaternion leftRotation = Quaternion.Euler(0f, -KEYBOARD_DEGRESS, 0f);
                KeyboardRotationHelperMethods.RotateSurface(leftAileron, leftRotation, SPEED); // left moves down

            }
        }

    }
}
