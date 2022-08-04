using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SpatialTracking;

public class ControlUI : MonoBehaviour
{
    [SerializeField] Button disconnectButton;
    [SerializeField] PhoneClient phoneClient;
    [SerializeField] PresentationUI presentationUI;
    private bool inputLock = false;
    private void Awake()
    {

        //When disconnecting, the phoneClient handles the UI. No need to hide anything here
        disconnectButton.onClick.AddListener(() =>
        {
            disconnectButton.interactable = false;
            presentationUI.ExitPresentation();
            phoneClient.Disconnect();
        });
        
    }
}
