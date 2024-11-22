using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleFrame
{
    public interface IAddressablesUtility : IUtility
    {
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名</param>
        /// <param name="callback">异步回调</param>
        void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callback);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名</param>

        void Release<T>(string name);

        /// <summary>
        /// 清空资源【不建议使用】,一般切换场景时使用
        /// </summary>
        void Clear();
    }

    public class AddressablesUtility : IAddressablesUtility
    {
        private Dictionary<string, IEnumerator> resDic = new Dictionary<string, IEnumerator>();

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名</param>
        /// <param name="callback">异步回调</param>
        public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callback)
        {
            string keyName = $"{name}_{typeof(T)}";
            AsyncOperationHandle<T> handle;
            if (resDic.ContainsKey(keyName))
            {
                handle = (AsyncOperationHandle<T>)resDic[keyName];
                if (handle.IsDone)
                    callback?.Invoke(handle);
                else
                {
                    handle.Completed += (obj) =>
                    {
                        if (obj.Status == AsyncOperationStatus.Succeeded)
                            callback?.Invoke(handle);
                    };
                }
                return;
            }
            handle = Addressables.LoadAssetAsync<T>(name);
            handle.Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                    callback?.Invoke(handle);
                else
                {
                    this.Warning($"{keyName}资源加载失败");
                    if (resDic.ContainsKey(keyName))
                        resDic.Remove(keyName);
                }
            };
            resDic.Add(keyName, handle);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名</param>
        public void Release<T>(string name)
        {
            string keyName = $"{name}_{typeof(T)}";
            if (resDic.ContainsKey(keyName))
            {
                AsyncOperationHandle<T> handle = (AsyncOperationHandle<T>)resDic[keyName];
                Addressables.Release(handle);
                resDic.Remove(keyName);
            }
        }

        /// <summary>
        /// 清空资源【不建议使用】,一般切换场景时使用
        /// </summary>
        public void Clear()
        {
            resDic.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}