using System;

/// <summary>
/// 玩家数据
/// </summary>
public class User
{
    /// <summary>
    /// 用户唯一ID
    /// </summary>
    public long ID;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Name;

    /// <summary>
    /// 金币数
    /// </summary>
    public int Coin;

    /// <summary>
    /// 钻石数
    /// </summary>
    public int Diamond;

    /// <summary>
    /// 胜利数
    /// </summary>
    public int Win;

    /// <summary>
    /// 失败数
    /// </summary>
    public int Lost;

    /// <summary>
    /// 用户头像
    /// </summary>
    public string AvatarPath;

    /// <summary>
    /// 创建账户时间
    /// </summary>
    public DateTime CreateTime;

    /// <summary>
    /// 上次登录时间
    /// </summary>
    public DateTime LastLogin;
}