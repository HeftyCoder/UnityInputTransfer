using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchscreenMoveController : MonoBehaviour
{
    private void Awake()
    {
        
    }

    private void OnEnable() { }
    public void ReadPosition(InputAction.CallbackContext ctx)
    {
        if (!enabled)
            return;
        var move = ctx.ReadValue<Vector2>();
        transform.position += (Vector3)move;
    }
}
