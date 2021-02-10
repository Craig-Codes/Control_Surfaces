using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHelperMethods : MonoBehaviour
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


    // Keeps angle between 180 and -180 so that Euler angle can be used properly
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

}
