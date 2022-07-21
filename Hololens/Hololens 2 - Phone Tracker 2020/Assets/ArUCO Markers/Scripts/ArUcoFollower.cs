using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArUcoFollower : MonoBehaviour
{
    private Vector3 ogPos;
    private Quaternion ogRot;
    [SerializeField] ArucoTracker tracker;

    private void Awake()
    {
        ogPos = transform.position;
        ogRot = transform.rotation;
    }
    private void OnEnable()
    {
        tracker.onDetectionFinished += OnBoard;
    }

    private void OnDisable()
    {
        tracker.onDetectionFinished -= OnBoard;
    }

    private void OnBoard(IReadOnlyList<Marker> detectedMarkers)
    {
        //
        if (detectedMarkers.Count != 0)
        {
            var firstMarker = detectedMarkers[0];
            transform.SetPositionAndRotation(firstMarker.position, firstMarker.rotation);
        }
        else
            transform.SetPositionAndRotation(ogPos, ogRot);
    }
}
