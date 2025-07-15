using System;

/// <summary>
/// 玩家信息
/// </summary>
[Serializable]
public class Player
{
    /// <summary>
    /// 账号ID
    /// </summary>
    public long ID;

    /// <summary>
    /// 是否是房主 1-房主 0-普通成员
    /// </summary>
    public int isOwner = 0;

    /// <summary>
    /// 阵营 1-第一个阵营 2-第二个阵营
    /// </summary>
    public int camp = 0;

    // 坐标和旋转
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;

    /// <summary>
    /// 坦克生命值
    /// </summary>
    public int hp = 0;

    /// <summary>
    /// 皮肤ID
    /// </summary>
    public int skin;

    /// <summary>
    /// 玩家头像路径
    /// </summary>
    public string AvatarPath;
}