using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ControlUI : MonoBehaviour
{
    [SerializeField] Button inputButton;
    [SerializeField] TMP_Text inputButtonTMP;
    [SerializeField] Button disconnectButton;
    [SerializeField] HololensPhoneClient phoneClient;
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
        inputButtonTMP.text = "Unlock Input";
    }

    private void UnlockInput()
    {
        inputLock = false;
        inputButtonTMP.text = "Lock Input";
    }

}
