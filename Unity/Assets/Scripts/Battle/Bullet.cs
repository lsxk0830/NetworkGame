using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Bullet : MonoBehaviour
{
    public Guid bulletID;
    public long ID; // 发射者ID

    private Vector3 startPos; // 初始位置
    public Vector3 targetPos; // 目标位置
    private bool isMoving = true;

    private void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log($"子弹到爆炸：坐标 = {transform.position},collisionInfo = {collisionInfo.gameObject.name}");
        if (ID != GameMain.ID) return; // 不是自己发出的炮弹
        isMoving = false; // 停止移动
        MsgFire msg = this.GetObjInstance<MsgFire>();
        msg.ID = ID; // 发射者ID
        msg.bulletID = bulletID; // 子弹ID
        msg.tx = transform.position.x;
        msg.tz = transform.position.z;
        msg.x = 0;
        msg.z = 0;
        msg.IsExplosion = true;
        NetManager.Instance.Send(msg);
        GameObject collObj = collisionInfo.gameObject;
        if (collObj.tag == "Obstacle") // 碰撞到障碍物
        {
            MsgObstacleOne msgOne = this.GetObjInstance<MsgObstacleOne>();
            msgOne.ObstacleID = int.Parse(collObj.name);
            msgOne.IsDestory = true; //销毁
            NetManager.Instance.Send(msgOne);
            this.PushPool(msgOne);
            Destroy(collObj);
        }
        else if (collObj.tag != $"Camp{BattleManager.Instance.GetCtrlTank().camp}") // 碰撞到坦克
        {
            if (collObj.TryGetComponent<BaseTank>(out BaseTank hitTank))
            {
                SendMsgHit(ID, hitTank.ID);
            }
        }

        this.PushPool(msg); // 将消息对象归还对象池
        this.PushGameObject(this.gameObject); // 将子弹归还对象池
        BulletManager.RemoveBullet(bulletID); // 从字典中移除子弹
        this.GetGameObject(EffectManager.HitPrefab)
            .GetComponent<Hit>()
            .PoolInit(this.transform.position);
    }

    /// <summary>
    /// 发送伤害协议。attackTankID-发射者 hitTankID-被攻击坦克
    /// </summary>
    private void SendMsgHit(long attackTankID, long hitTankID)
    {
        if (hitTankID == 0 || hitTankID == 0)
            return;

        if (attackTankID != GameMain.ID) return;// 不是自己发出的炮弹
        MsgHit msg = this.GetObjInstance<MsgHit>();
        msg.targetID = hitTankID;
        msg.ID = attackTankID;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Instance.Send(msg);
        this.PushPool(msg);
    }

    /// <summary>
    /// 初始化子弹
    /// </summary>
    /// <param name="id">发射者ID</param>
    /// <param name="bulletGuid">子弹ID</param>
    /// <param name="startPos">初始位置</param>
    /// <param name="targetPos">初始旋转</param>
    public void PoolInit(long id, Guid bulletGuid, Vector3 startPos, Vector3 targetPos)
    {
        ID = id;
        bulletID = bulletGuid;
        transform.position = startPos;
        this.startPos = startPos; // 保存初始位置
        this.targetPos = targetPos;
        isMoving = true; // 设置为正在移动状态
        MoveBulletAsync().Forget();
    }

    public void PoolReset()
    {
        isMoving = false; // 停止移动
        this.PushGameObject(this.gameObject); // 将子弹归还对象池
    }

    private async UniTaskVoid MoveBulletAsync()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1f && isMoving)
        {
            transform.position = Vector3.Lerp(
                startPos,
                targetPos,
                elapsedTime / 1f
            );
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
        if (isMoving)
        {
            this.PushGameObject(this.gameObject); // 将子弹归还对象池
            BulletManager.RemoveBullet(bulletID); // 从字典中移除子弹
        }
    }
}