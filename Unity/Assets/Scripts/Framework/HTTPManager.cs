using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPManager : Singleton<HTTPManager>
{
    public async UniTaskVoid Post<T>(string url, T data, Action<string> callback) where T : class
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

                    if (request.result == UnityWebRequest.Result.ProtocolError) // 检查是否是 HTTP 错误（401、404、500 等）
                    {
                        HandleError(request.responseCode, request.downloadHandler.text);
                        return;
                    }
                    else if (request.result == UnityWebRequest.Result.Success)
                    {
                        callback.Invoke(request.downloadHandler.text);
                    }
                    else
                    {
                        HandleError(request.responseCode, request.error);
                    }
                }
            }
        }
        catch (UnityWebRequestException ex)
        {
            // 捕获 UnityWebRequest 异常（如 401）
            HandleError(ex.ResponseCode, ex.Message);
            //Debug.LogError($"UnityWebRequest 异常: {ex.Message}");
        }
        catch (Exception ex)
        {
            // 其他异常（如 JSON 解析错误）
            HandleError(250, ex.Message);
            Debug.LogError($"未知错误: {ex.Message}");
        }
    }

    private void HandleError(long code, string error)
    {
        switch (code)
        {
            case 400:
                PanelManager.Open<TipPanel>("请求格式错误");
                Debug.LogError("请求格式错误");
                break;
            case 401:
                PanelManager.Open<TipPanel>("用户名或密码错误");
                Debug.LogError("用户名或密码错误");
                break;
            case 429:
                PanelManager.Open<TipPanel>("尝试次数过多，请稍后再试");
                Debug.LogError("尝试次数过多，请稍后再试");
                break;
            case 500:
                PanelManager.Open<TipPanel>("服务器开小差了，请联系客服");
                Debug.LogError("服务器开小差了，请联系客服");
                break;
            default:
                PanelManager.Open<TipPanel>($"连接失败: {error}");
                Debug.LogError($"连接失败: {error}");
                break;
        }
    }
}