using UnityEngine;

/// <summary>
/// 动画控制播放粒子特效
/// </summary>
public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    // 动画事件调用的方法
    public void PlayParticleSystem()
    {
        ps.Play();
    }
}