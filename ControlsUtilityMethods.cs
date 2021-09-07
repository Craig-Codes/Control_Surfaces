

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Script is used for all shared Methods and variables across the control scripts
// This script interacts with the actual control surface objects, based on commands from the control type scripts
public class ControlsUtilityMethods
{
    private static Button leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
    private static Button rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();
    // Pedal sizes changed from just one location
    private static Vector3 pedalFullsize = new Vector3(0.75f, 0.75f, 0.75f);
    private static Vector3 pedalSmallsize = new Vector3(0.6f, 0.6f, 0.6f);

    // Joystick
    static RectTransform joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle").GetComponent<RectTransform>(); // Get the joystick Handle object
    // Joystick starting positions
    public static float joystickX, joystickY;

   static Vector2 currentJoystickCoords;

    private const float buttonPressDegrees = 20f;

    static float degreesPerJoystickMove = 0.3125f;  // Joystick movment for 20 degrees
    //static float degreesPerJoystickMoveLess = 0.15625f;   // Joystick movement for 15 degrees 
    /* Joystick moves from +64 / -64 on both x and y axis
    For y axis - fully up at 64, fully down at -64 
    On each move, need to know the distance from the center, with the control surface being moved (20 degrees) enough to
    map +20 / -20 with the +64 / -64 
    20 / 64 = 0.3125
    We need to rotate by +/-0.3125 for each joystick movement
*/
   
    static ControlSurfaces.Rudder rudder = ControlSurfaces.rudder;
    static ControlSurfaces.Surface leftAileron = ControlSurfaces.leftAileron;
    static ControlSurfaces.Surface rightAileron = ControlSurfaces.rightAileron;
    static ControlSurfaces.Surface leftElevator = ControlSurfaces.leftElevator;
    static ControlSurfaces.Surface rightElevator = ControlSurfaces.rightElevator;
    static ControlSurfaces.Surface leftFlap = ControlSurfaces.leftFlap;
    static ControlSurfaces.Surface rightFlap = ControlSurfaces.rightFlap;

    static Vector3 rudderStartingRotations = rudder.GetStartingRotations();
    static Vector3 rightElevatorStartingRotations = rightElevator.GetStartingRotations();
    static Vector3 leftElevatorStartingRotations = leftElevator.GetStartingRotations();
    static Vector3 rightAileronStartingRotations = rightAileron.GetStartingRotations();
    static Vector3 leftAileronStartingRotations = leftAileron.GetStartingRotations();
    static Vector3 rightFlapStartingRotations = rightFlap.GetStartingRotations();
    static Vector3 leftFlapStartingRotations = leftFlap.GetStartingRotations();

    private static Slider flapsSlider = GameObject.FindGameObjectWithTag("FlapSlider").GetComponent<Slider>();

    // Throttle
    private static Slider throttleSlider = GameObject.FindGameObjectWithTag("ThrottleSlider").GetComponent<Slider>();
    private static RectTransform airspeedNeedle = GameObject.FindGameObjectWithTag("SpeedNeedle").GetComponent<RectTransform>();

    // const variables are set at compile time
    private const float sliderMinValue = 1;
    private const float sliderMaxValue = 3;

    // Add references to the different cloud particle systems
    private static ParticleSystem clouds1 = GameObject.Find("Clouds").GetComponent<ParticleSystem>();
    private static ParticleSystem clouds2 = GameObject.Find("Clouds2").GetComponent<ParticleSystem>();
    private static ParticleSystem clouds3 = GameObject.Find("Clouds3").GetComponent<ParticleSystem>();
    // Add the particle systems into a list
    private static List<ParticleSystem> particleList = new List<ParticleSystem> { clouds1, clouds2, clouds3 };

    // Get each surfaces starting positions

    //////////////////////////////////////////////////////////////////////
    /////////////////// CLICK CONTROLS //////////////////////////////////
    ////////////////////////////////////////////////////////////////////

    public static void PedalDown(Button pedal, float degrees) // Rotate Rudder based on pedal pressed by input type
    {
        pedal.transform.localScale = pedalSmallsize; // make pedal smaller to visualise foot press
        if(pedal == rightPedal)
        {
            Vector3 rudderRotation = new Vector3(rudderStartingRotations.x, rudderStartingRotations.y, -degrees);
            rudder.Rotate(rudderRotation);
        }
        else if(pedal == leftPedal)
        {
            Vector3 rudderRotation = new Vector3(rudderStartingRotations.x, rudderStartingRotations.y, degrees);
            rudder.Rotate(rudderRotation);
        }
    }

    public static void PedalUp(Button pedal)
    {
        pedal.transform.localScale = pedalFullsize;  // set pedal back to large
    }

    public static void PedalBothUp()
    {
        leftPedal.transform.localScale = pedalFullsize;  // set pedals back to large
        rightPedal.transform.localScale = pedalFullsize;
        Vector3 rudderRotation = new Vector3(rudderStartingRotations.x, rudderStartingRotations.y, 0);
        rudder.Rotate(rudderRotation);
    }

    // On single click press, one pedal comes down, and the other returns up
    public static void LeftPedalDownKeyboard()
    {
        PedalDown(leftPedal, buttonPressDegrees);
        PedalUp(rightPedal);
    }

    public static void LeftPedalUpKeyboard()
    {
        PedalBothUp();
    }

    public static void RightPedalDownKeyboard()
    {
        PedalDown(rightPedal, buttonPressDegrees);
        PedalUp(leftPedal);
    }

    public static void RightPedalUpKeyboard()
    {
        PedalBothUp();
    }

    //////////////////////////////////////////////////////////////////////
    ///////////////////////// JOYSTICK //////////////////////////////////
    ////////////////////////////////////////////////////////////////////

    public static void RotateSurfaces()
    {
        GetCurrentJoystickCoords();
        MoveElevators();
        MoveAilerons();
    }

    private static void GetCurrentJoystickCoords()  // Get the coords from the Joystick.cs script each time the joystick is moved.
    {
        joystickX = joystickHandle.localPosition.x;
        joystickY = joystickHandle.localPosition.y;
        currentJoystickCoords = new Vector2(joystickX, joystickY);
    }

    private static void MoveElevators()
    {
        float degrees = currentJoystickCoords.y * degreesPerJoystickMove;
        // rotate around parents pivot point on the y axis to the required degrees out of 20
        var leftElevatorRotation = new Vector3(leftElevatorStartingRotations.x, degrees, leftElevatorStartingRotations.z);
        leftElevator.Rotate(leftElevatorRotation);
        var rightElevatorRotation = new Vector3(rightElevatorStartingRotations.x, -degrees, rightElevatorStartingRotations.z);
        rightElevator.Rotate(rightElevatorRotation);
    }

    private static void MoveAilerons()
    {
        float degrees = currentJoystickCoords.x * degreesPerJoystickMove;
        var leftAileronRotation = new Vector3(leftAileronStartingRotations.x, -degrees, leftAileronStartingRotations.z);
        leftAileron.Rotate(leftAileronRotation);
        var rightAileronRotation = new Vector3(rightAileronStartingRotations.x, degrees, rightAileronStartingRotations.z);
        rightAileron.Rotate(rightAileronRotation);
    }

    //////////////////////////////////////////////////////////
    ////////////////////// FLAPS /////////////////////////////
    //////////////////////////////////////////////////////////
    public static void MoveFlapsDown()  // Control Input method
    {
        flapsSlider.value += 1;
    }

    public static void MoveFlapsUp()  
    {
        flapsSlider.value -= 1;
    }

    // Moves flaps based on the flaps slider value
    public static void MoveFlaps()
    {
        // Flaps are only moved at low speed, so firstly ensure airspeed is low
        if (throttleSlider.value <= 1)
        {
            float degrees = flapsSlider.value * 5;  // either zero, one or two, so 0, 10 or 20 degrees
            var leftFlapRotation = new Vector3(leftFlapStartingRotations.x, -degrees, leftFlapStartingRotations.z);
            leftFlap.Rotate(leftFlapRotation);
            var rightFlapRotation = new Vector3(rightFlapStartingRotations.x, -degrees, rightFlapStartingRotations.z);
            rightFlap.Rotate(rightFlapRotation);
        }
        else
        {
            flapsSlider.value = 0;  // Reset the flaps slider back to top
            var leftFlapRotation = new Vector3(leftFlapStartingRotations.x, 0, leftFlapStartingRotations.z);
            leftFlap.Rotate(leftFlapRotation);
            var rightFlapRotation = new Vector3(rightFlapStartingRotations.x, 0, rightFlapStartingRotations.z);
            rightFlap.Rotate(rightFlapRotation);
        }

    }

    //////////////////////////////////////////////////////////
    ////////////////////// THROTTLE //////////////////////////
    //////////////////////////////////////////////////////////

    public static void UpdateCloudSpeed()
    {
        // Setup the throttle values - cannot be done in start or awake methods as static classes can't have these
        throttleSlider.minValue = sliderMinValue;  // Slider max value (top) is 3
        throttleSlider.maxValue = sliderMaxValue;  // Slider min value (bottom) is 1

        // Change particle system speeds based on current thottle value
        foreach (ParticleSystem cloud in particleList)
        {
            ParticleSystem.VelocityOverLifetimeModule velocity = cloud.velocityOverLifetime;
            velocity.speedModifier = throttleSlider.value;  // between 1 and 3

            ParticleSystem.EmissionModule emission = cloud.emission;
            emission.rateOverTime = (throttleSlider.value / 10) * 5;
        }

        // Update the AirSpeed Needle based on throttle position
        MoveAirspeedNeedle(throttleSlider.value);
    }

    public static void MoveAirspeedNeedle(float sliderValue)
    {
        float newSpeedNeedleValue = sliderValue * 142;
        airspeedNeedle.anchoredPosition = new Vector2(airspeedNeedle.anchoredPosition.x, newSpeedNeedleValue);  // 142 * throttle number of 1-3 corresponds to needle x axis position
    }


    public static void MoveUiThrottle(Vector2 context)
    {
        Vector2 leftStickCoords = context;
        throttleSlider.value = leftStickCoords.y + 2;
    }

}


