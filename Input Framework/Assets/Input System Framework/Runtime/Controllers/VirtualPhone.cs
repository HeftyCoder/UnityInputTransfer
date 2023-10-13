using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class VirtualPhone : MonoBehaviour
{
    public struct PoseInformation
    {
        public Vector3 pos;
        public Quaternion rot;
        public double timeStamp;
    }
    [SerializeField] ArucoTracker arucoTracker;
    [SerializeField] DeviceServer phoneServer;

    //InputActions inputs => phoneServer.InputActions;

    [Header("VIO Reported")]
    public Vector3 posVio;
    public Quaternion rotVio;

    [Header("Marker and At Marker Vio")]
    public Vector3 posMarker;
    public Quaternion rotMarker;

    public int earlyBufferLength = 10;
    public int poseBufferLength = 50;
    private double arucoDetectionTime;
    private Quaternion R = Quaternion.identity;
    private Vector3 T = Vector3.zero;

    private InputAction phonePositionInput, phoneRotationInput;

    private PoseInformation[] earlyPoseBuffer, earlyPoseSavedBuffer, poseBuffer;
    private int readCount = 0, earlyPoseIndex = 0, timedPoseIndex = 0, poseIndex = 0;
    [ContextMenu("Test Calculate RT")]
    private void TestInEditor()
    {
        CalculateRT(posVio, rotVio, posMarker, rotMarker);
    }
    private void Awake()
    {
        earlyPoseBuffer = new PoseInformation[earlyBufferLength];
        earlyPoseSavedBuffer = new PoseInformation[earlyBufferLength];
        poseBuffer = new PoseInformation[poseBufferLength];

        //var actions = inputs.Phone;
        posMarker = transform.position;
        rotMarker = transform.rotation;

        posVio = transform.position;
        rotVio = transform.rotation;

        //Reading the values that the tracker has for us
        //phonePositionInput = actions.PhonePosition;
        //phoneRotationInput = actions.PhoneRotation;
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
        UpdateBuffer(posVio, rotVio);
    }
    private void ReadRotation(InputAction.CallbackContext ctx)
    {
        rotVio = ctx.ReadValue<Quaternion>();
        UpdateBuffer(posVio, rotVio);
    }
    private void UpdateBuffer(Vector3 posVio, Quaternion rotVio)
    {
        readCount++;
        if (readCount == 2)
        {
            readCount = 0;
            double phoneTime = phoneServer.LastNetworkTimestamp;
            var pose = new PoseInformation()
            {
                pos = posVio,
                rot = rotVio,
                timeStamp = phoneTime
            };

            earlyPoseBuffer[earlyPoseIndex] = pose;
            earlyPoseIndex++;
            if (earlyPoseIndex >= earlyPoseBuffer.Length)
                earlyPoseIndex = 0;

            if (poseIndex >= poseBufferLength)
                return;
            poseBuffer[poseIndex] = pose;
            poseIndex++;
            CalculateRT();
        }
    }
    private void OnArucoScanFinished(IReadOnlyList<Marker> markers)
    {
        if (markers.Count == 0)
            return;
        //We're treating this as if it was a board
        var marker = markers[0];
        posMarker = marker.position;
        rotMarker = marker.rotation;

        //var actions = inputs.Phone;

        arucoDetectionTime = phoneServer.Clock.Time;

        //Fill the buffer with values we got before detection
        for (int i = 0; i <= earlyPoseIndex; i++)
        {
            earlyPoseSavedBuffer[i] = earlyPoseBuffer[i];
        }
        earlyPoseIndex = 0;
        poseIndex = 0;
    }

    private void CalculateRT()
    {
        PoseInformation poseInfo = new PoseInformation();
        double diff = double.MaxValue;
        for (int i = 0; i < earlyPoseIndex; i++)
        {
            var p = earlyPoseSavedBuffer[i];
            var currentDiff = Math.Abs(arucoDetectionTime - p.timeStamp);
            if (currentDiff < diff)
            {
                diff = currentDiff;
                poseInfo = p;
            }
        }

        for (int i = 0; i < poseIndex; i++)
        {
            var p = poseBuffer[i];
            var currentDiff = Math.Abs(arucoDetectionTime - p.timeStamp);
            if (currentDiff < diff)
            {
                diff = currentDiff;
                poseInfo = p;
            }
        }

        CalculateRT(poseInfo.pos, poseInfo.rot, posMarker, rotMarker);
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
