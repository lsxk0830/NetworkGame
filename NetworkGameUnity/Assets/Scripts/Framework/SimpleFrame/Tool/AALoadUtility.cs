using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleFrame
{
    public interface IAALoadUtility : IUtility
    {
        void AALoadAsync<T>(string key, Action<T> callback);
        void AALoadAsync<T>(string key, Action<T, AsyncOperationHandle<T>> callback);
    }

    public class AALoadUtility : IAALoadUtility
    {
        public void Init()
        {

        }

        void IAALoadUtility.AALoadAsync<T>(string key, Action<T> callback)
        {
            Addressables.LoadAssetAsync<T>(key).Completed += (result) =>
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                    callback?.Invoke(result.Result);
                else
                {
                    string errorMessage = result.OperationException != null ? result.OperationException.Message : "Unknown error";
                    this.Error($"AA加载失败: 无法加载Key '{key}' 的资源. 错误: {errorMessage}");
                }
            };
        }

        void IAALoadUtility.AALoadAsync<T>(string key, Action<T, AsyncOperationHandle<T>> callback)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            handle.Completed += (result) =>
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                    callback?.Invoke(result.Result, handle);
                else
                {
                    string errorMessage = result.OperationException != null ? result.OperationException.Message : "Unknown error";
                    this.Error($"AA加载失败: 无法加载Key '{key}' 的资源. 错误: {errorMessage}");
                }
            };
        }
    }
}