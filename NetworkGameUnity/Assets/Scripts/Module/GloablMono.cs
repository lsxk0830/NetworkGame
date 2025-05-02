using System;
using System.Threading;
using SimpleFrame;

public class GloablMono : MonoSingleton<GloablMono>
{
    private SynchronizationContext unityContext;

    public Action OnUpdate;
    public Action OnFixedUpdate;
    public Action OnLateUpdate;

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
        OnUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        OnLateUpdate?.Invoke();
    }

    private void OnDestroy()
    {
        OnUpdate = null;
        OnFixedUpdate = null;
        OnLateUpdate = null;
    }
}