using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardRotationHelperMethods : MonoBehaviour
{
    private static float smoothing = 5f; // The smoothing applied to the movement of control surfaces.
    public static void RotateSurface(GameObject surface, Quaternion rotation, float speed)
    {
        // Create a target which is the surface's original rotation, rotated by the input.
        Quaternion target = surface.transform.localRotation * rotation; // The orignallocalRotation
        // Slerp the surface's current rotation towards the target rotation (current rotation * the axis which we want to move, by amount we want to move it by).
        surface.transform.localRotation = Quaternion.Slerp(surface.transform.localRotation, target,
                                                           smoothing * Time.deltaTime * speed);
    }

    public enum Position  // possible positions for the control surface when using a keyboard
    {
        neutral,
        up,
        down
    }

    public static Position GetSurfacePosition(float location)
    {
        // 1.3 / -1.3 used for leeway against crazy big inperfect rotation numbers!
        if (location < 1.3 && location > -1.3)
        {
            return Position.neutral;
        }
        else if (location > 0)
        {
            return Position.up;
        }
        else
        {
            return Position.down;
        }
    }

    public static void ManualJoystickMove()
    {
        RectTransform joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>();  // Get the joystick Handle object
        Position aileronPosition = AileronRotationKeyboard.surfacePosition;  // Get the current Aileron position
        Position elevatorPosition = ElevatorsRotateKeyboard.surfacePosition;  // Get the current Elevator position

        switch (aileronPosition)
        {
            case Position.neutral:
                if(elevatorPosition == Position.neutral)
                {
                    joystickHandle.transform.localPosition = new Vector3(0, 0, 0);
                }
                else if(elevatorPosition == Position.up)
                {
                    joystickHandle.transform.localPosition = new Vector3(0, 62, 0);
                }
                else if (elevatorPosition == Position.down)
                {
                    joystickHandle.transform.localPosition = new Vector3(0, -62, 0);
                }
                break;
            case Position.up:
                if (elevatorPosition == Position.neutral)
                {
                    joystickHandle.transform.localPosition = new Vector3(62, 0, 0);
                }
                else if (elevatorPosition == Position.up)
                {
                    joystickHandle.transform.localPosition = new Vector3(45, 45, 0);
                }
                else if (elevatorPosition == Position.down)
                {
                    joystickHandle.transform.localPosition = new Vector3(45, -45, 0);
                }
                break;
            case Position.down:
                if (elevatorPosition == Position.neutral)
                {
                    joystickHandle.transform.localPosition = new Vector3(-62, 0, 0);
                }
                else if (elevatorPosition == Position.up)
                {
                    joystickHandle.transform.localPosition = new Vector3(-45, 45, 0);
                }
                else if (elevatorPosition == Position.down)
                {
                    joystickHandle.transform.localPosition = new Vector3(-45, -45, 0);
                }
                break;
        }
    }

    // Keeps angle between 180 and -180 so that Euler angle can be used properly
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

}
