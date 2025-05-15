using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPManager : Singleton<HTTPManager>
{
    // 默认超时时间（秒）
    public int Timeout = 10;

    #region 标准Get

    /// <summary>
    /// GET方法,标准实现不带查询参数
    /// </summary>
    public async UniTask Get(string url, Action<string> onSuccess, Action<long, string> onFailed)
    {
        await Get(url, null, onSuccess, onFailed);
    }

    /// <summary>
    /// GET方法,带查询参数
    /// HTTPManager.Instance.Get(API.GetAvatar, new { ID = GameMain.ID }, OnGetAvatarSuccess, OnGetAvatarFail).Forget();
    /// </summary>
    public async UniTask Get(string url, object queryParams, Action<string> onSuccess, Action<long, string> onFailed)
    {
        try
        {
            if (queryParams != null) // 添加查询参数
            {
                url = AddQueryParams(url, queryParams);
            }

            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = Timeout;
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest().ToUniTask();

                ProcessResponse(request, onSuccess, onFailed);
            }
        }
        catch (UnityWebRequestException ex)
        {
            onFailed.Invoke(ex.ResponseCode, ex.Message);
            Debug.LogError($"请求失败: {url}\n错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            onFailed.Invoke(250, ex.Message);
            Debug.LogError($"未知错误: {url}\n错误: {ex.Message}");
        }
    }

    #endregion Get

    #region Post

    /// <summary>
    /// POST方法
    /// </summary>
    public async UniTaskVoid Post<T>(string url, T data, Action<string> onSuccess, Action<long, string> onFailed) where T : class
    {
        try
        {
            using (var request = new UnityWebRequest(url, "POST"))
            {
                request.timeout = Timeout;
                request.SetRequestHeader("Content-Type", "application/json");

                string json = JsonConvert.SerializeObject(data);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(jsonBytes);
                request.downloadHandler = new DownloadHandlerBuffer();

                await request.SendWebRequest().ToUniTask();

                ProcessResponse(request, onSuccess, onFailed);
            }
        }
        catch (UnityWebRequestException ex)
        {
            onFailed.Invoke(ex.ResponseCode, ex.Message);
            Debug.LogError($"请求失败: {url}\n错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            onFailed.Invoke(250, ex.Message);
            Debug.LogError($"未知错误: {url}\n错误: {ex.Message}");
        }
    }

    #endregion Post

    #region GetImage

    /// <summary>
    /// 获取用户头像
    /// </summary>
    public async UniTask SetAvatar(string avatarPath, UnityEngine.UI.Image image)
    {
        if (image == null) return;

        Texture2D texture = await GetAvatar(avatarPath);
        if (texture != null)
        {
            image.sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    #endregion GetImage

    #region 内部实现

    /// <summary>
    /// 处理响应（提取公共逻辑）
    /// </summary>
    private void ProcessResponse(UnityWebRequest request, Action<string> onSuccess, Action<long, string> onFailed)
    {
        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess.Invoke(request.downloadHandler.text);
        }
        else
        {
            var errorMessage = !string.IsNullOrEmpty(request.downloadHandler?.text)
                ? request.downloadHandler.text
                : request.error;

            onFailed.Invoke(request.responseCode, errorMessage);
        }
    }

    /// <summary>
    /// 添加查询参数
    /// </summary>
    private string AddQueryParams(string url, object parameters)
    {
        string json = JsonConvert.SerializeObject(parameters);
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        var builder = new StringBuilder(url);

        if (!url.Contains("?")) builder.Append("?");
        else if (!url.EndsWith("&")) builder.Append("&");

        foreach (var pair in dict)
            builder.Append($"{UnityWebRequest.EscapeURL(pair.Key)}={UnityWebRequest.EscapeURL(pair.Value)}&");

        return builder.ToString().TrimEnd('&');
    }


    #region GetImage

    /// <summary>
    /// 获取用户头像（返回 Texture2D）
    /// </summary>
    /// <param name="avatarPath">头像路径（如 "user/123/avatar.png"）</param>
    private Texture2D defaultAvatar;
    private async UniTask<Texture2D> GetAvatar(string avatarPath)
    {
        string url = $"{API.GetAvatar}?path={UnityWebRequest.EscapeURL($"{avatarPath}.png")}"; // http://127.0.0.1:5000/api/getavatar?path=user/123/avatar.png
        try
        {
            using (var request = UnityWebRequestTexture.GetTexture(url))
            {
                request.timeout = Timeout;
                await request.SendWebRequest().ToUniTask();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"头像下载失败: {request.error}.使用默认头像");
                    return defaultAvatar;
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    return texture;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"获取头像失败: {url}\n错误: {ex.Message}");
            return defaultAvatar; // 返回默认头像或 null
        }
    }

    #endregion GetImage

    #endregion
}