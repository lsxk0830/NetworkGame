using System;

[Serializable]
public class TankInfo
{
    /// <summary>
    /// 玩家id
    /// </summary>
    public string id = "";
    /// <summary>
    /// 玩家阵营
    /// </summary>
    public int camp = 0;
    /// <summary>
    /// 玩家生命值
    /// </summary>
    public int hp = 0;
    // 位置
    public float x = 0;
    public float y = 0;
    public float z = 0;
    // 旋转
    public float ex = 0;
    public float ey = 0;
    public float ez = 0;
}