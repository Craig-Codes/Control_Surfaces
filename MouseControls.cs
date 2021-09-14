// Script controls seperate mouse / touch interactions for the UI  pedals
// All other mouse controls are handled generically in the ControlsUtilityMethods script
// Whereby movement to various sliders and the joystick impact control surfaces

using UnityEngine;
using UnityEngine.UI;

public class MouseControls : MonoBehaviour 
{
    // Access to the pedal buttons which control the rudder
    private Button leftPedal;
    private Button rightPedal;

    private const float MOUSE_DEGREES = 20f;  // number of degrees to move a surface on keyboard press

    void Awake()
    {
        // Store the pedal button references inside the varaibles
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();
    }

    public void OnLeftPedalDown()  // Move left pedal down and right pedal up - movement controlled by PedalDown method
    {
            ControlsUtilityMethods.PedalDown(leftPedal, MOUSE_DEGREES);
            ControlsUtilityMethods.PedalUp(rightPedal);
    }

    public void OnRightPedalDown()  // Move right pedal down and left pedal up - movement controlled by PedalDown method
    {
            ControlsUtilityMethods.PedalDown(rightPedal, MOUSE_DEGREES);
            ControlsUtilityMethods.PedalUp(leftPedal);
    }

    public void OnPointerUp()  // When mouse no longer clicking, move both pedals back up
    {
        ControlsUtilityMethods.PedalBothUp();
    }
}
