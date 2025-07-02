using UnityEngine;

/// <summary>
/// 同步坦克类。预测信息，哪个时间到哪个位置
/// </summary>
public class SyncTank : BaseTank
{
    public override void Init(Player tankInfo)
    {
        base.Init(tankInfo);
        // 不受物理运动影响
        mRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        mRigidbody.useGravity = false;
    }

    /// <summary>
    /// 移动同步
    /// </summary>
    public void SyncPos(MsgSyncTank msg)
    {
        // 预测位置
        transform.position = new Vector3(msg.x, msg.y, msg.z);
        transform.eulerAngles = new Vector3(msg.ex, msg.ey, msg.ez);
        //forecastTime = Time.time;
        // 炮塔
        Vector3 le = turret.localEulerAngles;
        le.y = msg.turretY;
        turret.localEulerAngles = le;
        //Debug.Log($"同步位置协议:{JsonConvert.SerializeObject(msg)}");
    }

    /// <summary>
    /// 开火
    /// </summary>
    public void SyncFire(MsgFire msg)
    {
        Vector3 pos = new Vector3(msg.x, 1, msg.z);
        Vector3 tPos = new Vector3(msg.tx, 1, msg.tz);
        if (msg.IsExplosion)
        {
            this.GetGameObject(EffectManager.HitPrefab)
                .GetComponent<Hit>()
                .PoolInit(tPos);
            BulletManager.GetBullet(msg.bulletID)?.PoolReset(); // 将子弹归还对象池
            Debug.Log($"爆炸位置:{tPos}");
            return;
        }
        Bullet bullet = Fire(msg.bulletID);
        // 更新坐标
        bullet.transform.position = pos;
    }
}