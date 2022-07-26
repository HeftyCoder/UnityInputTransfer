using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class VirtualPhone : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text status;
    [SerializeField] ArucoTracker arucoTracker;
    [SerializeField] PhoneServer phoneServer;

    InputActions inputs;

    public bool useInitials; //useful in the editor
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    [Header("VIO Reported")]
    public Vector3 vioPosition;
    public Quaternion vioRotation;

    [Header("Marker and At Marker Vio")]
    public Marker lastMarker;
    public Quaternion markedInverseVioRotation;
    private Vector3 tHP = Vector3.zero;
    
    private void Awake()
    {
        inputs = new InputActions();
        var actions = inputs.Player;

        lastMarker = Marker.Identity;

        //With these set, we can work in the editor as well
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        vioRotation = Quaternion.identity;
        markedInverseVioRotation = Quaternion.identity;

        //Reading the values that the tracker has for us
        actions.PhonePosition.performed += (ctx) => vioPosition = ctx.ReadValue<Vector3>();
        actions.PhoneRotation.performed += (ctx) => vioRotation = ctx.ReadValue<Quaternion>();
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
        vioPosition = actions.PhonePosition.ReadValue<Vector3>();
        vioRotation = actions.PhoneRotation.ReadValue<Quaternion>();

        //Finding rotation and translation 
        markedInverseVioRotation = Quaternion.Inverse(vioRotation);
        tHP = lastMarker.position - vioPosition;
}
    private void Update()
    {
        EnsureDevicesSet();

        var rot = (markedInverseVioRotation * vioRotation) * lastMarker.rotation;
        var pos = tHP + vioPosition;
        if (useInitials)
        {
            rot *= initialRotation;
            pos += initialPosition;
        }
        transform.SetPositionAndRotation(pos, rot);
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
