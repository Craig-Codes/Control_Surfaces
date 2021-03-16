using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirspeedCalculation : MonoBehaviour
{
    private RectTransform airspeedNeedle;

    private Slider uiSlider;
    private float sliderValue;

    // Start is called before the first frame update
    void Start()
    {
        airspeedNeedle = GameObject.FindGameObjectWithTag("SpeedNeedle").GetComponent<RectTransform>();
        Debug.Log(airspeedNeedle);

        uiSlider = FindObjectOfType<Slider>();
        sliderValue = uiSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        sliderValue = uiSlider.value;
        MovePointer(sliderValue);
    }

    private void MovePointer(float throttle)
    {
        airspeedNeedle.anchoredPosition = new Vector2(airspeedNeedle.anchoredPosition.x, throttle * 142);
        Debug.Log(airspeedNeedle.anchoredPosition.y);
    }
}
