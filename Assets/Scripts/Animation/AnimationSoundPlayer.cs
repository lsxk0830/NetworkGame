using UnityEngine;

/// <summary>
/// 动画控制播放音效
/// </summary>
public class AnimationSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;


    // 动画事件调用的方法
    public void PlaySound()
    {
        audioSource.Play();
    }
}