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

    // Variables used to store rudder starting rotations
    float rudderX, rudderY;

    private float KEYBOARD_DEGRESS = 20f;  // change this is a much smaller amount for other input types to allow some sensitivity - 10f is 20 degrees approx

    private PlayerControls controls;

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
        rudder = GameObject.Find("Rudder");
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();

        rudderX = rudder.transform.localEulerAngles.x;
        rudderY = rudder.transform.localEulerAngles.y;

        controls.KeyboardInput.Enable();  // Start with the keyboard controls enabled
    }

    private void LeftPedalDown()
    {
        PedalDown(leftPedal);
        MoveRudder("left");
    }

    private void LeftPedalUp()
    {
        PedalUp(leftPedal);
        MoveRudder("start");
    }

    private void RightPedalDown()
    {
        PedalDown(rightPedal);
        MoveRudder("right");
    }

    private void RightPedalUp()
    {
        PedalUp(rightPedal);
        MoveRudder("start");
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
            if (eventData.selectedObject.name == "L_Pedal")
            {
                PedalDown(leftPedal);
                MoveRudder("left");
            }
            if (eventData.selectedObject.name == "R_Pedal")
            {
                PedalDown(rightPedal);
                MoveRudder("right");
            }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
            if (eventData.selectedObject.name == "L_Pedal")
            {
                PedalUp(leftPedal);
                MoveRudder("start");
            }
            if (eventData.selectedObject.name == "R_Pedal")
            {
                PedalUp(rightPedal);
                MoveRudder("start");
            }  
    }

    private void MoveRudder(string direction)  // Overloaded method deals with keyboard / mouse input
    {
        float degrees;

        if(direction == "left")
        {
            degrees = KEYBOARD_DEGRESS;
            rudder.transform.localEulerAngles = new Vector3(rudderX, rudderY, degrees);

        }
        else if(direction == "right") 
        {
            degrees = -KEYBOARD_DEGRESS;
            rudder.transform.localEulerAngles = new Vector3(rudderX, rudderY, degrees);
        }
        else if(direction == "start")
        {
            rudder.transform.localEulerAngles = new Vector3(rudderX, rudderY, 0f);  // return to start position
        }
        //float degrees = currentJoystickCoords.y * degreesPerJoystickMove;
        // rotate around parents pivot point on the y axis to the required degrees out of 20
        Debug.Log(rudder.transform.localEulerAngles);
    }

    private void MoveRudder()  // Method deals with control pad input
    {
        //float degrees currentJoystickCoords.x* degreesPerJoystickMove;

        //rudder.transform.localEulerAngles = new Vector3(rudderX, rudderY, degrees);  // return to start position
     
        //float degrees = currentJoystickCoords.y * degreesPerJoystickMove;
        // rotate around parents pivot point on the y axis to the required degrees out of 20
    }

    private void LeftPedalDownKeyboard()
    {
            LeftPedalDown();
    }

    private void LeftPedalUpKeyboard()
    {
            LeftPedalUp();
    }

    private void RightPedalDownKeyboard()
    {
            RightPedalDown();
    }

    private void RightPedalUpKeyboard()
    {
            RightPedalUp();
    }
}




