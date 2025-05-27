using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResManager : MonoSingleton<ResManager>
{
    // 使用字典存储资源句柄，Key=资源路径(sourceName)，Value=加载句柄
    private Dictionary<string, AsyncOperationHandle> handleDic = new Dictionary<string, AsyncOperationHandle>();

    /// <summary>
    /// 异步加载Addressable资源
    /// </summary>
    /// <typeparam name="T">资源类型（如GameObject、Texture等）</typeparam>
    /// <param name="sourceName">资源在Addressables中的路径</param>
    /// <param name="isRelease">立即释放资源</param>
    /// <param name="onComplete">加载成功回调（返回资源对象）</param>
    /// <param name="onFailed">加载失败回调（返回错误信息）</param>
    public async UniTaskVoid LoadAssetAsync<T>(string sourceName, bool isRelease = false,
        Action<T> onComplete = null,
        Action<string> onFailed = null)
    {
        if (handleDic.ContainsKey(sourceName)) // 检查是否已加载过该资源
        {
            var resultHandle = handleDic[sourceName];
            if (resultHandle.IsDone && resultHandle.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete?.Invoke((T)resultHandle.Result);
                return;
            }
        }
        try
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(sourceName);
            if (!isRelease) handleDic[sourceName] = handle; // 记录句柄到字典

            await handle.Task; // 等待加载完成

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete?.Invoke(handle.Result);
                if (isRelease) Addressables.Release(handle);
            }
            else
            {
                onFailed?.Invoke($"Failed to load asset: {sourceName}. Error: {handle.OperationException}");
                Addressables.Release(handle);
            }
        }
        catch (Exception e)
        {
            onFailed?.Invoke($"Exception when loading {sourceName}: {e.Message}");
            ReleaseHandle(sourceName);
        }
    }

    /// <summary>
    /// 释放单个资源
    /// </summary>
    public void ReleaseHandle(string sourceName)
    {
        if (handleDic.TryGetValue(sourceName, out var handle))
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
            handleDic.Remove(sourceName);
        }
    }

    /// <summary>
    /// 释放所有已加载资源
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var kvp in handleDic)
        {
            if (kvp.Value.IsValid())
            {
                Addressables.Release(kvp.Value);
            }
        }
        handleDic.Clear();
    }

    private void OnDestroy()
    {
        ReleaseAll(); // 自动清理
    }
}
