using System;
using System.Threading;
using SimpleFrame;
using UnityEngine;

public class GloablMono : MonoSingleton<GloablMono>
{
    private SynchronizationContext unityContext;

    public Action<float> OnUpdate;
    public Action<float> OnFixedUpdate;
    public Action<float> OnLateUpdate;

    protected override void OnAwake()
    {
        // 记录主线程的 SynchronizationContext
        unityContext = SynchronizationContext.Current;
    }

    /// <summary>
    /// 主线程执行其他线程的代码
    /// </summary>
    public void TriggerFromOtherThread(Action action)
    {
        // 将操作调度到主线程
        unityContext.Post(_ => action?.Invoke(), null);
    }

    private void Update()
    {
        OnUpdate?.Invoke(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        OnFixedUpdate?.Invoke(Time.deltaTime);
    }

    private void LateUpdate()
    {
        OnLateUpdate?.Invoke(Time.deltaTime);
    }

    private void OnDestroy()
    {
        OnUpdate = null;
        OnFixedUpdate = null;
        OnLateUpdate = null;
    }
}