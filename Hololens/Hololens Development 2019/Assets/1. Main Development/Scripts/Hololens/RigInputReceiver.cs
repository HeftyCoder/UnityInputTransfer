using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System.IO;

[RequireComponent(typeof(Rigidbody))]
public class RigInputReceiver : MonoBehaviour
{
    [SerializeField] float angularScale = 1.5f;
    [SerializeField] float accelerationScale = 1.5f;
    HololensPhoneServer hololensServer;
    Rigidbody rigidBody;

    //HACKY way to avoid the random order in which Start, Enable is invoked. 
    //To avoid not having a server instance
    private bool firstInitialization = true;
    private void Awake()
    {
    }

    private void OnEnable()
    {
        hololensServer = HololensPhoneServer.Instance;
        hololensServer.onPhoneInput += HandleInput;
    }
    private void OnDisable()
    {
        hololensServer.onPhoneInput -= HandleInput;
    }

    private void HandleInput(PhoneInputSerialization input)
    {
        rigidBody.AddRelativeTorque(angularScale * input.AngularVelocity, ForceMode.Impulse);
        rigidBody.AddRelativeForce(input.LinearAcceleration * accelerationScale, ForceMode.Impulse);
    }
}
