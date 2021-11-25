/* Script controls seperate mouse / touch interactions for the UI  pedals. All
other mouse controls are handled generically in the ControlsUtilityMethods
script */

using UnityEngine;
using UnityEngine.UI;

public class MouseControls : MonoBehaviour
{
    // Access to the pedal buttons which control the rudder
    private Button leftPedal;
    private Button rightPedal;

    // number of degrees to move a surface on keyboard press
    private const float MOUSE_DEGREES = 20f;

    void Awake()
    {
        // Store the pedal button references inside the varaibles
        leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
        rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();
    }

    // Move left pedal down and right pedal up - movement controlled by PedalDown method
    public void OnLeftPedalDown()
    {
        ControlsUtilityMethods.PedalDown(leftPedal, MOUSE_DEGREES);
        ControlsUtilityMethods.PedalUp(rightPedal);
    }

    // Move right pedal down and left pedal up - movement controlled by PedalDown method
    public void OnRightPedalDown()
    {
        ControlsUtilityMethods.PedalDown(rightPedal, MOUSE_DEGREES);
        ControlsUtilityMethods.PedalUp(leftPedal);
    }

    // When mouse no longer clicking, move both pedals back up
    public void OnPointerUp()
    {
        ControlsUtilityMethods.PedalBothUp();
    }
}
