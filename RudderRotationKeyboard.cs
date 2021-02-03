using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RudderRotationKeyboard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler   // Intefaces to interact with the Unity event system
{
    GameObject rudder;
    Button leftPedal;
    Button rightPedal;

    private float SPEED = 100f;
    private float KEYBOARD_DEGRESS = 12f;  // change this is a much smaller amount for other input types to allow some sensitivity - 10f is 20 degrees approx
    private float TOPCLAMP = 10f;  // inspector angle at top end of rotation
    private float BOTTOMCLAMP = -10f;  // inspector angle at bottom end of rotation

    float inspectorFloat;

    void Start()
    {
        rudder = GameObject.Find("RudderPivot");
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        leftPedal.onClick.AddListener(LeftPedalActionsDown);
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();
        rightPedal.onClick.AddListener(RightPedalActionsDown);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A)) // When key pressed down
        {
            LeftPedalActionsDown();
        }

        if (Input.GetKeyUp(KeyCode.A)) // when key is released
        {
            LeftPedalActionsUp();
        }

        if (Input.GetKeyDown(KeyCode.D)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
        {
            RightPedalActionsDown();  
        }

        if (Input.GetKeyUp(KeyCode.D)) 
        {
            RightPedalActionsUp();
        }
    }



    public void RightPedalActionsDown()  
    {
        PedalDown(rightPedal);
        MoveSurface("left");
    }

    public void RightPedalActionsUp()  // return rudder back to center
    {
        PedalUp(rightPedal);
        MoveSurface("right");
    }

    public void LeftPedalActionsDown()
    {
        PedalDown(leftPedal);
        MoveSurface("right");
    }

    public void LeftPedalActionsUp()  // return rudder back to center
    {
        PedalUp(leftPedal);
        MoveSurface("left");
    }


    public void PedalDown(Button pedal)
    {
        pedal.transform.localScale = new Vector3(0.90f, 0.90f, 0.90f); // make pedal smaller to visualise foot press
    }

    public void PedalUp(Button pedal)
    {
        pedal.transform.localScale = new Vector3(1f, 1f, 1f);

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
        Debug.Log(eventData.selectedObject);
        if (eventData.selectedObject.name == "L_Pedal")
        {
            Debug.Log("YAY! LEFT");
            LeftPedalActionsDown();
        }
        if (eventData.selectedObject.name == "R_Pedal")
        {
            Debug.Log("YAY! RIGHT");
            RightPedalActionsDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("UP");
        if (eventData.selectedObject.name == "L_Pedal")
        {
            LeftPedalActionsUp();
        }
        if (eventData.selectedObject.name == "R_Pedal")
        {
            RightPedalActionsUp();
        }
    }

    private void MoveSurface(string direction)
    {
        if (direction == "left")
        {
            if (inspectorFloat < TOPCLAMP)
            {
                Quaternion rotation = Quaternion.Euler(0f, 0f, KEYBOARD_DEGRESS);
                KeyboardRotationHelperMethods.RotateSurface(rudder, rotation, SPEED);
            }
        }
        else if (direction == "right")
        {
            if (inspectorFloat > BOTTOMCLAMP)
            {
                Quaternion rotation = Quaternion.Euler(0f, 0f, -KEYBOARD_DEGRESS);
                KeyboardRotationHelperMethods.RotateSurface(rudder, rotation, SPEED);
            }
        }

    }
}

//void Update()
//{
//    if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
//    {
//        MoveSurface("up");  // Move the surface
//                            // Get changed angle of the control surface
//        inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);  // wrap the y angle to stop it going over 180 or bellow -180
//        surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
//        KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }
//    if (Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
//    {
//        MoveSurface("down");  // Move the surface
//                              // Get changed angle of the control surface
//        inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);  // wrap the y angle to stop it going over 180 or bellow -180
//        surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
//        KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//    if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
//    {
//        MoveSurface("down");
//        // Get changed angle of the control surface
//        inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);
//        surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
//        KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }
//    if (Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) // use GetKey to get constant press, KeyDown only gets once, much better for keyboard
//    {
//        MoveSurface("up");
//        // Get changed angle of the control surface
//        inspectorFloat = KeyboardRotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);
//        surfacePosition = KeyboardRotationHelperMethods.GetSurfacePosition(inspectorFloat);  // Update the surface position varaible
//        KeyboardRotationHelperMethods.ManualJoystickMove();  // move the joystick based on surface position for both ailerons and elevators
//    }

//}

//private void MoveSurface(string direction)
//{
//    if (direction == "up")
//    {
//        if (inspectorFloat < TOPCLAMP)
//        {
//            Quaternion rotation = Quaternion.Euler(0f, KEYBOARD_DEGRESS, 0f);
//            KeyboardRotationHelperMethods.RotateSurface(rightElevator, rotation, SPEED);
//            KeyboardRotationHelperMethods.RotateSurface(leftElevator, rotation, SPEED);
//        }
//    }
//    else if (direction == "down")
//    {
//        if (inspectorFloat > BOTTOMCLAMP)
//        {
//            Quaternion rotation = Quaternion.Euler(0f, -KEYBOARD_DEGRESS, 0f);
//            KeyboardRotationHelperMethods.RotateSurface(rightElevator, rotation, SPEED);
//            KeyboardRotationHelperMethods.RotateSurface(leftElevator, rotation, SPEED);
//        }
//    }

//}



