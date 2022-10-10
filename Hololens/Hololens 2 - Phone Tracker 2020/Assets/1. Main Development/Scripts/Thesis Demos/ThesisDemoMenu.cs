using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class ThesisDemoMenu : MonoBehaviour
{
    [SerializeField] float waitAfterButtonClick = 0.05f;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject internalMenu;
    [SerializeField] DeviceServer phoneServer;
    [SerializeField] Vector3 offsetPosition = new Vector3(0, 0, 0.8f);
    [SerializeField] GameObject gamepadDemoObject, trackerDemoObject, accelerationDemoObject, attitudeDemoObject, touchscreenDemoObject;

    GameObject activeObject;
    private IEnumerable<GameObject> objs 
    { 
        get
        {
            yield return gamepadDemoObject;
            yield return trackerDemoObject;
            yield return accelerationDemoObject;
            yield return attitudeDemoObject;
            yield return touchscreenDemoObject;
        } 
    }
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
        Disable();
        mainMenu.SetActive(true);
        phoneServer.EnableTracker(false);
    }

    public void EnableGamepadDemo()
    {
        Enable(gamepadDemoObject);
        phoneServer.EnableTracker(false);
    }
    public void EnableTrackerDemo() => Enable(trackerDemoObject);
    public void EnableAccelerationDemo() => Enable(accelerationDemoObject);
    public void EnableAttitudeDemo() => Enable(attitudeDemoObject);
    public void EnableTouchscreenDemo() => Enable(touchscreenDemoObject);

    private void Enable(GameObject activeObj)
    {
        foreach (var obj in objs)
        {
            if (obj == activeObj)
            {
                obj.SetActive(true);
                if (obj == trackerDemoObject)
                    phoneServer.EnableTracker(true);
                else
                    obj.transform.position = Camera.main.transform.position + Camera.main.transform.rotation * offsetPosition;
            }
            else
                obj.SetActive(false);
        }
    }
    private void Disable()
    {
        phoneServer.EnableTracker(false);
        foreach (var obj in objs)
            obj.SetActive(false);
    }
}