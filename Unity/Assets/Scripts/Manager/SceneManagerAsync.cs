using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

public class SceneManagerAsync : MonoSingleton<SceneManagerAsync>
{
    private string currentSceneName; // 当前场景名称
    private string loadingSceneName = "LoadingScene"; // 加载场景名称
    private Action<float> onLoadingProgress; // 加载进度回调

    /// <summary>
    /// 设置加载进度回调
    /// </summary>
    public void SetLoadingProgressCallback(Action<float> callback)
    {
        onLoadingProgress = callback;
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="useLoadingScene">是否使用加载场景，游戏场景回主场景不需要</param>
    /// <param name="loadSceneMode">加载模式</param>
    public async UniTask LoadSceneAsync(string sceneName, bool useLoadingScene = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("场景名称不能为空");
            return;
        }

        if (useLoadingScene && !string.IsNullOrEmpty(loadingSceneName))
        {
            await LoadSceneInternal(loadingSceneName, LoadSceneMode.Additive);

            var operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            operation.allowSceneActivation = false;

            while (!operation.isDone) // 跟踪加载进度
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                onLoadingProgress?.Invoke(progress);
                if (operation.progress >= 0.9f)
                {
                    await UniTask.DelayFrame(1);// 等待一帧确保加载界面更新
                    // ToDo:可以在这里添加额外的加载逻辑或等待用户输入
                    operation.allowSceneActivation = true;
                }
                await UniTask.Yield();
            }
            Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);
            if (loadingScene.IsValid() && loadingScene.isLoaded)// 检查 LoadingScene 是否仍然有效（未被自动卸载）
            {
                await UnloadSceneAsync(loadingSceneName); // 卸载加载场景
            }
        }
        else
        {
            await LoadSceneInternal(sceneName, loadSceneMode);
        }
        currentSceneName = sceneName;
    }

    /// <summary>
    /// 异步卸载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public async UniTask UnloadSceneAsync(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("场景名称不能为空");
            return;
        }

        var operation = SceneManager.UnloadSceneAsync(sceneName);
        await operation;
    }

    /// <summary>
    /// 异步重新加载当前场景
    /// </summary>
    public async UniTask ReloadCurrentSceneAsync()
    {
        if (string.IsNullOrEmpty(currentSceneName))
        {
            Debug.LogError("没有当前场景记录");
            return;
        }

        await LoadSceneAsync(currentSceneName);
    }

    /// <summary>
    /// 异步加载附加场景（不卸载当前场景）
    /// </summary>
    public async UniTask LoadAdditiveSceneAsync(string sceneName)
    {
        await LoadSceneAsync(sceneName, false, LoadSceneMode.Additive);
    }

    /// <summary>
    /// 内部加载场景方法
    /// </summary>
    private async UniTask LoadSceneInternal(string sceneName, LoadSceneMode loadSceneMode)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        await operation;
    }

    /// <summary>
    /// 设置加载场景名称
    /// </summary>
    public void SetLoadingSceneName(string sceneName)
    {
        loadingSceneName = sceneName;
    }

    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
}