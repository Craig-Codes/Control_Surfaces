//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class AileronRotationKeyboard : MonoBehaviour
//{
//    GameObject leftAileron;
//    GameObject rightAileron;

//    private float SPEED = 100f;
//    private float KEYBOARD_DEGRESS = 25f;  // change this is a much smaller amount for other input types to allow some sensitivity
//    private float TOPCLAMP = 20f;  // inspector angle at top end of rotation
//    private float BOTTOMCLAMP = -20f;  // inspector angle at bottom end of rotation
//    float inspectorFloatAngle;

//    public static JoystickMovement.Position surfacePosition;  // start at neutral

//    PlayerControls controls;

//    private void Awake()
//    {
//        controls = new PlayerControls();
//        controls.KeyboardInput.AileronsUp.performed += context => AileronsUp();  // context cant be used to get input information
//        controls.KeyboardInput.AileronsDown.performed += context => AileronsDown();  // context cant be used to get input information
//        controls.KeyboardInput.AileronsUpReverse.performed += context => AileronsUpReverse();  // context cant be used to get input information
//        controls.KeyboardInput.AileronsDownReverse.performed += context => AileronsDownReverse();  // context cant be used to get input information

//    }

//    void Start()
//    {
//        leftAileron = GameObject.Find("L_Aileron");
//        rightAileron = GameObject.Find("R_Aileron");

//        surfacePosition = JoystickMovement.Position.neutral;
//        controls.KeyboardInput.Enable();  // Start with the keyboard controls enabled
//    }

//    private void AileronsUp()
//    {
//        MoveSurface("right");  // move the surface
//        // Get changed angle of the control surface
//        inspectorFloatAngle = RotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);
//        surfacePosition = JoystickMovement.GetSurfacePosition(inspectorFloatAngle);  // Update the surface position varaible
//        JoystickMovement.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//    private void AileronsUpReverse()
//    {
//        MoveSurface("left");  // move the surface
//        inspectorFloatAngle = RotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);
//        surfacePosition = JoystickMovement.GetSurfacePosition(inspectorFloatAngle);  // Update the surface position varaible
//        JoystickMovement.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//    private void AileronsDown()
//    {
//        MoveSurface("left");  // move the surface
//        inspectorFloatAngle = RotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);
//        surfacePosition = JoystickMovement.GetSurfacePosition(inspectorFloatAngle);  // Update the surface position varaible
//        JoystickMovement.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//    private void AileronsDownReverse()
//    {
//        MoveSurface("right");  // move the surface
//        inspectorFloatAngle = RotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);
//        surfacePosition = JoystickMovement.GetSurfacePosition(inspectorFloatAngle);  // Update the surface position varaible
//        JoystickMovement.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//    //private void MoveSurface(string direction)
//    //{
//    //    if(direction == "left")
//    //    {
//    //        if (inspectorFloatAngle > BOTTOMCLAMP)
//    //        {
//    //            Quaternion rightRotation = Quaternion.Euler(0f, -KEYBOARD_DEGRESS, 0f);
//    //            RotationHelperMethods.RotateSurface(rightAileron, rightRotation, SPEED); // right moves down
//    //            Quaternion leftRotation = Quaternion.Euler(0f, KEYBOARD_DEGRESS, 0f);
//    //            RotationHelperMethods.RotateSurface(leftAileron, leftRotation, SPEED); // left moves up
//    //        }
//    //    }
//    //    else if(direction == "right")
//    //    {
//    //        if (inspectorFloatAngle < TOPCLAMP)
//    //        {
//    //            Quaternion rightRotation = Quaternion.Euler(0f, KEYBOARD_DEGRESS, 0f);
//    //            RotationHelperMethods.RotateSurface(rightAileron, rightRotation, SPEED); // right moves up
//    //            Quaternion leftRotation = Quaternion.Euler(0f, -KEYBOARD_DEGRESS, 0f);
//    //            RotationHelperMethods.RotateSurface(leftAileron, leftRotation, SPEED); // left moves down
//    //        }
//    //    }
//    //}
//}
