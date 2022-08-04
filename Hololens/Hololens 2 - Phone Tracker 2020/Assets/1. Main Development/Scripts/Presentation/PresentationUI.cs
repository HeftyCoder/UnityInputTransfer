using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentationUI : MonoBehaviour
{
    [SerializeField] GameObject connectionUI;
    [SerializeField] ScreenOrientation mainOrientation;

    private PresentationItem currentUI;
    public void EnterPresentation(PresentationItem item)
    {
        currentUI = item;
        currentUI.gameObject.SetActive(true);
        connectionUI.SetActive(false);
        Screen.orientation = currentUI.Orientation;
        currentUI.onEnter?.Invoke();
    }
    public void ExitPresentation()
    {
        currentUI.gameObject.SetActive(false);
        connectionUI.SetActive(true);
        Screen.orientation = mainOrientation;
        currentUI.onExit?.Invoke();
        currentUI = null;
    }

    private void OnEnable()
    {
        ExitPresentation();
    }
}
