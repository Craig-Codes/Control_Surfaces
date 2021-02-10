//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;  // Gives access to UI methods
//using UnityEngine.InputSystem;

//public class ElevatorsRotateKeyboard : MonoBehaviour
//{
//    private GameObject leftElevator;
//    private GameObject rightElevator;

//    private float SPEED = 100f;
//    private float KEYBOARD_DEGRESS = 25f;  // change this is a much smaller amount for other input types to allow some sensitivity
//    private float TOPCLAMP = 20f;  // inspector angle at top end of rotation
//    private float BOTTOMCLAMP = -20f;  // inspector angle at bottom end of rotation
//    private float inspectorFloatAngle;


//    public static JoystickMovement.Position surfacePosition;  // public static so that Joystick movement scripts can access this variable

//    private PlayerControls controls;

//    void Awake()
//    {
//        controls = new PlayerControls();
//        controls.KeyboardInput.ElevatorsUp.performed += context => ElevatorsUp();  // context cant be used to get input information
//        controls.KeyboardInput.ElevatorsDown.performed += context => ElevatorsDown();  // context cant be used to get input information
//        controls.KeyboardInput.ElevatorsUpReverse.performed += context => ElevatorsUpReverse();  // context cant be used to get input information
//        controls.KeyboardInput.ElevatorsDownReverse.performed += context => ElevatorsDownReverse();  // context cant be used to get input information

//    }
//   private void Start()  
//    {
//        leftElevator = GameObject.Find("LeftElevator");
//        rightElevator = GameObject.Find("RightElevator");
//        surfacePosition = JoystickMovement.Position.neutral;
//        controls.KeyboardInput.Enable();  // Start with the keyboard controls enabled
//    }

//    private void ElevatorsUp()
//    {
//        MoveSurface("up");  // Move the surface
//        inspectorFloatAngle = RotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);  // wrap the y angle to stop it going over 180 or bellow -180
//        surfacePosition = JoystickMovement.GetSurfacePosition(inspectorFloatAngle);  // Update the surface position varaible
//        JoystickMovement.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//    private void ElevatorsUpReverse()
//    {
//        MoveSurface("down");  // Move the surface
//        inspectorFloatAngle = RotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);  // wrap the y angle to stop it going over 180 or bellow -180
//        surfacePosition = JoystickMovement.GetSurfacePosition(inspectorFloatAngle);  // Update the surface position varaible
//        JoystickMovement.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//    private void ElevatorsDown()
//    {
//        MoveSurface("down");  // Move the surface
//        inspectorFloatAngle = RotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);  // wrap the y angle to stop it going over 180 or bellow -180
//        surfacePosition = JoystickMovement.GetSurfacePosition(inspectorFloatAngle);  // Update the surface position varaible
//        JoystickMovement.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//    private void ElevatorsDownReverse()
//    {
//        MoveSurface("up");  // Move the surface
//        inspectorFloatAngle = RotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);  // wrap the y angle to stop it going over 180 or bellow -180
//        surfacePosition = JoystickMovement.GetSurfacePosition(inspectorFloatAngle);  // Update the surface position varaible
//        JoystickMovement.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }


//    private void MoveSurface(string direction)
//    {
//        if(direction == "up")
//        {
//            if (inspectorFloatAngle < TOPCLAMP)
//            {
//                Quaternion rotation = Quaternion.Euler(0f, KEYBOARD_DEGRESS, 0f);
//                RotationHelperMethods.RotateSurface(rightElevator, rotation, SPEED);
//                RotationHelperMethods.RotateSurface(leftElevator, rotation, SPEED);
//            }
//        }
//        else if(direction == "down")
//        {
//            if (inspectorFloatAngle > BOTTOMCLAMP)
//            {
//                Quaternion rotation = Quaternion.Euler(0f, -KEYBOARD_DEGRESS, 0f);
//                RotationHelperMethods.RotateSurface(rightElevator, rotation, SPEED);
//                RotationHelperMethods.RotateSurface(leftElevator, rotation, SPEED);
//            }
//        }
//    }
//}
