using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlapsSlider : MonoBehaviour
{
    public Slider flapsSlider;
    public Slider throttleSlider;
    private ControlSurfaces.Surface leftFlap;
    private ControlSurfaces.Surface rightFlap;
    private Vector3 rightFlapStartingRotations;
    private Vector3 leftFlapStartingRotations;

    // Start is called before the first frame update
    void Start()
    {
        leftFlap = ControlSurfaces.leftFlap;
        rightFlap = ControlSurfaces.rightFlap;
        rightFlapStartingRotations = rightFlap.GetStartingRotations();
        leftFlapStartingRotations = leftFlap.GetStartingRotations();
    }

    // Update is called once per frame
    void Update()
    {
        MoveFlaps();
    }

    public void MoveFlaps()
    {
       // Flaps are only moved at low speed, so firstly ensure airspeed is low
       if(throttleSlider.value <= 1)
        {
            float degrees = flapsSlider.value * 10;  // either zero, one or two, so 0, 10 or 20 degrees
            var leftFlapRotation = new Vector3(leftFlapStartingRotations.x, -degrees, leftFlapStartingRotations.z);
            leftFlap.Rotate(leftFlapRotation);
            var rightFlapRotation = new Vector3(rightFlapStartingRotations.x, -degrees, rightFlapStartingRotations.z);
            rightFlap.Rotate(rightFlapRotation);
        }
        else
        {
            flapsSlider.value = 0;
        }

    }
}
