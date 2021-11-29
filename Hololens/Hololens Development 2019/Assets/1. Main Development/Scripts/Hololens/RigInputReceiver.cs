using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigInputReceiver : MonoBehaviour
{
    [SerializeField] float angularScale = 1.5f;
    HololensPhoneServer hololensServer;
    Rigidbody rigidBody;

    //HACKY way to avoid the random order in which Start, Enable is invoked. 
    //To avoid not having a server instance
    private bool firstInitialization = true;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
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
        rigidBody.AddTorque(angularScale * input.AngularVelocity);
    }
}
