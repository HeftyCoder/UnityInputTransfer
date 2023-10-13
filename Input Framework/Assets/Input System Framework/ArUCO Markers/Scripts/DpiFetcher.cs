using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DpiFetcher : MonoBehaviour
{
    [SerializeField] TMP_InputField dpiTmp;

    public float GetDpi()
    {
        float.TryParse(dpiTmp.text, out float result);
        if (result == 0)
            return Screen.dpi;
        return result;
    }
}
