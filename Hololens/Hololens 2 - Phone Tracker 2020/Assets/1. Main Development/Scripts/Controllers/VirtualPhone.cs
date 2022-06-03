using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Vuforia;
public class VirtualPhone : MonoBehaviour
{
    [SerializeField] PhoneServer phoneServer;
    [SerializeField] ImageTargetBehaviour imageTarget;
    [SerializeField] Transform imageContainer;

    [SerializeField] Vector3 rotationOffset;
    [SerializeField] bool alterPositionAxis = true;
    InputActions inputs;

    [SerializeField] float positionScaleFactor = 10;
    Status imageTrackingStatus = Status.NO_POSE;
    public Vector3 onImageLostDevicePos = Vector3.zero,
            onImageLostDeviceRot = Vector3.zero,
            lastImageTrackedPosition, lastImageTrackedRotation;
            
    private void Awake()
    {
        inputs = new InputActions();
        var actions = inputs.Player;
        var positionAction = actions.PhonePosition;
        var rotationAction = actions.PhoneRotation;

        //Image target
        imageTarget.OnTargetStatusChanged += (observer, statusStruct) =>
        {
            var status = statusStruct.Status;
            switch (status)
            {
                case Status.NO_POSE:
                case Status.LIMITED:
                case Status.EXTENDED_TRACKED:
                    if (imageTrackingStatus == Status.NO_POSE)
                        return;
                    lastImageTrackedPosition = transform.position;
                    lastImageTrackedRotation = transform.rotation.eulerAngles;
                    onImageLostDevicePos = positionAction.ReadValue<Vector3>();
                    onImageLostDeviceRot = rotationAction.ReadValue<Quaternion>().eulerAngles;
                    imageTrackingStatus = Status.NO_POSE;
                    transform.SetParent(null);
                    break;
                case Status.TRACKED:
                    if (imageTrackingStatus == Status.TRACKED)
                        return;
                    transform.SetParent(imageContainer);
                    transform.localRotation = Quaternion.identity;
                    transform.localPosition = Vector3.zero;
                    imageTrackingStatus = Status.TRACKED;
                    break;
            }
        };

        //Position
        

        //Rotation
    }
    private void OnEnable() => inputs.Enable();
    private void OnDisable() => inputs.Disable();

    private void Update()
    {
        //Temporary
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
        }
    
        if (imageTrackingStatus == Status.TRACKED)
            return;
    
        var actions = inputs.Player;
        var position = actions.PhonePosition.ReadValue<Vector3>();
        var rotation = actions.PhoneRotation.ReadValue<Quaternion>();
    
        transform.position = lastImageTrackedPosition + positionScaleFactor * AlterPositionAxis((position - onImageLostDevicePos)); 
        transform.rotation = Quaternion.Euler(lastImageTrackedRotation + rotation.eulerAngles - onImageLostDeviceRot);
    }

    //Real world and Unity axis react differently
    //x is y, y is x, z is -z
    private Vector3 AlterPositionAxis(Vector3 v)
    {
        if (!alterPositionAxis)
            return v;
        return new Vector3(-v.x, v.y, -v.z);
    }
}
