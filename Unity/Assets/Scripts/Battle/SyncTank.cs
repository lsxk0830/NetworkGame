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
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    private void OnUpdate()
    {

    }

    /// <summary>
    /// 移动同步
    /// </summary>
    public void SyncPos(MsgSyncTank msg)
    {
        if (turret == null) return;

        // 预测位置
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        transform.position = pos;
        transform.eulerAngles = rot;
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
        if (msg.IsExplosion)
        {
            GloablMono.Instance.TriggerFromOtherThread(() =>
            {
                BulletDic[msg.bulletID].Explosion();
                BulletDic.Remove(msg.bulletID);
            });
            return;
        }
        Bullet bullet = Fire(msg.bulletID);
        bullet.bulletID = msg.bulletID;
        // 更新坐标
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        bullet.transform.position = pos;
        bullet.transform.eulerAngles = rot;
    }

    private void OnDestroy()
    {
        //GloablMono.Instance.OnUpdate -= OnUpdate;
    }
}