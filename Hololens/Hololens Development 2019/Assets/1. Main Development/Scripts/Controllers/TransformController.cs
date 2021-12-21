using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransformController : MonoBehaviour
{
    [SerializeField] float moveScale = 1;
    InputActions inputs;
    [SerializeField] Vector3 attitudeOffset;

    [SerializeField] Vector3 acceleration;
    [SerializeField] Vector3 linearAcceleration;
    [SerializeField] Vector3 attitude;

    Vector3 ogPosition;
    private Quaternion attitudeOffsetQuat;
    private void Awake()
    {
        ogPosition = transform.position;
        attitudeOffsetQuat = Quaternion.Euler(attitudeOffset);
        inputs = new InputActions();
        inputs.Enable();
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
        var actions = inputs.Player;
        acceleration = actions.Acceleration.ReadValue<Vector3>();
        linearAcceleration = actions.LinearAcceleration.ReadValue<Vector3>();
        var attitudeQuat = actions.Attitude.ReadValue<Quaternion>();
        attitude = attitudeQuat.eulerAngles;

        transform.rotation = Quaternion.Euler(attitudeOffset) * attitudeQuat;
    }
    public void Move(Vector2 inputMovement)
    {
        var moveVector = moveScale * new Vector3(inputMovement.x, 0, inputMovement.y);
        transform.position += moveVector;
    }
}
