using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public class DeviceActionEvent
{
    [SerializeField, HideInInspector] private string actionName;
    [SerializeField, HideInInspector] private string fullActionName;
    [SerializeField, HideInInspector] private string actionId;
    [SerializeField] UnityEvent<InputAction.CallbackContext> unityEvent; // this could get replaced with something better but leaving it to default unity for now

    Guid guid;
    Action<InputAction.CallbackContext> cEvent;

    public event Action<InputAction.CallbackContext> Callback { add => cEvent += value;  remove => cEvent -= value; }
    public string Name => actionName;
    public string ActionId => actionId;
    public string FullActionName => fullActionName;
    public DeviceActionEvent()
    {
    }

    public DeviceActionEvent(InputAction action) => ApplyActionInfo(action);
    public DeviceActionEvent(Guid actionGUID, string name = null)
    {
        actionId = actionGUID.ToString();
        this.fullActionName = name;
    }

    public void ApplyActionInfo(InputAction action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));
        //if (action.isSingletonAction) //cant use this its internal
        //throw new ArgumentException($"Action must be part of an asset (given action '{action}' is a singleton)");
        if (action.actionMap.asset == null)
            throw new ArgumentException($"Action must be part of an asset (given action '{action}' is not)");

        guid = action.id;
        actionId = guid.ToString();
        var actionName = action.name;
        this.actionName = actionName;
        fullActionName = $"{action.actionMap.name}/{actionName}";
    }

    public bool Connect(InputActionMap map, bool useName = false)
    {
        var action = useName ? map.FindAction(actionName) : map.FindAction(guid);
        if (action == null) 
            return false;

        action.performed += Invoke;
        action.started += Invoke;
        action.canceled += Invoke;
        return true;
    }
    public bool Disconnect(InputActionMap map, bool useName = false)
    {
        var action = useName ? map.FindAction(actionName) : map.FindAction(guid);
        if (action == null) 
            return false;

        action.performed -= Invoke;
        action.started -= Invoke;
        action.canceled -= Invoke;
        return true;
    }

    private void Invoke(InputAction.CallbackContext ctx)
    {
        unityEvent?.Invoke(ctx);
        cEvent?.Invoke(ctx);
    }

}
