using System;
using System.Collections.Generic;

/// <summary>
/// 子弹管理类,子弹ID和子弹脚本进行绑定
/// </summary>
public class BulletManager
{
    private static Dictionary<Guid, Bullet> bulletDic = new Dictionary<Guid, Bullet>();

    public static void AddBullet(Bullet bullet)
    {
        bulletDic.Add(bullet.bulletID, bullet);
    }

    public static void RemoveBullet(Guid bulletID)
    {
        if (bulletDic.ContainsKey(bulletID))
            bulletDic.Remove(bulletID);
    }

    public static Bullet GetBullet(Guid bulletID)
    {
        bulletDic.TryGetValue(bulletID, out Bullet bullet);
        return bullet;
    }

    public static void Clear()
    {
        bulletDic.Clear();
    }
}