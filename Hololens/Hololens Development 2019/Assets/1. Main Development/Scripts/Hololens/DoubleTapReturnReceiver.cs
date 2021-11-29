using System;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTapReturnReceiver : MonoBehaviour
{
    [SerializeField] Transform anchor;

    private void OnEnable() => HololensPhoneServer.Instance.onPhoneInput += HandleInput;

    private void OnDisable() => HololensPhoneServer.Instance.onPhoneInput -= HandleInput;

    private void HandleInput(PhoneInputSerialization input)
    {
        if (input.TapCount >= 2)
            transform.position = anchor.position;
    }
}
