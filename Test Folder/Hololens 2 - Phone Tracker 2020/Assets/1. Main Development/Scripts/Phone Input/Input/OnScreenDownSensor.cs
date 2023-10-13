using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem;

[AddComponentMenu("Input/On-Screen Down Sensor")]
public class OnScreenDownSensor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [InputControl(layout = "Quaternion")]
    [SerializeField]
    private string controlPath;

    [SerializeField] TMPro.TMP_Text tmp;
    InputControl control;
    private void Start()
    {
        control = InputSystem.FindControl(controlPath);
    }
    bool sendValues = false;
    public void OnPointerDown(PointerEventData data) => sendValues = true;
    public void OnPointerUp(PointerEventData data) => sendValues = false;
    private void Update()
    {

    }
    
}