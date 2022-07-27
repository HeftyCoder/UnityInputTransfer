using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkersLayout : MonoBehaviour
{
    [SerializeField] DpiFetcher dpiFetcher;
    [SerializeField] bool findChildrenInTransform;
    [SerializeField] List<RectTransform> rectTransforms = new List<RectTransform>();
    [SerializeField] List<RectTransform> excludedTransforms = new List<RectTransform>();

    [SerializeField] List<ArucoBoardLayoutItem> testItems;
    [ContextMenu("Test")]
    private void Test()
    {
        var board = CalculateBoard();
        testItems = board.items;
    }

    private void Awake()
    {
        if (findChildrenInTransform)
        {
            rectTransforms.Clear();
            foreach (Transform child in transform)
            {
                var rectTr = (RectTransform)child;
                if (!excludedTransforms.Contains(rectTr))
                    rectTransforms.Add(rectTr);
            }
        }
    }
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

            var topLeftInScreen = GetFromCenterInMeters(topLeft, camera);
            var topRightInScreen = GetFromCenterInMeters(topRight, camera);
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


    private Vector3 GetFromCenterInMeters(Vector3 worldPos, Camera camera)
    {
        var inScreen = camera.WorldToScreenPoint(worldPos);
        var dpi = dpiFetcher.GetDpi();
        inScreen -= new Vector3(Screen.width / 2, Screen.height / 2, 0);
        inScreen.z = 0;
        inScreen = inScreen / dpi;
        inScreen *= 0.0254f;
        return inScreen;
    }
}
