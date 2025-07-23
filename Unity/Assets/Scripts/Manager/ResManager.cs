using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class ResManager : Singleton<ResManager>
{
    // 资源句柄字典
    private readonly Dictionary<string, AsyncOperationHandle> resHandles = new Dictionary<string, AsyncOperationHandle>();

    #region 单个资源加载 (LoadAssetAsync)

    /// <summary>
    /// 加载单个资源
    /// </summary>
    public async UniTaskVoid LoadAssetAsync<T>(string key, Action<T> onLoaded = null)
    {
        // 检查缓存
        if (TryGetCachedResource<T>(key, out T cachedAsset, out AsyncOperationHandle cachedHandle))
        {
            onLoaded?.Invoke(cachedAsset);
            return;
        }
        try
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            resHandles[key] = handle;
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"加载资源 {key} 失败: {handle.OperationException}");
                resHandles.Remove(key);
                Addressables.Release(handle);
            }
        }
        catch (Exception e)
        {
            if (resHandles.TryGetValue(key, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle);
                resHandles.Remove(key);
            }
            Debug.LogError($"加载资源 {key} 失败: {e.Message}");
        }
    }

    #endregion

    #region 多个资源加载 (LoadAssetsAsync)

    /// <summary>
    /// 加载多个资源
    /// </summary>
    public async UniTaskVoid LoadAssetsAsync<T>(string key, Action<T> onLoaded = null, Action<IList<T>> onAllLoaded = null)
    {
        // 检查缓存
        if (TryGetCachedResource<IList<T>>(key, out IList<T> cachedAssets, out var cachedHandle))
        {
            if (onLoaded != null)
            {
                foreach (T asset in cachedAssets)
                {
                    onLoaded.Invoke(asset);
                }
            }
            onAllLoaded?.Invoke(cachedAssets);
            return;
        }

        try
        {
            var handle = Addressables.LoadAssetsAsync<T>(key, loadedAsset =>
            {
                onLoaded?.Invoke(loadedAsset);
            });

            resHandles[key] = handle;

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onAllLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"加载资源 {key} 失败: {handle.OperationException}");
                resHandles.Remove(key);
                Addressables.Release(handle);
            }
        }
        catch (Exception e)
        {
            resHandles.Remove(key);
            Debug.LogError($"加载资源 {key} 失败: {e.Message}");
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
            Addressables.Release(handle);
            Debug.LogError($"加载资源 {key} 失败: {handle.OperationException}");
        }

        resource = default;
        return false;
    }

    #endregion

    /// <summary>
    /// 释放单个资源
    /// </summary>
    public void Release(string key)
    {
        if (resHandles.TryGetValue(key, out AsyncOperationHandle handle))
        {
            Addressables.Release(handle);
            resHandles.Remove(key);
            Debug.Log($"资源 {key} 已释放");
        }
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    private void OnDestroy()
    {
        foreach (AsyncOperationHandle handle in resHandles.Values)
        {
            Addressables.Release(handle);
            Debug.Log($"资源 {handle.DebugName} 已释放");
        }
        resHandles.Clear();
    }
}