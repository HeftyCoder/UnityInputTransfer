using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceServerInputControl : MonoBehaviour
{
    [SerializeField] InputActionAsset inputAsset;
    [SerializeField] List<DeviceActionMap> actionMaps = new List<DeviceActionMap>();
    public InputActionAsset InputAsset => inputAsset;

    private void OnValidate()
    {
        if (inputAsset != null && actionMaps.Count == 0)
            SelfInitialize();
    }
    [ContextMenu("Self Initialize")]
    public void SelfInitialize()
    {
        if (inputAsset != null)
        {
            Initialize(inputAsset);
        }
    }
    public void Initialize(InputActionAsset inputAsset)
    {
        actionMaps.Clear();
        foreach (var map in inputAsset.actionMaps)
        {
            var dMap = new DeviceActionMap();
            dMap.InitializeActions(map);
            actionMaps.Add(dMap);
        }
    }
    public void Connect(InputActionAsset inputAsset)
    {
        foreach (var map in actionMaps)
        {
            var inputMap = inputAsset.FindActionMap(map.Name);
            if (inputMap == null)
            {
                Debug.LogWarning($"Missing map:{map.Name} for {inputAsset}", inputAsset);
                continue;
            }

            foreach (var action in map.Actions)
            {
                var result = action.Connect(inputMap, true);
            }
        }
    }
    public void Disconnect(InputActionAsset inputAsset)
    {
        foreach (var map in actionMaps)
        {
            var inputMap = inputAsset.FindActionMap(map.Name);
            if (inputMap == null)
            {
                Debug.LogWarning($"Missing map:{map.Name} for {inputAsset}", inputAsset);
                continue;
            }

            foreach (var action in map.Actions)
            {
                action.Disconnect(inputMap, true);
            }
        }
    }
}
