using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPManager : Singleton<HTTPManager>
{
    public async UniTaskVoid Post<T>(string url, T data, Action<string> callback) where T : class
    {
        using (var request = UnityWebRequest.Post(url, "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            string json = JsonUtility.ToJson(data);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));

            await request.SendWebRequest().ToUniTask();

            // 处理响应
            if (request.result == UnityWebRequest.Result.Success)
            {
                callback.Invoke(request.downloadHandler.text);
            }
            else
            {
                HandleError(request.responseCode, request.error);
            }
        }
    }

    private void HandleError(long code, string error)
    {
        switch (code)
        {
            case 400:
                Debug.LogError("请求格式错误");
                break;
            case 401:
                Debug.LogError("用户名或密码错误");
                break;
            case 429:
                Debug.LogError("尝试次数过多，请稍后再试");
                break;
            case 500:
                Debug.LogError("服务器开小差了，请联系客服");
                break;
            default:
                Debug.LogError($"连接失败: {error}");
                break;
        }
    }
}