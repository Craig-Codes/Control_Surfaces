﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AircraftRotationKeyboard : MonoBehaviour
{
    private static float smoothing = 5f; // The smoothing applied to the movement of control surfaces.
    // Work out how Aircraft should be rotating based on the different control surfaces
    // Control the speed of the rotation based on on throttle position
    // Move the camera left / right based on joystick position
    private Camera mainCamera;

    private GameObject aircraft;
    private GameObject rudder;
    private GameObject rightAileron;
    private GameObject rightElevator;

    private Slider uiSlider;
    private float sliderValue;

    float rudderInspectorFloat;
    float aileronInspectorFloat;
    float elevatorInspectorFloat;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("SkyboxCamera").GetComponent<Camera>();
        aircraft = GameObject.Find("AircraftPivot");
        rudder = GameObject.Find("RudderPivot");  
        rightAileron = GameObject.Find("R_Aileron");
        rightElevator = GameObject.Find("RightElevator");

        uiSlider = GameObject.FindObjectOfType<Slider>();
        sliderValue = uiSlider.value;
    }

    // Update is called once per frame
    void Update()
    {

        //RotateCamera();
        RotateAircraft();  // Rotate the aircraft depending on the control surface settings
    }

    //private void RotateCamera()  // Rudder changes direction so controls cam direction - Cannot make camera move to make it look like no rotation if on zero unfortunately
    //{
    //    if(rudderInspectorFloat >= 0)
    //    {
    //        // Rotate Right
    //        mainCamera.transform.Rotate(Vector3.down, sliderValue * Time.deltaTime);
    //    }
    //    else
    //    {
    //        //rotate Left
    //        mainCamera.transform.Rotate(Vector3.up, sliderValue * Time.deltaTime);
    //    }
    //}

    private void RotateAircraft()
    {
        sliderValue = uiSlider.value;
        // Get all of the control surfaces relevant angles
        rudderInspectorFloat = RotationHelperMethods.WrapAngle(rudder.transform.localEulerAngles.z);
        aileronInspectorFloat = RotationHelperMethods.WrapAngle(rightAileron.transform.localEulerAngles.y);
        elevatorInspectorFloat = RotationHelperMethods.WrapAngle(rightElevator.transform.localEulerAngles.y);
        // Get all of the surface relevant locations
        float roll = aileronInspectorFloat;  // ailerons (x axis)
        float pitch = elevatorInspectorFloat;  // elevators (y axis)
        float yaw = rudderInspectorFloat;  // rudder (z axis)

        AxisRotate(pitch, Vector3.up, Vector3.down);
        AxisRotate(roll, Vector3.right, Vector3.left);
        AxisRotate(yaw, Vector3.back, Vector3.forward);
    }

    private void AxisRotate(float axis, Vector3 directionOne, Vector3 directionTwo)
    {
        if(axis >= 1)
        {
            aircraft.transform.localRotation *= Quaternion.AngleAxis((axis / 3) * sliderValue * Time.deltaTime, directionOne);
        }
        else if (axis < 0)
        {
            aircraft.transform.localRotation *= Quaternion.AngleAxis((-axis / 3) * sliderValue * Time.deltaTime, directionTwo);
        }

    }


    public static void RotateSurface(GameObject surface, Quaternion rotation, float speed)
    {
        // Create a target which is the surface's original rotation, rotated by the input.
        Quaternion target = surface.transform.localRotation * rotation; // The orignallocalRotation
                                                                        // Slerp the surface's current rotation towards the target rotation (current rotation * the axis which we want to move, by amount we want to move it by).
        surface.transform.localRotation = Quaternion.Slerp(surface.transform.localRotation, target,
                                                           smoothing * Time.deltaTime * speed);
    }
}