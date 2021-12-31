using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransformController : MonoBehaviour
{
    [SerializeField] float moveScale = 1;
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
    
    private void FixedUpdate()
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
    }
    public void Move(Vector2 inputMovement)
    {
        var moveVector = moveScale * new Vector3(inputMovement.x, 0, inputMovement.y);
        transform.position += moveVector;
    }

    private void ResetValues()
    {
        position = ogPosition;
        velocity = Vector3.zero;
    }
}
