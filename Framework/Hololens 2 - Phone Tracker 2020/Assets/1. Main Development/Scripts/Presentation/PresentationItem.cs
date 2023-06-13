using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PresentationItem : MonoBehaviour
{
    [SerializeField] ScreenOrientation orientation;
    public UnityEvent onEnter, onExit;
    public ScreenOrientation Orientation => orientation;
}
