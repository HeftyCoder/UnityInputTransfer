using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class ThesisDemoMenu : MonoBehaviour
{
    [SerializeField] float waitAfterButtonClick = 0.05f;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject internalMenu;
    [SerializeField] PhoneServer phoneServer;
    [SerializeField] Vector3 gamepadStartPos = new Vector3(0, 0, 0.8f);
    [SerializeField] GameObject gamepadDemoObject, trackerDemoObject;

    bool working = false;
    public void EnterDemoMenu()
    {
        mainMenu.SetActive(false);
        gameObject.SetActive(true);
        internalMenu.SetActive(true);
    }

    public void ExitDemoMenu()
    {
        internalMenu.SetActive(false);
        gameObject.SetActive(false);
        gamepadDemoObject.SetActive(false);
        trackerDemoObject.SetActive(false);
        mainMenu.SetActive(true);
        phoneServer.EnableTracker(false);
    }

    public void EnableGamepadDemo()
    {
        gamepadDemoObject.transform.position = Camera.main.transform.rotation * gamepadStartPos;
        gamepadDemoObject.SetActive(true);
        trackerDemoObject.SetActive(false);
        phoneServer.EnableTracker(false);
    }
    public void EnableTrackerDemo()
    {
        gamepadDemoObject.SetActive(false);
        trackerDemoObject.SetActive(true);
        phoneServer.EnableTracker(true);
    }
}