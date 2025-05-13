using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPManager : Singleton<HTTPManager>
{
    public async UniTaskVoid Post<T>(string url, T data, Action<string> OnSuccess, Action<long, string> OnFailed) where T : class
    {
        try
        {
            using (var request = UnityWebRequest.Post(url, "POST"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                string json = JsonUtility.ToJson(data);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                using (var uploadHandler = new UploadHandlerRaw(jsonBytes))
                {
                    request.uploadHandler = uploadHandler;
                    await request.SendWebRequest().ToUniTask();

                    if (request.result == UnityWebRequest.Result.Success)
                        OnSuccess.Invoke(request.downloadHandler.text);
                    else if (request.result == UnityWebRequest.Result.ProtocolError) // 检查是否是 HTTP 错误（401、404、500 等）
                        OnFailed.Invoke(request.responseCode, request.downloadHandler.text);
                    else
                        OnFailed.Invoke(request.responseCode, request.error);
                }
            }
        }
        catch (UnityWebRequestException ex)
        {
            // 捕获 UnityWebRequest 异常（如 401）
            OnFailed.Invoke(ex.ResponseCode, ex.Message);
            //Debug.LogError($"UnityWebRequest 异常: {ex.Message}");
        }
        catch (Exception ex)
        {
            // 其他异常（如 JSON 解析错误）
            OnFailed.Invoke(250, ex.Message);
            Debug.LogError($"未知错误: {ex.Message}");
        }
    }
}