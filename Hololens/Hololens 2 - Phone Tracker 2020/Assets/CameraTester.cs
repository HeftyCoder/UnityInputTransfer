using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using System;
using System.Collections.Generic;
public class CameraTester : MonoBehaviour
{
    MaterialPropertyBlock mProp;
    [SerializeField] ARCameraManager manager;
    Texture2D texture;
    private void Awake()
    {
        mProp = new MaterialPropertyBlock();
        var device = WebCamTexture.devices[0];

        texture = new Texture2D(520, 520);
        var renderer = GetComponent<Renderer>();
        renderer.GetPropertyBlock(mProp);
        mProp.SetTexture("_MainTex", texture);
        renderer.SetPropertyBlock(mProp);

    }

    private void Update()
    {
        if (!manager.TryAcquireLatestCpuImage(out var image))
            return;

        var parameters = new XRCpuImage.ConversionParams(image, TextureFormat.ARGB32);
        image.ConvertAsync(parameters, ProcessImageData);
    
    }

    private void ProcessImageData(XRCpuImage.AsyncConversionStatus status, XRCpuImage.ConversionParams cParams, NativeArray<byte> imData)
    {
        if (status == XRCpuImage.AsyncConversionStatus.Ready)
        {
            texture.LoadRawTextureData(imData);
            texture.Apply();
        }
    }

}
