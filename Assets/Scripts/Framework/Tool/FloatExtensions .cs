using UnityEngine;

public static class FloatExtensions
{
    /// <summary>
    /// 保留小数点后四位
    /// </summary>
    public static float RoundTo4(this float value)
    {
        return Mathf.Round(value * 10000) / 10000f;
    }
}