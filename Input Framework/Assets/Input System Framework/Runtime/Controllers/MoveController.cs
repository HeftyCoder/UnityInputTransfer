using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    [SerializeField] DeviceServer phoneServer;
    [SerializeField] string moveActionPath;
    [SerializeField] float moveScale = 0.01f;
    [SerializeField] float jumpScale = 0.3f;
    [SerializeField] ForceMode jumpForceMode;
    [SerializeField] Rigidbody rBody;
    InputAction moveAction;
    Camera cam;
    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        cam = Camera.main;
    }
    private void Start()
    {
        moveAction = phoneServer.ActionAssetInstance.FindAction(moveActionPath);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Move(ctx.ReadValue<Vector2>());
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!enabled)
            return;
        var jump = new Vector3(0, 1, 0);
        rBody.AddForce(jumpScale * jump, jumpForceMode);
    }
    private void Update()
    {
        Move(moveAction.ReadValue<Vector2>());
    }
    private void Move(Vector3 stick)
    {
        if (!enabled)
            return;
        stick = new Vector3(stick.x, 0, stick.y);
        var pos = cam.transform.rotation * (moveScale * stick);
        if (pos == Vector3.zero)
            return;
        rBody.MovePosition(rBody.position + pos);
    }
}
