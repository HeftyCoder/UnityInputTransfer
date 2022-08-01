using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class SimpleFirebaseClient : MonoBehaviour
{
    private const string myUrl = "https://hololens-phone-controller-default-rtdb.europe-west1.firebasedatabase.app/";

    [SerializeField] string firebaseUrl;

    public void Get<T>(string path, Action<T, bool> onResult)
    {
        Get(path, (data, result) =>
        {
            if (!result)
            {
                onResult?.Invoke(default(T), result);
                return;
            }

            var dataAsObj = JsonUtility.FromJson<T>(data);
            onResult?.Invoke(dataAsObj, result);
        });
    }
    public void Get(string path, Action<string, bool> onResult)
    {
        IEnumerator get()
        {
            var request = UnityWebRequest.Get(getPath(path));
            yield return request.SendWebRequest();
            var result = request.result == UnityWebRequest.Result.Success;
            var data = request.downloadHandler.text;
            result = result && data != "null";
            onResult?.Invoke(request.downloadHandler.text, result);
        }
        StartCoroutine(get());
    }

    public void Save(string path, object obj, Action<string, bool> onResult) =>
        Save(path, JsonUtility.ToJson(obj), onResult);
    public void Save(string path, string data, Action<string, bool> onResult)
    {
        IEnumerator save()
        {
            var request = UnityWebRequest.Put(getPath(path), data);
            yield return request.SendWebRequest();
            var result = request.result == UnityWebRequest.Result.Success;
            onResult?.Invoke(request.downloadHandler.text, result);
        }
        StartCoroutine(save());
    }

    private string getPath(string innerPath) => $"{firebaseUrl}{innerPath}.json";

}
