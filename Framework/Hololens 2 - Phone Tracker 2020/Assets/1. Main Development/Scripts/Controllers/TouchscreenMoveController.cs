using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchscreenMoveController : MonoBehaviour
{
    [SerializeField] DeviceServer phoneServer;

    InputActions inputActions;
    InputAction moveScreen;
    private void Awake()
    {
        inputActions = phoneServer.InputActions;
        moveScreen = inputActions.Phone.MoveTouch;

    }

    private void OnEnable() => moveScreen.performed += ReadPosition;
    private void OnDisable() => moveScreen.performed -= ReadPosition;
    private void ReadPosition(InputAction.CallbackContext ctx)
    {
        var move = ctx.ReadValue<Vector2>();
        transform.position += (Vector3)move;
    }
}
