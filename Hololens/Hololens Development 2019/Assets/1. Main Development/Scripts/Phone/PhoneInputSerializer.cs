using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using System.IO;
public class PhoneInputSerializer : MonoBehaviour
{
    Accelerometer accelerometer;
    GravitySensor gravitySensor;
    LinearAccelerationSensor linearAccelerationSensor;
    UnityEngine.InputSystem.Gyroscope gyro;

    InputEventTrace eventTrace;
    Func<InputEventPtr, InputDevice, bool> filter;
    private void Awake()
    {
        
    }
    private void OnEnable()
    {   
        accelerometer = Accelerometer.current;
        gyro = UnityEngine.InputSystem.Gyroscope.current;
        gravitySensor = GravitySensor.current;
        linearAccelerationSensor = LinearAccelerationSensor.current;
        
        EnableDevice(accelerometer);
        EnableDevice(gyro);
        EnableDevice(gravitySensor);
        EnableDevice(linearAccelerationSensor);

        Debug.Log(Gamepad.current.deviceId);
        eventTrace = new InputEventTrace(device: Gamepad.current, growBuffer: true);
        eventTrace.deviceId = Gamepad.current.deviceId;
        eventTrace.Enable();
    }
    
    private void OnDisable()
    {
        DisableDevice(accelerometer);
        DisableDevice(gyro);
        DisableDevice(gravitySensor);
        DisableDevice(linearAccelerationSensor);

        eventTrace.Dispose();
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

    public byte[] GetEventsAsBytes()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            eventTrace.WriteTo(stream);
            var result = stream.ToArray();
            eventTrace.Dispose();
            eventTrace = new InputEventTrace(device: Gamepad.current, growBuffer: true);
            eventTrace.deviceId = Gamepad.current.deviceId;
            eventTrace.Enable();
            return result;
        }
    }

    private Vector3 ReadValue(Vector3Control control) => control == null ? Vector3.zero : control.ReadValue();

    private Vector2 ReadValue(Vector2Control control) => control == null ? Vector2.zero : control.ReadValue();
}
