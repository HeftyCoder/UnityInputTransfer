using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentationUI : MonoBehaviour
{
    [SerializeField] GameObject connectionUI;
    [SerializeField] ScreenOrientation mainOrientation;

    [SerializeField] PresentationItem[] presItems;
    private PresentationItem currentUI;

    private void Start()
    {
        foreach (var presentation in presItems)
            presentation.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
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
        connectionUI.SetActive(true);
        Screen.orientation = mainOrientation;
        if (currentUI == null)
            return;
        currentUI.gameObject.SetActive(false);
        currentUI.onExit?.Invoke();
        currentUI = null;
    }
}
