using UnityEngine;
using Unity.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class WebCameraTester : MonoBehaviour
{
    [SerializeField] TMP_Text tmpText;

    MaterialPropertyBlock mProp;
    WebCamTexture texture;
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

    public void Toggle() => enabled = !enabled;

    private void Update()
    {
        tmpText.text = texture.requestedFPS.ToString();
    }

    private void OnEnable() => texture.Play();

    private void OnDisable() => texture.Stop();

}