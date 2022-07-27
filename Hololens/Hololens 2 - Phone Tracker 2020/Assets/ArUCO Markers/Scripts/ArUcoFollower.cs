using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArucoFollower : MonoBehaviour
{
    private Vector3 pos;
    private Quaternion rot;
    [SerializeField] ArucoTracker tracker;

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
        if (detectedMarkers.Count == 0)
            return;
        var firstMarker = detectedMarkers[0];
        pos = firstMarker.position;
        rot = firstMarker.rotation;
        transform.SetPositionAndRotation(pos, rot);
    }
}
