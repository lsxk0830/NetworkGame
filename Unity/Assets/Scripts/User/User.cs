using System;

/// <summary>
/// 玩家数据
/// </summary>
public class User
{
    /// <summary>
    /// 用户唯一ID
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 金币数
    /// </summary>
    public int Coin { get; set; }

    /// <summary>
    /// 钻石数
    /// </summary>
    public int Diamond { get; set; }

    /// <summary>
    /// 胜利数
    /// </summary>
    public int Win { get; set; }

    /// <summary>
    /// 失败数
    /// </summary>
    public int Lost { get; set; }

    /// <summary>
    /// 用户头像
    /// </summary>
    public string AvatarPath { get; set; }

    /// <summary>
    /// 创建账户时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 上次登录时间
    /// </summary>
    public DateTime LastLogin { get; set; }
}