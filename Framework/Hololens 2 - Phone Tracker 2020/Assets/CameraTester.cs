using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraTester : MonoBehaviour
{
    [SerializeField] RawImage image;
    MaterialPropertyBlock mProp;
    WebCamTexture texture;
    private void Awake()
    {
        mProp = new MaterialPropertyBlock();
        var device = WebCamTexture.devices[0];

        texture = new WebCamTexture(device.name);
        image.texture = texture;
        
    }

    private void OnEnable()
    {
        texture.Play();
    }

    private void OnDisable()
    {
        texture.Stop();
    }

    public void ChangeState() => enabled = !enabled;

}
