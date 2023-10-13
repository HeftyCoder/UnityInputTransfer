using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAttitudeController : MonoBehaviour
{
    private void Awake()
    {

    }

    private void OnEnable() { }
    public void DoAttitude(InputAction.CallbackContext ctx)
    {
        if (!enabled)
            return;
        var rotation = ctx.ReadValue<Quaternion>();
        transform.rotation = rotation;
    }
}
