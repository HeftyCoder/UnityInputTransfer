using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadController : MonoBehaviour
{
    [SerializeField] PhoneServer phoneServer;

    InputAction leftPadInput, rightPadInput;
    private void Awake()
    {
        var input = phoneServer.InputActions;
        var phone = input.Phone;

        leftPadInput = phone.Move;
    }

    private void Update()
    {
        var camera = Camera.main;
        var pos = leftPadInput.ReadValue<Vector3>();

        if (pos == Vector3.zero)
            return;
        //Transform pos by camera view
        pos = camera.transform.rotation * pos;
        transform.position = pos;
    }
}
