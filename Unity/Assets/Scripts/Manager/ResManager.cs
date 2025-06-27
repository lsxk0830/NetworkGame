using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class ResManager : MonoSingleton<ResManager>
{
    // 资源句柄字典
    private readonly Dictionary<string, AsyncOperationHandle> resHandles = new Dictionary<string, AsyncOperationHandle>();

    // 引用计数
    private readonly Dictionary<string, int> refCounts = new Dictionary<string, int>();

    #region 单个资源加载 (LoadAssetAsync)

    /// <summary>
    /// 加载单个资源
    /// </summary>
    public async UniTaskVoid LoadAssetAsync<T>(string key, bool autoRelease = false,
        Action<T> onLoaded = null,
        Action<string> onFailed = null)
    {
        // 检查缓存
        if (TryGetCachedResource<T>(key, out T cachedAsset, out AsyncOperationHandle cachedHandle))
        {
            onLoaded?.Invoke(cachedAsset);
            if (!autoRelease) IncrementReference(key);
            return;
        }

        try
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            if (!autoRelease)
            {
                resHandles[key] = handle;
                IncrementReference(key);
            }

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(handle.Result);
                if (autoRelease) Addressables.Release(handle);
            }
            else
            {
                onFailed?.Invoke($"AA加载资源失败: {key}. 错误: {handle.OperationException}");
                CleanupHandle(key, handle, autoRelease);
            }
        }
        catch (Exception e)
        {
            onFailed?.Invoke($"加载{key}异常 : {e.Message}");
            ReleaseResource(key);
        }
    }

    #endregion

    #region 多个资源加载 (LoadAssetsAsync)

    /// <summary>
    /// 加载多个资源
    /// </summary>
    public async UniTaskVoid LoadAssetsAsync<T>(string key, bool autoRelease = false,
        Action<T> onLoaded = null,
        Action<IList<T>> onAllLoaded = null,
        Action<string> onFailed = null)
    {

        // 检查缓存
        if (TryGetCachedResource<IList<T>>(key, out var cachedAssets, out var cachedHandle))
        {
            if (onLoaded != null)
            {
                foreach (var asset in cachedAssets)
                {
                    onLoaded.Invoke(asset);
                }
            }
            onAllLoaded?.Invoke(cachedAssets);
            if (!autoRelease) IncrementReference(key);
            return;
        }

        try
        {
            var handle = Addressables.LoadAssetsAsync<T>(key, loadedAsset =>
            {
                onLoaded?.Invoke(loadedAsset);
            });

            if (!autoRelease)
            {
                resHandles[key] = handle;
                IncrementReference(key);
            }

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onAllLoaded?.Invoke(handle.Result);
                if (autoRelease) Addressables.Release(handle);
            }
            else
            {
                onFailed?.Invoke($"加载 {key} 错误: {handle.OperationException}");
                CleanupHandle(key, handle, autoRelease);
            }
        }
        catch (Exception e)
        {
            onFailed?.Invoke($"加载 {key} 时发生异常 : {e.Message}");
            ReleaseResource(key);
        }
    }

    #endregion

    #region 资源缓存检查

    /// <summary>
    /// 尝试从缓存获取资源
    /// </summary>
    private bool TryGetCachedResource<T>(string key, out T resource, out AsyncOperationHandle handle)
    {
        if (resHandles.TryGetValue(key, out handle))
        {
            if (handle.IsDone && handle.Status == AsyncOperationStatus.Succeeded)
            {
                try
                {
                    resource = (T)handle.Result;
                    return true;
                }
                catch (InvalidCastException)
                {
                    Debug.LogWarning($"缓存资源的类型不匹配: {key}");
                }
            }

            // 移除无效的缓存
            resHandles.Remove(key);
            refCounts.Remove(key);
            Addressables.Release(handle);
        }

        resource = default;
        return false;
    }

    #endregion

    #region 资源释放管理

    /// <summary>
    /// 释放资源
    /// </summary>
    public void ReleaseResource(string key)
    {
        if (!resHandles.ContainsKey(key)) return;

        DecrementReference(key);

        if (refCounts.TryGetValue(key, out var count) && count <= 0)
        {
            if (resHandles.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                resHandles.Remove(key);
                refCounts.Remove(key);
            }
        }
    }

    /// <summary>
    /// 清理句柄
    /// </summary>
    private void CleanupHandle(string key, AsyncOperationHandle handle, bool autoRelease)
    {
        if (!autoRelease)
        {
            resHandles.Remove(key);
            refCounts.Remove(key);
        }
        Addressables.Release(handle);
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var handle in resHandles.Values)
        {
            Addressables.Release(handle);
        }

        resHandles.Clear();
        refCounts.Clear();
    }

    #endregion

    #region 引用计数管理

    /// <summary>
    /// 增加引用计数
    /// </summary>
    private void IncrementReference(string key)
    {
        refCounts[key] = refCounts.TryGetValue(key, out var count) ? count + 1 : 1;
    }

    /// <summary>
    /// 减少引用计数
    /// </summary>
    private void DecrementReference(string key)
    {
        if (refCounts.TryGetValue(key, out var count))
        {
            if (count <= 1)
            {
                refCounts.Remove(key);
            }
            else
            {
                refCounts[key] = count - 1;
            }
        }
    }

    #endregion

    private void OnDestroy()
    {
        ReleaseAll();
    }
}