using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchscreenMoveController : MonoBehaviour
{
    [SerializeField] DeviceServer phoneServer;

    InputAction touchDelta;
    
    private void Awake()
    {
        var phone = phoneServer.InputActions.Phone;
        
    }
}
