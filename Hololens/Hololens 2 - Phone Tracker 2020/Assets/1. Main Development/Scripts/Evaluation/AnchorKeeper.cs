using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.OpenXR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(XRAnchorStore), typeof(ARAnchorManager))]
public class AnchorKeeper : MonoBehaviour
{
    [SerializeField] GameObject prefabForVisualization;

    private XRAnchorStore anchorStore;
    private ARAnchorManager anchorManager;
    private void Awake()
    {
        anchorStore = GetComponent<XRAnchorStore>();
        anchorManager = GetComponent<ARAnchorManager>();
    }


}
