using UnityEngine;

/// <summary>
/// 战斗场景特效管理器
/// </summary>
public static class EffectManager
{
    public static GameObject DiePrefab; // 死亡火焰
    public static GameObject HitPrefab; // 受伤爆炸

    public static void Destroy()
    {
        DiePrefab = null;
        HitPrefab = null;
    }
}
