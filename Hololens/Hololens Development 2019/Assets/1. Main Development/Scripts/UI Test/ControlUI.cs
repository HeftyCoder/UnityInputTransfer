using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SpatialTracking;

public class ControlUI : MonoBehaviour
{
    [SerializeField] Button inputButton;
    [SerializeField] TMP_Text inputButtonTMP;
    [SerializeField] Button disconnectButton;
    [SerializeField] Button resetButton;
    [SerializeField] PhoneClient phoneClient;
    [SerializeField] TMP_Text devicesText;
    private bool inputLock = false;
    private void Awake()
    {
        UnlockInput();
        inputButton.onClick.AddListener(() =>
        {
            if (inputLock)
                UnlockInput();
            else
                LockInput();
        });

        //When disconnecting, the phoneClient handles the UI. No need to hide anything here
        disconnectButton.onClick.AddListener(() =>
        {
            disconnectButton.interactable = false;
            phoneClient.Disconnect();
        });
        
    }

    private void OnDisable()
    {
        UnlockInput();
        disconnectButton.interactable = true;
    }
    private void LockInput()
    {
        inputLock = true;
        phoneClient.enabled = false;
        inputButtonTMP.text = "Unlock Input";
    }

    private void UnlockInput()
    {
        inputLock = false;
        phoneClient.enabled = true;
        inputButtonTMP.text = "Lock Input";
    }

}
