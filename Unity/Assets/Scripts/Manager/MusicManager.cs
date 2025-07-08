using UnityEngine;

/// <summary>
/// 音乐管理器
/// </summary>
public class MusicManager : MonoSingleton<MusicManager>
{
    private AudioSource audioSource;

    protected override void OnAwake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 是否开启音乐
    /// </summary>
    public void ChangeOpen(bool isOpen)
    {
        audioSource.mute = !isOpen; //静音
    }

    /// <summary>
    /// 改变音乐大小
    /// </summary>
    public void ChangeValue(float f)
    {
        audioSource.volume = f;
    }
}
