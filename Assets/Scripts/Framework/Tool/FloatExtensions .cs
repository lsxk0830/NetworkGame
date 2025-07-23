using System;

public static class FloatExtensions
{
    /// <summary>
    /// 将浮点数四舍五入到指定的十进制位数
    /// </summary>
    /// <param name="value">要四舍五入的浮点数</param>
    /// <param name="decimals">保留的十进制位数</param>
    /// <returns></returns>
    public static float RoundTo(this float value, int decimals)
    {
        return (float)Math.Round(value, decimals, MidpointRounding.AwayFromZero);
    }
}