using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Marker
{
    public static Marker Identity => new Marker(Vector3.zero, Quaternion.identity);

    public Vector3 position;
    public Quaternion rotation;

    public Marker(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
