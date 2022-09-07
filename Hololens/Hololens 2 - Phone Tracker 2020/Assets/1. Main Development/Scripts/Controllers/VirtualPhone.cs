using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class VirtualPhone : MonoBehaviour
{
    [SerializeField] ArucoTracker arucoTracker;
    [SerializeField] PhoneServer phoneServer;

    InputActions inputs => phoneServer.InputActions;

    [Header("VIO Reported")]
    public Vector3 posVio;
    public Quaternion rotVio;

    [Header("Marker and At Marker Vio")]
    public Vector3 posMarker;
    public Quaternion rotMarker;

    private Quaternion R = Quaternion.identity;
    private Vector3 T = Vector3.zero;

    private InputAction phonePositionInput, phoneRotationInput;

    [ContextMenu("Test Calculate RT")]
    private void TestInEditor()
    {
        CalculateRT(posVio, rotVio, posMarker, rotMarker);
    }
    private void Awake()
    {
        var actions = inputs.Phone;
        posMarker = transform.position;
        rotMarker = transform.rotation;

        posVio = transform.position;
        rotVio = transform.rotation;

        //Reading the values that the tracker has for us
        phonePositionInput = actions.PhonePosition;
        phoneRotationInput = actions.PhoneRotation;
    }
    private void OnEnable()
    {
        phonePositionInput.performed += ReadPosition;
        phoneRotationInput.performed += ReadRotation;
        arucoTracker.onDetectionFinished += OnArucoScanFinished;
    }
    private void OnDisable()
    {
        phonePositionInput.performed -= ReadPosition;
        phoneRotationInput.performed -= ReadRotation;
        arucoTracker.onDetectionFinished -= OnArucoScanFinished;
    }
    private void ReadPosition(InputAction.CallbackContext ctx)
    {
        posVio = ctx.ReadValue<Vector3>();
        Debug.Log("Reading position at : " +Time.frameCount);
    }
    private void ReadRotation(InputAction.CallbackContext ctx)
    {
        rotVio = ctx.ReadValue<Quaternion>();
        Debug.Log("Reading rotation at : " + Time.frameCount);
    }

    private void OnArucoScanFinished(IReadOnlyList<Marker> markers)
    {
        if (markers.Count == 0)
            return;
        //We're treating this as if it was a board
        var marker = markers[0];
        posMarker = marker.position;
        rotMarker = marker.rotation;

        transform.SetPositionAndRotation(marker.position, marker.rotation);
        var actions = inputs.Phone;

        //Values reported from VIO at the moment of aruco detection
        posVio = actions.PhonePosition.ReadValue<Vector3>();
        rotVio = actions.PhoneRotation.ReadValue<Quaternion>();

        CalculateRT(posVio, rotVio, marker.position, marker.rotation);
    }

    private void CalculateRT(Vector3 posVio, Quaternion rotVio, Vector3 posMarker, Quaternion rotMarker)
    {
        R = Quaternion.Inverse(rotVio) * rotMarker;
        T = posMarker - (R * posVio);
    }
    private void Update()
    {
        var rot = R * rotVio;
        var pos = T + R * posVio;

        transform.SetPositionAndRotation(pos, rot);

    }
}
