using UnityEngine;

/// <summary>
/// 同步坦克类。预测信息，哪个时间到哪个位置
/// </summary>
public class SyncTank : BaseTank
{
    private Vector3 lastPosition; // 上一帧位置
    private float lastTime; // 上一次更新的时间

    public override void Init(Player tankInfo)
    {
        base.Init(tankInfo);
        lastPosition = transform.position;
        // 不受物理运动影响
        mRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        mRigidbody.useGravity = false;
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    /// <summary>
    /// 移动同步
    /// </summary>
    public void SyncPos(MsgSyncTank msg)
    {
        // 预测位置
        transform.position = new Vector3(msg.x / BattleManager.AcceptsScale, msg.y / BattleManager.AcceptsScale, msg.z / BattleManager.AcceptsScale);
        transform.eulerAngles = new Vector3(msg.ex / BattleManager.AcceptsScale, msg.ey / BattleManager.AcceptsScale, msg.ez / BattleManager.AcceptsScale);
        //forecastTime = Time.time;
        // 炮塔
        Vector3 le = turret.localEulerAngles;
        le.y = msg.turretY / BattleManager.AcceptsScale;
        turret.localEulerAngles = le;
        //Debug.Log($"同步位置协议:{JsonConvert.SerializeObject(msg)}");
    }

    private void OnUpdate()
    {
        if (hp <= 0) return;
        if (Time.time - lastTime > 0.5f)
        {
            lastTime = Time.time;

            if (Vector3.SqrMagnitude(transform.position - lastPosition) > 0.25f)
            {
                if (audioSource.volume == 0)
                    audioSource.volume = BattleManager.EffectValue; // 恢复音量
            }
            else
            {
                if (audioSource.volume != 0)
                    audioSource.volume = 0; // 恢复音量
            }

            lastPosition = transform.position;
        }
    }

    private void OnDestroy()
    {
        base.Destroy();
        GloablMono.Instance.OnUpdate -= OnUpdate;
    }
}