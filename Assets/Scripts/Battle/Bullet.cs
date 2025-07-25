using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f; // 子弹移动速度
    public float maxDistance = 200f;
    private float dis = 0.1f;// 只在移动距离足够时检测碰撞
    public LayerMask hitLayers; // 可命中的层级（如 Enemy | Friend | Bullet | CanDestroy）
    public Guid bulletID;
    public long ID; // 发射者ID

    private Vector3 startPos; // 初始位置
    private Vector3 lastPos;
    private bool isMove; // 子弹是否移动

    public AudioSource audioSource;

    /// <summary>
    /// 初始化子弹
    /// </summary>
    /// <param name="id">发射者ID</param>
    /// <param name="bulletGuid">子弹ID</param>
    /// <param name="startPos">初始位置</param>
    public void PoolInit(long id, Guid bulletGuid, Vector3 startPos)
    {
        ID = id;
        bulletID = bulletGuid;
        transform.position = startPos;
        this.startPos = startPos; // 保存初始位置
        lastPos = startPos;
        isMove = true;

        if (BattleManager.EffectActive)
        {
            audioSource.volume = BattleManager.EffectValue;
            audioSource.Play();// 子弹开火音效
        }

        GloablMono.Instance.OnUpdate += OnUpdate;
        GloablMono.Instance.OnFixedUpdate += OnFixedUpdate;
    }

    private void OnDisable()
    {
        GloablMono.Instance.OnUpdate -= OnUpdate;
        GloablMono.Instance.OnFixedUpdate -= OnFixedUpdate;
        BulletManager.RemoveBullet(bulletID); // 从字典中移除子弹
    }

    private void OnFixedUpdate()
    {
        if (ID != GameMain.ID || !isMove) return;

        // 只在移动距离足够时检测碰撞
        if ((transform.position - lastPos).sqrMagnitude > dis * dis)
        {
            // 检测两帧之间的命中
            if (Physics.Linecast(lastPos, transform.position, out RaycastHit hit, hitLayers))
            {
                isMove = false;// 标记子弹停止移动
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") &&
                    hit.collider.TryGetComponent(out SyncTank hitTank))
                {
                    MsgHit msgHit = this.GetObjInstance<MsgHit>();
                    msgHit.targetID = hitTank.ID; // 被击中坦克ID
                    msgHit.ID = ID; // 发射者ID
                    msgHit.x = hit.point.x.RoundTo(4);
                    msgHit.y = hit.point.y.RoundTo(4);
                    msgHit.z = hit.point.z.RoundTo(4);
                    NetManager.Instance.Send(msgHit);
                    this.PushPool(msgHit);
                    Debug.Log($"发送击中受伤协议)");
                }
                MsgFire msg = this.GetObjInstance<MsgFire>();
                msg.ID = ID; // 发射者ID
                msg.bulletID = bulletID; // 子弹ID
                msg.x = hit.point.x.RoundTo(4);
                msg.y = hit.point.y.RoundTo(4);
                msg.z = hit.point.z.RoundTo(4);
                msg.IsExplosion = true;
                NetManager.Instance.Send(msg);
                this.PushPool(msg); // 将消息对象归还对象池

                this.PushGameObject(this.gameObject); // 将子弹归还对象池
                this.GetGameObject(EffectManager.HitPrefab)
                    .GetComponent<Hit>()
                    .PoolInit(hit.point);
            }
            lastPos = transform.position;
        }
    }

    private void OnUpdate()
    {
        if (!isMove) return;

        // 移动子弹
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 超出射程销毁
        if ((transform.position - startPos).sqrMagnitude > maxDistance * maxDistance)
        {
            isMove = false;
            this.GetGameObject(EffectManager.HitPrefab)
                .GetComponent<Hit>()
                .PoolInit(this.transform.position);
            this.PushGameObject(gameObject);
        }
    }
}