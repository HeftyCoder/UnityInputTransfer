using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class VirtualPhone : MonoBehaviour
{
    [SerializeField] ArucoTracker arucoTracker;
    [SerializeField] PhoneServer phoneServer;

    InputActions inputs;

    public Vector3 vioPosition, vioRotation;
    public Vector3 markedVioPos, markedVioRot;
    public Marker lastMarker;
    private void Awake()
    {
        inputs = new InputActions();
        var actions = inputs.Player;
        
        //Reading the values that the tracker has for us
        actions.PhonePosition.performed += (ctx) => vioPosition = ctx.ReadValue<Vector3>();
        actions.PhoneRotation.performed += (ctx) => vioRotation = ctx.ReadValue<Quaternion>().eulerAngles;
    }
    private void OnEnable()
    {
        inputs.Enable();
        arucoTracker.onDetectionFinished += OnArucoScanFinished;
    }
    private void OnDisable()
    {
        inputs.Disable();
        arucoTracker.onDetectionFinished -= OnArucoScanFinished;
    }

    private void OnArucoScanFinished(IReadOnlyList<Marker> markers)
    {
        if (markers.Count == 0)
            return;
        //We're treating this as if it was a board
        lastMarker = markers[0];
        transform.SetPositionAndRotation(lastMarker.position, lastMarker.rotation);
        var actions = inputs.Player;

        //Values reported from VIO at the moment of aruco detection
        markedVioPos = actions.PhonePosition.ReadValue<Vector3>();
        markedVioRot = actions.PhoneRotation.ReadValue<Quaternion>().eulerAngles;
    }
    private void Update()
    {
        EnsureDevicesSet();
        transform.SetPositionAndRotation(vioPosition, Quaternion.Euler(vioRotation));
    }
    private void EnsureDevicesSet()
    {
        var server = phoneServer;
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
            Awake();
            if (enabled)
                inputs.Enable();
            else
                inputs.Disable();
        }
    }
}
