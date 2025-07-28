using UnityEngine;

public class BaseTank : MonoBehaviour
{
    public float steer = 20; // 转向速度
    public float turretSpeed = 30f; // 炮塔旋转速度
    public Transform turret; // 炮塔
    public Transform gun; // 炮管
    public Transform firePoint; // 发射点
    public float fired = 0.5f; // 炮弹Cd时间
    public float lastFireTime = 0; // 上一次发射炮弹时间
    public float hp = 100;
    public long ID; // 哪一玩家
    public int camp = 0; // 阵营
    protected Rigidbody mRigidbody;
    private GameObject explosion;
    public AudioSource audioSource; // 音频源
    public Animator Animator;

    public virtual void Init(Player tankInfo)
    {
        camp = tankInfo.camp;
        ID = tankInfo.ID;
        hp = tankInfo.hp;

        transform.position = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        transform.eulerAngles = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);

        mRigidbody = GetComponent<Rigidbody>();
        turret = transform.Find("Tank/Turret");
        gun = turret.transform.Find("Gun");
        firePoint = turret.transform.Find("FirePoint");
        Animator = turret.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.volume = BattleManager.EffectValue; // 设置音量
    }

    /// <summary>
    /// 被攻击
    /// </summary>
    /// <param name="attackID">攻击者ID</param>
    /// <param name="hp">剩余血量</param>
    /// <param name="att">攻击力</param>
    public void Die()
    {
        if (explosion != null) return; // 如果已经爆炸了，就不再处理

        explosion = this.GetGameObject(EffectManager.DiePrefab);
        explosion.transform.position = transform.position;
        explosion.transform.rotation = transform.rotation;
        mRigidbody.constraints = RigidbodyConstraints.FreezeAll; // 冻结刚体

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color32 color = new Color32(94, 94, 94, 255); // 灰色
        foreach (Renderer rend in renderers)
        {
            rend.GetPropertyBlock(mpb);  // 保留原有属性
            mpb.SetColor("_Color", color);
            rend.SetPropertyBlock(mpb);  // 仅影响当前Renderer
        }
    }

    protected virtual void Destroy()
    {
        explosion = null; // 清除爆炸特效引用
        mRigidbody = null;
        turret = null;
        gun = null;
        firePoint = null;
    }
}