using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransformController : MonoBehaviour
{
    [SerializeField] PhoneServer server;
    InputActions inputs;

    [SerializeField] Vector3 unityOffset;
    [SerializeField] Vector3 attitudeOffset;

    [SerializeField] Vector3 acceleration;
    [SerializeField] Vector3 linearAcceleration;
    [SerializeField] Vector3 attitude;

    [SerializeField] Vector3 velocity, position;

    Vector3 ogPosition;

    [SerializeField] Vector3 accOffset;
    [SerializeField] float sampleTime = 10000;
    [SerializeField] float startTime = 0;
    [SerializeField] int count = 0;
    [SerializeField] float accMagnitude;
    private void Awake()
    {
        ogPosition = transform.position;
        inputs = new InputActions();
        inputs.Enable();
    }
    private void Start()
    {
        InvokeRepeating("ResetValues", 0, 5f);
    }
    [ContextMenu("Reset Controller")]
    public void ResetController()
    {
    
    }
    private void OnEnable()
    {
        inputs.Enable();
    }
    private void OnDisable()
    {
        inputs.Disable();
    }

    private void Update()
    {
        //Temporary
        if (server.haveDevicesChanged)
        {
            var devices = server.CreatedDevices;
            var array = new InputDevice[devices.Count];
            var i = 0;
            foreach (var device in devices)
            {
                array[i] = device;
                i++;
            }
            inputs.devices = new UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputDevice>(array);
            server.haveDevicesChanged = false;
        }

        var actions = inputs.Player;
        var position = actions.PhonePosition.ReadValue<Vector3>();
        position += unityOffset;
        var rotation = actions.PhoneRotation.ReadValue<Quaternion>();

        transform.position = position;
        transform.rotation = rotation;
    }
    //Trying without AR Core
    /*private void FixedUpdate()
    {
        var actions = inputs.Player;
        
        //Attitude
        var attitudeQuat = actions.Attitude.ReadValue<Quaternion>();
        attitude = attitudeQuat.eulerAngles;
        var rotation = Quaternion.Euler(unityOffset) * Quaternion.Inverse(Quaternion.Euler(attitudeOffset)) * attitudeQuat;
        transform.rotation = rotation;

        acceleration = actions.Acceleration.ReadValue<Vector3>();
        
        //Linear Acceleration
        var newLinearAcceleration = moveScale * actions.LinearAcceleration.ReadValue<Vector3>();
        accMagnitude = linearAcceleration.magnitude;

        if (startTime <= sampleTime)
        {
            accOffset += newLinearAcceleration;
            count++;
            startTime += Time.fixedDeltaTime;
            return;
        }

        if (count != 0 && startTime > sampleTime)
        {
            accOffset /= count;
            count = 0;
        }

        newLinearAcceleration -= accOffset;

        var dt = Time.fixedDeltaTime;
        var newVelocity = velocity + (linearAcceleration + (newLinearAcceleration - linearAcceleration) * 0.5f) * dt;
        var newPosition = position + (velocity + (newVelocity - velocity) * 0.5f) * dt;
        
        transform.position = Quaternion.Euler(unityOffset) * newPosition;

        position = newPosition;
        velocity = newVelocity;
        linearAcceleration = newLinearAcceleration;
    }*/

    private void ResetValues()
    {
        position = ogPosition;
        velocity = Vector3.zero;
    }
}
