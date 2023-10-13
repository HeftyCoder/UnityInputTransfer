using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class DeviceActionMap 
{
    [SerializeField, HideInInspector] string name;
    [SerializeField] List<DeviceActionEvent> actions = new List<DeviceActionEvent>();
    public IReadOnlyList<DeviceActionEvent> Actions => actions;
    public string Name => name;
    public void InitializeActions(InputActionMap actionMap)
    {
        name = actionMap.name;
        actions.Clear();
        foreach (var action in actionMap.actions)
        {
            var dAction = new DeviceActionEvent(action);
            actions.Add(dAction);
        }
    }
}
