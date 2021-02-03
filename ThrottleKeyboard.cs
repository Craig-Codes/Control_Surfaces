using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrottleKeyboard : MonoBehaviour
{

    private Slider uiSlider;
    private float sliderValue;
    private const float SLIDER_MIN_VALUE = 1;
    private const float SLIDER_MAX_VALUE = 3;
    private const float SLIDER_INCREMENT_VALUE = 1;  // slider increments in values of 1 on keyboard key input

    // particle systems added to list via the inspector
    [SerializeField] List<ParticleSystem> particleList;


    // Start is called before the first frame update
    void Start()
    {
        uiSlider = GameObject.FindObjectOfType<Slider>();
        uiSlider.minValue = SLIDER_MIN_VALUE;  // Slider max value (top) is 3
        uiSlider.maxValue = SLIDER_MAX_VALUE;  // Slider min value (bottom) is 1
    }

    // Update is called once per frame
    void Update()
    {
        uiSlider = GameObject.FindObjectOfType<Slider>();
        sliderValue = uiSlider.value;

        if (Input.GetKeyDown(KeyCode.W))  // Faster
        {
            Debug.Log("Triggered");
            uiSlider.value += SLIDER_INCREMENT_VALUE;
            Debug.Log(sliderValue);
        }

        // Right Joystick
        if (Input.GetKeyDown(KeyCode.X))  // Slower
        {
            uiSlider.value -= SLIDER_INCREMENT_VALUE;
        }

        UpdateClouds(); // update the cloud movement speed based on throttle movement
    }

    private void UpdateClouds()
    {
            foreach (ParticleSystem cloud in particleList)
            {
                ParticleSystem.VelocityOverLifetimeModule velocity = cloud.velocityOverLifetime;
                velocity.speedModifier = uiSlider.value;  // between 1 and 3

                ParticleSystem.EmissionModule emission = cloud.emission;
                emission.rateOverTime = (uiSlider.value / 10) * 5;
            }
    }
 
}
