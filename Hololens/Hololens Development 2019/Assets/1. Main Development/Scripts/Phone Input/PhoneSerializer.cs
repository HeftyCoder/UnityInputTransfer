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
    [InputControl(layout = "Sensor")]
    [SerializeField] string[] sensorsToConnect;

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
        EnableSensors(true);
    }

    private void OnDisable()
    {
        EnableSensors(false);
        InputSystem.onDeviceChange -= OnDeviceChange;
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

        var bytes = stream.ToArray();
        //Get the changes before clearing the dictionary. If you directly use deviceChanges.Values, it will be cleared before being sent
        var changes = new List<DeviceData>(deviceChanges.Values);
        deviceChanges.Clear();

        data.Reset(changes, bytes);
    }

    private void EnableSensors(bool enable)
    {
        for (int i = 0; i < sensorsToConnect.Length; i++)
        {
            var path = sensorsToConnect[i];
            var control = InputSystem.FindControl(path);
            if (control == null)
                continue;
            var device = control.device;

            if (device != null && !string.IsNullOrEmpty(Utilities.GetSupportedDeviceLayout(device)))
            {
                if (enable)
                    InputSystem.EnableDevice(device);
                else
                    InputSystem.DisableDevice(device);
            }
        }
    }

}
