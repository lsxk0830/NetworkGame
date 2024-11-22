using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SimpleFrame
{
    public static partial class ToolExtension
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        public static IEnumerator SendRequest_Get(this object obj, string url, Action<string> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Error: {webRequest.error}");
                else
                    callback?.Invoke(webRequest.downloadHandler.text);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public static IEnumerator SendRequest_Get<T>(this object obj, string url, Action<T> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Error: {webRequest.error}");
                else
                {
                    T t = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callback?.Invoke(t);
                }
            }
        }
    }
}