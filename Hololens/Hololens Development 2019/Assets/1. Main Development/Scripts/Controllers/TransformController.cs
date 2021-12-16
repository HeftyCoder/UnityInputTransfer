using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransformController : MonoBehaviour
{
    [SerializeField] float moveScale = 1;
    InputActions inputs;

    private void Awake()
    {
        inputs = new InputActions();
        inputs.Enable();
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
        var movementControl = inputs.Player.Move;
        var movementInput = movementControl.ReadValue<Vector2>();
        Move(movementInput);

        var angVel = inputs.Player.AngVelocity.ReadValue<Vector3>();

        var rotEuler = transform.rotation.eulerAngles;

        var newOrientation = rotEuler + (180 / Mathf.PI) * angVel * Time.deltaTime;
        transform.rotation = Quaternion.Euler(newOrientation);
    }
    public void Move(Vector2 inputMovement)
    {
        var moveVector = moveScale * new Vector3(inputMovement.x, 0, inputMovement.y);
        transform.position += moveVector;
    }
}
