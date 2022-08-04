using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    [SerializeField] PhoneServer phoneServer;
    [SerializeField] float moveScale = 0.01f;
    [SerializeField] float jumpScale = 0.3f;
    [SerializeField] ForceMode jumpForceMode;
    [SerializeField] Rigidbody rBody;
    InputAction moveAction, jumpAction;
    Camera cam;
    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        var input = phoneServer.InputActions;
        var phone = input.Phone;
        cam = Camera.main;

        moveAction = phone.Move;
        jumpAction = phone.Jump;
        jumpAction.performed += (ctx) =>
        {
            var jump = new Vector3(0, 1, 0);
            rBody.AddForce(jumpScale * jump, jumpForceMode);
        };
    }

    private void OnEnable() => jumpAction.Enable();
    private void OnDisable() => jumpAction.Disable();

    private void Update()
    {
        Vector3 stick = moveAction.ReadValue<Vector2>();
        stick = new Vector3(stick.x, 0, stick.y);
        var pos = cam.transform.rotation * (moveScale * stick);
        if (pos == Vector3.zero)
            return;

        rBody.MovePosition(rBody.position + pos);
    }
}
