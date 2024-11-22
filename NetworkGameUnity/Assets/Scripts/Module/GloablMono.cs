using System;
using SimpleFrame;
using UnityEngine;

public class GloablMono : MonoSingleton<GloablMono>
{
    public Action<float> OnUpdate;
    public Action<float> OnFixedUpdate;
    public Action<float> OnLateUpdate;

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