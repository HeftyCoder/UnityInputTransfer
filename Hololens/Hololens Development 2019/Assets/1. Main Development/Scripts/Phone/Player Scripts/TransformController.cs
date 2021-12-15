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
        var input = movementControl.ReadValue<Vector2>();
        Move(input);
    }
    public void Move(Vector2 inputMovement)
    {
        var moveVector = moveScale * new Vector3(inputMovement.x, 0, inputMovement.y);
        transform.position += moveVector;
    }
}
