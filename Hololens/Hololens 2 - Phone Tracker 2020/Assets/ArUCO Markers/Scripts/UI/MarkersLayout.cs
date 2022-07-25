using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkersLayout : MonoBehaviour
{
    [SerializeField] DpiFetcher dpiFetcher;
    [SerializeField] List<RectTransform> rectTransforms = new List<RectTransform>();
    public ArucoBoardLayout CalculateBoard()
    {
        var result = new ArucoBoardLayout();
        var items = new List<ArucoBoardLayoutItem>();

        var dpi = dpiFetcher.GetDpi();
        Vector3[] corners = new Vector3[4];
        var camera = Camera.main;
        foreach (var rectTr in rectTransforms)
        {
            rectTr.GetWorldCorners(corners);
            var topLeft = corners[1];
            var topRight = corners[2];

            var topLeftInScreen = GetFromCenterInCentimeters(topLeft, camera);
            var topRightInScreen = GetFromCenterInCentimeters(topRight, camera);
            var width = (topLeftInScreen - topRightInScreen).magnitude;

            var item = new ArucoBoardLayoutItem
            {
                topLeftCorner = topLeftInScreen,
                size = width
            };

            items.Add(item);
        }

        result.items = items;
        return result;
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
}
