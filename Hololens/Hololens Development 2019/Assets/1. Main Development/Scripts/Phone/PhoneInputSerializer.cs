using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PhoneInputSerializer : MonoBehaviour
{
    Touchscreen touchScreen;
    Accelerometer accelerometer;
    GravitySensor gravitySensor;
    LinearAccelerationSensor linearAccelerationSensor;
    UnityEngine.InputSystem.Gyroscope gyro;    

    private void OnEnable()
    {
        accelerometer = Accelerometer.current;
        gyro = UnityEngine.InputSystem.Gyroscope.current;
        gravitySensor = GravitySensor.current;
        linearAccelerationSensor = LinearAccelerationSensor.current;
        touchScreen = Touchscreen.current;

        InputSystem.EnableDevice(accelerometer);
        InputSystem.EnableDevice(gyro);
        InputSystem.EnableDevice(gravitySensor);
        InputSystem.EnableDevice(linearAccelerationSensor);
        InputSystem.EnableDevice(touchScreen);
    }

    private void OnDisable()
    {
        InputSystem.DisableDevice(accelerometer);
        InputSystem.DisableDevice(gyro);
        InputSystem.DisableDevice(gravitySensor);
        InputSystem.DisableDevice(linearAccelerationSensor);
        InputSystem.DisableDevice(touchScreen);


    }

    public PhoneInputSerialization GetInputSerialization()
    {
        var acceleration = accelerometer.acceleration.ReadValue();
        var angularVel = gyro.angularVelocity.ReadValue();
        var gravity = gravitySensor.gravity.ReadValue();
        var linearAcc = linearAccelerationSensor.acceleration.ReadValue();

        var primaryTouch = touchScreen.primaryTouch;
        var delta = primaryTouch.delta.ReadValue();
        var tapCount = primaryTouch.tapCount.ReadValue();

        return new PhoneInputSerialization(acceleration, angularVel, gravity, linearAcc, delta, tapCount);
    }

}
