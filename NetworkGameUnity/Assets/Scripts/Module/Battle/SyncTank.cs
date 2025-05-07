using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 同步坦克类。预测信息，哪个时间到哪个位置
/// </summary>
public class SyncTank : BaseTank
{
    // 预测信息，哪个时间到哪个位置
    //private Vector3 lastPos;
    //private Vector3 lastRot;
    //private Vector3 forecastPos;
    //private Vector3 forecastRot;
    //private float forecastTime;

    private void Start()
    {
        //GloablMono.Instance.OnUpdate += OnUpdate;
    }

    private void OnUpdate(float f)
    {
        //ForecastUpdate();
    }

    public override AsyncOperationHandle Init(string tankName)
    {
        AsyncOperationHandle option = base.Init(tankName);
        option.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 不受物理运动影响
                mRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                mRigidbody.useGravity = false;
                // 初始化预测信息
                //lastPos = transform.position;
                //lastRot = transform.eulerAngles;
                //forecastPos = transform.position;
                //forecastRot = transform.eulerAngles;
                //forecastTime = Time.time;
            }
        };
        return option;
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    public void ForecastUpdate()
    {
        // 时间
        //float t = (Time.time - forecastTime) / CtrlTank.syncInterval;
        //t = Mathf.Clamp(t, 0, 1); // 将一个数值限制在指定的范围之内。它可以防止数值超出你设定的最小值和最大值边界
        // 位置
        Vector3 pos = transform.position;
        //pos = Vector3.Lerp(pos, forecastPos, t);
        transform.position = pos;
        // 旋转
        Quaternion quat = transform.rotation;
        //Quaternion forecastQuat = Quaternion.Euler(forecastRot);
        //quat = Quaternion.Lerp(quat, forecastQuat, t);
        transform.rotation = quat;
    }

    /// <summary>
    /// 移动同步
    /// </summary>
    public void SyncPos(MsgSyncTank msg)
    {
        // 预测位置
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        //forecastPos = pos + 2 * (pos - lastPos);
        //forecastRot = rot + 2 * (rot - lastRot);
        //// 更新
        //lastPos = pos;
        //lastRot = rot;
        transform.position = pos;
        transform.eulerAngles = rot;
        //forecastTime = Time.time;
        // 炮塔
        Vector3 le = turret.localEulerAngles;
        le.y = msg.turretY;
        turret.localEulerAngles = le;
    }

    /// <summary>
    /// 开火
    /// </summary>
    public void SyncFire(MsgFire msg)
    {
        Bullet bullet = Fire();
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