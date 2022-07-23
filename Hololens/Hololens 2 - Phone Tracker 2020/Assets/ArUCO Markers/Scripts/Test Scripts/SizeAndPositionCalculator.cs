using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class SizeAndPositionCalculator : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text tmp;
    [SerializeField] DpiFetcher dpiFetcher;
    public void Calculate()
    {
        var rectTr = (RectTransform)transform;
        Vector3[] corners = new Vector3[4];
        rectTr.GetWorldCorners(corners);
        //botL,topL,topR,botR
        var topLeft = corners[1];
        var topRight = corners[2];
        var camera = Camera.main;

        var dpi = dpiFetcher.GetDpi();
        var center = camera.WorldToScreenPoint(rectTr.position);
        var topLeftInScreen = GetFromCenterInCentimeters(topLeft, camera);
        var topRightInScreen = GetFromCenterInCentimeters(topRight, camera);
        var width = (topLeftInScreen - topRightInScreen).magnitude;
        tmp.text = $"{topLeftInScreen}\n{width}";
    }
    private Vector3 GetFromCenterInCentimeters(Vector3 worldPos, Camera camera)
    {
        var inScreen = camera.WorldToScreenPoint(worldPos);
        var dpi = dpiFetcher.GetDpi();
        inScreen -= new Vector3(Screen.width / 2, Screen.height / 2, 0);
        inScreen.z = 0;
        inScreen = inScreen / dpi;
        inScreen *= 2.54f;
        return inScreen;
    }
    public void OnPointerClick(PointerEventData data)
    {
        Calculate();
    }

    /*
    public void Calculate()
    {
        var rectTr = (RectTransform)transform;
        Vector3[] corners = new Vector3[4];
        rectTr.GetWorldCorners(corners);
        //botL,topL,topR,botR
        var topLeft = corners[1];
        var topRight = corners[2];
        var camera = Camera.main;

        var topLeftInScreen = GetFromCenterInCentimeters(topLeft, camera);
        var topRightInScreen = GetFromCenterInCentimeters(topRight, camera);
        var width = (topLeftInScreen - topRightInScreen).magnitude;

        tmp.text = $"{topLeftInScreen}\n{width}";
    }*/

}
