// Script is used for all shared Methods and variables across scripts
// Utility script is used to allow DRY code (Don't Repeat Yourself)
// Allows methods to be called from other scripts without the need to get object references in each individual script
// A Static class is not instantiated, so no reference to this class needs to be stored in individal scripts to use the methods

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public static class ControlsUtilityMethods
{
    // Reference to the pedal buttons
    private static Button leftPedal = GameObject.Find("L_Pedal").GetComponent<Button>();
    private static Button rightPedal = GameObject.Find("R_Pedal").GetComponent<Button>();
    // Pedal sizes changed from just one location
    // Readonly used so that varaibles cannot be redeclared after construction
    // Only C# built in types can be declared as constants, so Vector3 doesnt allow this so readonly used
    private static readonly Vector3 pedalFullsize = new Vector3(0.75f, 0.75f, 0.75f);
    private static readonly Vector3 pedalSmallsize = new Vector3(0.6f, 0.6f, 0.6f);

    // Reference to the UI Joystick object
    private static RectTransform joystickHandle = GameObject.FindGameObjectWithTag("JoystickHandle")
        .GetComponent<RectTransform>(); // Get the joystick Handle object
    private static float joystickX, joystickY;// Joystick starting positions
    private static Vector2 currentJoystickCoords;  // Reference to current Joystick position both x and y axis
    private const float BUTTON_PRESS_DEGREES = 20f;  // Move 20 degrees when a keyboard button is used to move the joystick
    private const float DEGREES_PER_JOTSTICK_MOVE = 0.3125f;  // Joystick movement for 20 degrees
    //static float degreesPerJoystickMoveLess = 0.15625f;   // Joystick movement for 15 degrees 
    
    /* Joystick Calculations:
        Joystick moves from +64 / -64 on both x and y axis
        For y axis - fully up at 64, fully down at -64 
        On each joystick move, need to know the distance from the center, with the control surface being moved (20 degrees) 
        enough to map +20 / -20 with the +64 / -64 
        20 / 64 = 0.3125
        We need to rotate by +/-0.3125 for each joystick movement
    */
   
    // References to all of the control surfaces from the ControlSurfaces.cs Script
    // As the Objects are static, they don't need instantiated for this script to get the reference to them
    static ControlSurfaces.Rudder rudder = ControlSurfaces.rudder;
    static ControlSurfaces.Surface leftAileron = ControlSurfaces.leftAileron;
    static ControlSurfaces.Surface rightAileron = ControlSurfaces.rightAileron;
    static ControlSurfaces.Surface leftElevator = ControlSurfaces.leftElevator;
    static ControlSurfaces.Surface rightElevator = ControlSurfaces.rightElevator;
    static ControlSurfaces.Surface leftFlap = ControlSurfaces.leftFlap;
    static ControlSurfaces.Surface rightFlap = ControlSurfaces.rightFlap;

    // Store the starting rotations for each control surface
    // Required to ensure all axis remain correct other than the one angle being rotated
    static Vector3 rudderStartingRotations = rudder.GetStartingRotations();
    static Vector3 rightElevatorStartingRotations = rightElevator.GetStartingRotations();
    static Vector3 leftElevatorStartingRotations = leftElevator.GetStartingRotations();
    static Vector3 rightAileronStartingRotations = rightAileron.GetStartingRotations();
    static Vector3 leftAileronStartingRotations = leftAileron.GetStartingRotations();
    static Vector3 rightFlapStartingRotations = rightFlap.GetStartingRotations();
    static Vector3 leftFlapStartingRotations = leftFlap.GetStartingRotations();

    // Reference to the flaps control slider
    private static Slider flapsSlider = GameObject.FindGameObjectWithTag("FlapSlider").GetComponent<Slider>();

    // Reference to the throttle control slider and corresponding airspeed guage
    private static Slider throttleSlider = GameObject.FindGameObjectWithTag("ThrottleSlider").GetComponent<Slider>();
    private static RectTransform airspeedNeedle = GameObject.FindGameObjectWithTag("SpeedNeedle").GetComponent<RectTransform>();

    // const variables are set at compile time and cannot be changed. Values used to limit the throttleSliders values
    private const float SLIDER_MIN_VALUE = 1;
    private const float SLIDER_MAX_VALUE = 3;

    // Add references to the different cloud particle systems
    private static ParticleSystem clouds1 = GameObject.Find("Clouds").GetComponent<ParticleSystem>();
    private static ParticleSystem clouds2 = GameObject.Find("Clouds2").GetComponent<ParticleSystem>();
    private static ParticleSystem clouds3 = GameObject.Find("Clouds3").GetComponent<ParticleSystem>();
    // Add the particle systems into a list allowing for them to be easily looped over to edit them
    private static List<ParticleSystem> particleList = new List<ParticleSystem> { clouds1, clouds2, clouds3 };


    // Method ensures a control surfaces LocalEulerAngle is always between -180 / +180
    // In our case, this ensures the angle figure is between +/- 20 degrees as no surface moves more than that
    // When working with quaternions in the Unity Editor, values can exceed 180 degrees which causes issues and inconsistencies
    public static float WrapAngle(float angle)
    {
        angle %= 360;  // get remainder from division by 360
        if (angle > 180)  // If remainder is greater than 180, minus 360 to bring the value into the 180 range
            return angle - 360;
        return angle;
    }

    // Pedal Controls

    // Method controls what happens when a pedal is pressed:
        // The pedal image becomes smaller
        // The rudder rotates depending on the pedal pressed
    public static void PedalDown(Button pedal, float degrees) // Rotate Rudder based on pedal pressed by input type
    {
        pedal.transform.localScale = pedalSmallsize; 
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

    // Method sets the pedal size back to full showing the pedal as no longer pressed
    public static void PedalUp(Button pedal)
    {
        pedal.transform.localScale = pedalFullsize;  
    }

    // Method returns both pedals back to full size, ensuring the rudder is also rotated back to the start position
    public static void PedalBothUp()
    {
        leftPedal.transform.localScale = pedalFullsize;  // set pedals back to large
        rightPedal.transform.localScale = pedalFullsize;
        Vector3 rudderRotation = new Vector3(rudderStartingRotations.x, rudderStartingRotations.y, 0);
        rudder.Rotate(rudderRotation);
    }

    // On single click press, one pedal comes down, and the other returns up
    public static void PedalDownKeyboard(string pedal)
    {
        if(pedal == "left")
        {
           PedalDown(leftPedal, BUTTON_PRESS_DEGREES);
           PedalUp(rightPedal);
        }
        else if(pedal == "right")
        {
           PedalDown(rightPedal, BUTTON_PRESS_DEGREES);
           PedalUp(leftPedal);
        }

    }

    // Joystick Controls

    // Method rotates control surfaces mapped to the UI joystick (Elevators and Ailerons), based on its position
    public static void RotateSurfaces()
    {
        GetCurrentJoystickCoords();  // Method gets the current joystick position, storing them in the currentJoystickCoords variable
        RotateElevators();  // Method rotates the elevators based on the joystick position
        RotateAilerons();  // Method rotates the ailerons based on the jostick position
    }

   // Method reads the joysticks current position, saving it into a global variable
    private static void GetCurrentJoystickCoords()  
    {
        joystickX = joystickHandle.localPosition.x;
        joystickY = joystickHandle.localPosition.y;
        currentJoystickCoords = new Vector2(joystickX, joystickY);
    }

    // Method rotates the elevators based on the joystick position
    private static void RotateElevators()
    {
        // rotate around parent objects pivot point on the y axis to the required degrees (20 is the maximum)
        float degrees = currentJoystickCoords.y * DEGREES_PER_JOTSTICK_MOVE;
        // Left and right elevators always move opposite to one another
        var leftElevatorRotation = new Vector3(leftElevatorStartingRotations.x, degrees, leftElevatorStartingRotations.z);
        leftElevator.Rotate(leftElevatorRotation);
        var rightElevatorRotation = new Vector3(rightElevatorStartingRotations.x, -degrees, rightElevatorStartingRotations.z);
        rightElevator.Rotate(rightElevatorRotation);
    }

    private static void RotateAilerons()
    {
        // rotate around parent objects pivot point on the x axis to the required degrees (20 is the maximum)
        float degrees = currentJoystickCoords.x * DEGREES_PER_JOTSTICK_MOVE;
        // Left and right ailerons always move together in the same direction
        var leftAileronRotation = new Vector3(leftAileronStartingRotations.x, -degrees, leftAileronStartingRotations.z);
        leftAileron.Rotate(leftAileronRotation);
        var rightAileronRotation = new Vector3(rightAileronStartingRotations.x, degrees, rightAileronStartingRotations.z);
        rightAileron.Rotate(rightAileronRotation);
    }

    // Flaps Controls

    // Method moves the flapsSlider up by 1 (min value is 0, max is 2)
    public static void MoveFlapsDown() 
    {
        flapsSlider.value += 1;
    }

    // Method moves the flapsSlider down by 1 (min value is 0, max is 2)
    public static void MoveFlapsUp()  
    {
        flapsSlider.value -= 1;
    }

    // Method moves the flaps objects based on the flaps slider value
    public static void MoveFlaps()
    {
        // Flaps are only moved at low speed, so firstly ensure airspeed is low
        if (throttleSlider.value <= 1)
        {
            float degrees = flapsSlider.value * 5;  // either zero, one or two, so 0, 5 or 10 degrees change
            var flapRotation = new Vector3(leftFlapStartingRotations.x, -degrees, leftFlapStartingRotations.z);
            leftFlap.Rotate(flapRotation);
            rightFlap.Rotate(flapRotation);
        }
        else
        {
            flapsSlider.value = 0;  // Reset the flaps slider back to top
            var flapRotation = new Vector3(leftFlapStartingRotations.x, 0, leftFlapStartingRotations.z);
            leftFlap.Rotate(flapRotation);
            rightFlap.Rotate(flapRotation);
        }
    }

    // Throttle Controls

    //Method changes the amount and speed of clouds based on the throttle value
    public static void UpdateCloudSpeed()
    {
        // Setup the throttle values - cannot be done in start or awake methods as static classes can't have these
        throttleSlider.minValue = SLIDER_MIN_VALUE;  // Slider max value (top) is 3
        throttleSlider.maxValue = SLIDER_MAX_VALUE;  // Slider min value (bottom) is 1

        // Change particle system speeds based on current throttle value
        foreach (ParticleSystem cloud in particleList)  // Loop through each particle system
        {
            ParticleSystem.VelocityOverLifetimeModule velocity = cloud.velocityOverLifetime;
            velocity.speedModifier = throttleSlider.value;  // change value between 1 and 3 (possible throttle values)

            ParticleSystem.EmissionModule emission = cloud.emission;  // Rate of particles produced
            emission.rateOverTime = (throttleSlider.value / 10) * 5;  // Change rate based on throttle value
        }

        // Update the AirSpeed Needle based on throttle position
        MoveAirspeedNeedle(throttleSlider.value);
    }

    // Method maps the airspeed needle to the throttle, ensuring the guages line up in the user interface
    public static void MoveAirspeedNeedle(float sliderValue)
    {
        // 142 * the throttle value (1-3) ensures the needle follows the throttle slider movement accurately on the x axis
        float newSpeedNeedleValue = sliderValue * 142; 
        airspeedNeedle.anchoredPosition = new Vector2(airspeedNeedle.anchoredPosition.x, newSpeedNeedleValue);  
    }

    // Method allows the throttle to be contolled from a gamepad.
    // Method takes the gamepads left joystick position, moving the throttle within its bounds (1-3)
    // on the y axis based on the input of -1 - 1 on the gamepad joystick
    public static void MoveUiThrottle(Vector2 context)
    {
        Vector2 leftStickCoords = context;
        throttleSlider.value = leftStickCoords.y + 2;
    }
}


