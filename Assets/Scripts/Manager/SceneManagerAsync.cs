using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

public class SceneManagerAsync : Singleton<SceneManagerAsync>
{
    private AsyncOperation currentOperation;

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="loadSceneMode">加载模式</param>
    public async UniTaskVoid LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("场景名称不能为空");
            return;
        }
        currentOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        currentOperation.allowSceneActivation = false; // 允许场景在准备就绪后立即激活。

        await currentOperation;
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
    /// 当前场景加载完成，进入场景，场景打开
    /// </summary>
    public void Success(Action<AsyncOperation> OnSuccess)
    {
        currentOperation.allowSceneActivation = true; // 允许场景激活
        currentOperation.completed += OnSuccess;
    }
}