using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
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

        EnableDevice(accelerometer);
        EnableDevice(gyro);
        EnableDevice(gravitySensor);
        EnableDevice(linearAccelerationSensor);
        EnableDevice(touchScreen);
    }
    
    private void OnDisable()
    {
        DisableDevice(accelerometer);
        DisableDevice(gyro);
        DisableDevice(gravitySensor);
        DisableDevice(linearAccelerationSensor);
        DisableDevice(touchScreen);
    }
    private void EnableDevice(InputDevice device)
    {
        if (device != null)
            InputSystem.EnableDevice(device);
    }
    private void DisableDevice(InputDevice device)
    {
        if (device != null)
            InputSystem.DisableDevice(device);
    }

    public PhoneInputSerialization GetInputSerialization()
    {
        var acceleration = ReadValue(accelerometer?.acceleration);
        var angularVel = ReadValue(gyro?.angularVelocity);
        var gravity = ReadValue(gravitySensor?.gravity);
        var linearAcc = ReadValue(linearAccelerationSensor?.acceleration);

        var primaryTouch = touchScreen?.primaryTouch;
        var delta = ReadValue(primaryTouch?.delta);

        var tapCount = primaryTouch?.tapCount == null ? 0 : primaryTouch.tapCount.ReadValue();

        return new PhoneInputSerialization(acceleration, angularVel, gravity, linearAcc, delta, tapCount);
    }

    private Vector3 ReadValue(Vector3Control control) => control == null ? Vector3.zero : control.ReadValue();

    private Vector2 ReadValue(Vector2Control control) => control == null ? Vector2.zero : control.ReadValue();
}
