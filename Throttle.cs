using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class controls how throttle is moved, and actions on movement

public class Throttle : MonoBehaviour
{

    private Slider uiSlider;
    public static float sliderValue;

    private RectTransform airspeedNeedle;

    // const variables are set at compile time
    private const float sliderMinValue = 1;
    private const float sliderMaxValue = 3;
    private const float sliderIncrementValue = 1;  // slider increments in values of 1 on keyboard key input

    // SerializedField lets private varaible show in the inspector, allowing the ParticleSystems to be added from inspector
    [SerializeField] private List<ParticleSystem> particleList;


    // Start is called before the first frame update
    void Start()
    {
        uiSlider = GameObject.FindObjectOfType<Slider>();
        uiSlider.minValue = sliderMinValue;  // Slider max value (top) is 3
        uiSlider.maxValue = sliderMaxValue;  // Slider min value (bottom) is 1

        airspeedNeedle = GameObject.FindGameObjectWithTag("SpeedNeedle").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        sliderValue = uiSlider.value;  // Get the current throttle value

        if (Input.GetKeyDown(KeyCode.W))  // Faster
        {
            Debug.Log("Triggered");
            uiSlider.value += sliderIncrementValue;
            Debug.Log(sliderValue);
        }

        // Right Joystick
        if (Input.GetKeyDown(KeyCode.X))  // Slower
        {
            uiSlider.value -= sliderIncrementValue;
        }

        UpdateClouds(); // update the cloud movement speed based on throttle movement
        MoveAirspeedNeedle(sliderValue);  // Move pointer in relation to throttle slider value
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

    private void MoveAirspeedNeedle(float throttle)
    {
        airspeedNeedle.anchoredPosition = new Vector2(airspeedNeedle.anchoredPosition.x, throttle * 142);  // 142 * throttle number of 1-3 corresponds to needle x axis position
    }

}
