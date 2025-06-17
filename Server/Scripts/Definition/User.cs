using System.ComponentModel.DataAnnotations;

/// <summary>
/// 玩家数据
/// </summary>
public class User
{
    /// <summary>
    /// 用户唯一ID
    /// </summary>
    [Key]
    public long ID { get; set; }

    /// <summary>
    /// 房间ID
    /// </summary>
    public string RoomID { get; set; } = "";

    /// <summary>
    /// 用户名
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [MaxLength(64)]
    public string PW { get; set; }

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
    [MaxLength(255)]
    public string AvatarPath { get; set; }

    /// <summary>
    /// 创建账户时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 上次登录时间
    /// </summary>
    public DateTime? LastLogin { get; set; }
}