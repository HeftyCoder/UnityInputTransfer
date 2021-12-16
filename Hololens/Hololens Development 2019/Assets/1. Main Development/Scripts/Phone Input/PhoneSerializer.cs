using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Layouts;
using System.IO;
public class PhoneSerializer : MonoBehaviour
{
    [SerializeField] bool controlSensorActivity;
    [InputControl(layout = "Sensor")]
    [SerializeField] string[] enabledSensorsAtStart;
    InputEventTrace eventTrace;
    Dictionary<int, DeviceData> deviceChanges = new Dictionary<int, DeviceData>();

    private PhoneServer localServer;

    private void Awake()
    {
        localServer = GetComponent<PhoneServer>();
    }
    private void Start()
    {
        EnableSensors(true);
    }
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;

        eventTrace = new InputEventTrace(growBuffer: true);
        eventTrace.Enable();
        EnableSensors(true);
    }

    private void OnDisable()
    {
        EnableSensors(false);
        InputSystem.onDeviceChange -= OnDeviceChange;
        eventTrace.Dispose();
    }
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        //Used in order to avoid conflicts with the server - Might be useless
        if (localServer != null && localServer.CreatedDevices.Contains(device))
        {
            return;
        }

        Debug.Log($"{device.name} {change}");
        switch (change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Removed:
                var deviceData = new DeviceData(device, change);
                deviceChanges[device.deviceId] = deviceData;
                break;
        }
        
    }

    public void ResolveLocalServerDeviceConflict(InputDevice device)
    {
        deviceChanges.Remove(device.deviceId);
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
        //Get the changes before clearing the dictionary. If you directly use deviceChanges.Values, it will be cleared before being sent
        var changes = new List<DeviceData>(deviceChanges.Values);
        deviceChanges.Clear();

        data.Reset(changes, bytes);
    }

    private void EnableSensors(bool enable)
    {
        if (!controlSensorActivity)
            return;
        for (int i = 0; i < enabledSensorsAtStart.Length; i++)
        {
            var path = enabledSensorsAtStart[i];
            var control = InputSystem.FindControl(path);
            if (control == null)
                continue;
            var device = control.device;
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
