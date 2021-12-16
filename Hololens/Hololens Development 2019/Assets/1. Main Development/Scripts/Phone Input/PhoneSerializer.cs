using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using System.IO;
public class PhoneSerializer : MonoBehaviour
{
    [SerializeField] SensorType[] sensors;
    InputEventTrace eventTrace;
    Dictionary<int, DeviceData> deviceChanges = new Dictionary<int, DeviceData>();

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;

        eventTrace = new InputEventTrace(growBuffer: true);
        eventTrace.Enable();
        EnableDevices(true);
    }

    private void OnDisable()
    {
        EnableDevices(false);
        InputSystem.onDeviceChange -= OnDeviceChange;
        eventTrace.Dispose();
    }
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Removed:
                var deviceData = new DeviceData(device, change);
                deviceChanges[device.deviceId] = deviceData;
                break;
        }
        
    }

    public PhoneData GetPhoneData()
    {
        var data = new PhoneData();
        FillPhoneData(data);
        return data;
    }
    public void FillPhoneData(PhoneData data)
    {
        //This specific class does not need to call dispose - otherwise, use using statement
        var stream = new MemoryStream();
        eventTrace.WriteTo(stream);
        eventTrace.Clear();

        var bytes = stream.ToArray();
        var changes = deviceChanges.Values;
        deviceChanges.Clear();

        data.Reset(changes, bytes);
    }

    private void EnableDevices(bool enable)
    {
        for (int i = 0; i < sensors.Length; i++)
        {
            var className = sensors[i].sensorType;
            var device = InputSystem.GetDevice(className);
            
            if (device != null)
            {
                if (enable)
                    InputSystem.EnableDevice(device);
                else
                    InputSystem.DisableDevice(device);
            }
        }
    }

}
