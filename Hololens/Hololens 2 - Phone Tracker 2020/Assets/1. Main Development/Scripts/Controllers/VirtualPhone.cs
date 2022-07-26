using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class VirtualPhone : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text status;
    [SerializeField] ArucoTracker arucoTracker;
    [SerializeField] PhoneServer phoneServer;

    InputActions inputs => phoneServer.InputActions;

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

    private InputAction phonePosition, phoneRotation;
    private void Awake()
    {
        var actions = inputs.Phone;
        lastMarker = Marker.Identity;

        //With these set, we can work in the editor as well
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        vioRotation = Quaternion.identity;
        markedInverseVioRotation = Quaternion.identity;

        //Reading the values that the tracker has for us
        phonePosition = actions.PhonePosition;
        phoneRotation = actions.PhoneRotation;
    }
    private void OnEnable()
    {
        phonePosition.performed += ReadPosition;
        phoneRotation.performed += ReadRotation;
        arucoTracker.onDetectionFinished += OnArucoScanFinished;
    }
    private void OnDisable()
    {
        phonePosition.performed -= ReadPosition;
        phoneRotation.performed -= ReadRotation;
        arucoTracker.onDetectionFinished -= OnArucoScanFinished;
    }
    private void ReadPosition(InputAction.CallbackContext ctx) => vioPosition = ctx.ReadValue<Vector3>();
    private void ReadRotation(InputAction.CallbackContext ctx) => vioRotation = ctx.ReadValue<Quaternion>();

    private void OnArucoScanFinished(IReadOnlyList<Marker> markers)
    {
        if (markers.Count == 0)
            return;
        //We're treating this as if it was a board
        lastMarker = markers[0];
        transform.SetPositionAndRotation(lastMarker.position, lastMarker.rotation);
        var actions = inputs.Phone;

        //Values reported from VIO at the moment of aruco detection
        vioPosition = actions.PhonePosition.ReadValue<Vector3>();
        vioRotation = actions.PhoneRotation.ReadValue<Quaternion>();

        //Finding rotation and translation 
        markedInverseVioRotation = Quaternion.Inverse(vioRotation);
        tHP = lastMarker.position - vioPosition;
}
    private void Update()
    {
        var rot = (markedInverseVioRotation * vioRotation) * lastMarker.rotation;
        var pos = tHP + vioPosition;
        if (useInitials)
        {
            rot *= initialRotation;
            pos += initialPosition;
        }
        status.text = rot.eulerAngles.ToString();
        transform.SetPositionAndRotation(pos, rot);
    }
}
