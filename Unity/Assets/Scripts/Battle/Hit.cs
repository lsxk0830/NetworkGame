using UnityEngine;

/// <summary>
/// Hit类用于处理击中效果，包括爆炸、环形效果和贴花效果。
/// </summary>
public class Hit : MonoBehaviour
{
    // public ParticleSystem Exposion;
    // public ParticleSystem Ring;
    // public ParticleSystem Decal;
    public ParticleSystem Effect;

    public void PoolInit(Vector3 pos)
    {
        transform.position = pos;
        Effect.Play();
        // Exposion.Play();
        // Ring.Play();
        // Decal.Play();

    }

    void OnParticleSystemStopped()
    {
        this.PushGameObject(this.gameObject); // 将Hit归还对象池
    }
}
