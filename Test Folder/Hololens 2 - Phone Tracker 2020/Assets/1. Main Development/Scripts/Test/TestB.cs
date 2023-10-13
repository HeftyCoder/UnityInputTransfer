using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TestB : MonoBehaviour
{
    [ContextMenu("Test")]
    private void Test()
    {
        TestGuid();
    }

    private void TestGuid()
    {
        Action<byte[]> randomizeBytes = (bytes) =>
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                var b = (byte)UnityEngine.Random.Range(0, 2);
                Debug.Log(b);
                bytes[i] = b;
            }
                
        };
        var bytes = new byte[16];
        randomizeBytes(bytes);
        var guid = new Guid(bytes);
        randomizeBytes(bytes);
        Debug.Log($"{guid} {new Guid(bytes)}");

    }
}
