using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class CameraTester : MonoBehaviour
{
    WebCamTexture texture;
    MaterialPropertyBlock mProp;
    
    private void Awake()
    {
        mProp = new MaterialPropertyBlock();
        var device = WebCamTexture.devices[0];
        texture = new WebCamTexture(device.name);

        var renderer = GetComponent<Renderer>();
        renderer.GetPropertyBlock(mProp);
        mProp.SetTexture("_MainTex", texture);
        renderer.SetPropertyBlock(mProp);
    }

    private void OnEnable() => texture.Play();
    private void OnDisable() => texture.Stop();
}
