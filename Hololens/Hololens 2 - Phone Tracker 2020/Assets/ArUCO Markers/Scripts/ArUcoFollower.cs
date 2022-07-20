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
        tracker.onBoardDetected += OnBoard;
    }

    private void OnDisable()
    {
        tracker.onBoardDetected -= OnBoard;
    }

    private void OnBoard(bool found, Vector3 pos, Quaternion rot)
    {
        if (found)
            transform.SetPositionAndRotation(pos, rot);
        else
            transform.SetPositionAndRotation(ogPos, ogRot);
    }
}
