using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAttitudeController : MonoBehaviour
{
    public DeviceServer server;
    InputActions inputActions;
    InputAction attitude;
    private void Awake()
    {
        inputActions = server.InputActions;
        attitude = inputActions.Phone.Attitude;
    }

    private void OnEnable() => attitude.performed += DoAttitude;
    private void OnDisable() => attitude.performed -= DoAttitude;

    private void DoAttitude(InputAction.CallbackContext ctx)
    {
        var rotation = ctx.ReadValue<Quaternion>();
        transform.rotation = rotation;
    }
}
