using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// 战斗管理器
/// </summary>
public class BattleManager : MonoBehaviour
{
    /// <summary>
    /// 战场中的坦克。添加坦克、删除坦克、获取坦克、获取玩家控制的坦克
    /// </summary>
    public static Dictionary<long, BaseTank> tanks;
    private Transform tankParent;
    private List<string> handles;

    public static CinemachineFreeLook freeLookCam;
    public static CinemachineImpulseSource impulseSource;
    public Camera mapCamera;

    public static bool SoundActive = false;
    public static float SoundValue = 0;

    void Awake()
    {
        BGMusicManager.Instance.ChangeOpen(false);
        SoundActive = PlayerPrefs.GetInt("Toggle_Sound") == 1 ? true : false;
        SoundValue = PlayerPrefs.GetFloat("Slider_Sound");

        //PanelManager.Instance.Open<GamePanel>();
        freeLookCam = GetComponent<CinemachineFreeLook>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        EventManager.Instance.RegisterEvent(Events.MsgEnterBattle, OnMsgEnterBattle);
        EventManager.Instance.RegisterEvent(Events.MsgEndBattle, OnMsgEndBattle);
        EventManager.Instance.RegisterEvent(Events.MsgSyncTank, OnMsgSyncTank);
        EventManager.Instance.RegisterEvent(Events.MsgFire, OnMsgFire);
        EventManager.Instance.RegisterEvent(Events.MsgHit, OnMsgHit);

        handles = new List<string>();
        tanks = new Dictionary<long, BaseTank>();
        tankParent = new GameObject("Tanks").transform;
        tankParent.position = Vector3.zero;

        ResManager.Instance.LoadAssetAsync<GameObject>("BulletPrefab",
        handle => EffectManager.BulletPrefab = handle.gameObject).Forget();

        ResManager.Instance.LoadAssetAsync<GameObject>("Die",
        handle => EffectManager.DiePrefab = handle.gameObject).Forget();

        ResManager.Instance.LoadAssetAsync<GameObject>("Hit",
        handle => EffectManager.HitPrefab = handle.gameObject).Forget();
    }

    void OnDestroy()
    {
        EventManager.Instance.RemoveEvent(Events.MsgEnterBattle, OnMsgEnterBattle);
        EventManager.Instance.RemoveEvent(Events.MsgEndBattle, OnMsgEndBattle);
        EventManager.Instance.RemoveEvent(Events.MsgSyncTank, OnMsgSyncTank);
        EventManager.Instance.RemoveEvent(Events.MsgFire, OnMsgFire);
        EventManager.Instance.RemoveEvent(Events.MsgHit, OnMsgHit);
        BulletManager.Clear(); // 清空子弹管理器
        EffectManager.Destroy();
        tanks.Clear(); tanks = null;
        ResManager.Instance.Release("BulletPrefab");
        ResManager.Instance.Release("Die");
        ResManager.Instance.Release("Hit");
        ResManager.Instance.Release("Tank_1");
        ResManager.Instance.Release("Tank_2");
        ResManager.Instance.Release("Tank_3");
        ResManager.Instance.Release("Tank_4");
        ResManager.Instance.Release("Tank_5");
        ResManager.Instance.Release("Tank_6");
    }

    /// <summary>
    /// 删除坦克
    /// </summary>
    public void RemoveTank(long ID)
    {
        tanks.Remove(ID);
    }

    /// <summary>
    /// 获取坦克
    /// </summary>
    public static BaseTank GetTank(long ID)
    {
        if (tanks.ContainsKey(ID))
            return tanks[ID];
        return null;
    }

    /// <summary>
    /// 获取玩家控制的坦克
    /// </summary>
    public static BaseTank GetCtrlTank()
    {
        return GetTank(GameMain.ID);
    }

    #region 网络协议监听

    /// <summary>
    /// 收到进入战斗协议
    /// </summary>
    private void OnMsgEnterBattle(MsgBase msgBse)
    {
        Debug.Log($"收到进入战斗协议");
        MsgEnterBattle msg = (MsgEnterBattle)msgBse;
        if (msg.result != 0)
        {
            PanelManager.Instance.Open<TipPanel>("进入战斗失败");
            return;
        }
        if (msg.tanks == null || msg.tanks.Length < 2) // 没有坦克信息
        {
            PanelManager.Instance.Open<TipPanel>("没有坦克信息");
            return;
        }
        foreach (var tankInfo in msg.tanks) // 生成坦克
        {
            Init(tankInfo);
        }
    }

    /// <summary>
    /// 收到战斗结束协议
    /// </summary>
    private void OnMsgEndBattle(MsgBase msgBse)
    {
        Cursor.lockState = CursorLockMode.None;
        Debug.Log($"收到战斗结束协议");
        MsgEndBattle msg = (MsgEndBattle)msgBse;
        // 判断显示胜利还是失败
        bool isWin = false;
        BaseTank tank = GetCtrlTank();
        if (tank != null && tank.camp == msg.winCamp)
        {
            UserManager.Instance.GetUser(GameMain.ID).Win++; // 更新玩家信息
            isWin = true;
        }
        else
        {
            UserManager.Instance.GetUser(GameMain.ID).Lost++; // 更新玩家信息
        }
        tank.hp = 0; // 设置坦克血量为0
        PanelManager.Instance.Open<ResultPanel>(isWin);
        BulletManager.Clear(); // 清空子弹管理器
        EffectManager.Destroy();
        EventManager.Instance.RemoveEvent(Events.MsgEnterBattle, OnMsgEnterBattle);
        EventManager.Instance.RemoveEvent(Events.MsgEndBattle, OnMsgEndBattle);
        EventManager.Instance.RemoveEvent(Events.MsgSyncTank, OnMsgSyncTank);
        EventManager.Instance.RemoveEvent(Events.MsgFire, OnMsgFire);
        EventManager.Instance.RemoveEvent(Events.MsgHit, OnMsgHit);
        // this.gameObject.ClearGameobjectPool(); // 清空对象池
        // this.ClearAll(); // 清空所有
    }

    /// <summary>
    /// 收到同步协议
    /// </summary>
    private void OnMsgSyncTank(MsgBase msgBse)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBse;
        if (msg.ID == GameMain.ID) // 不能同步自己
            return;
        // 查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.ID);
        if (tank == null) return;
        tank.SyncPos(msg); // 移动同步
    }

    /// <summary>
    /// 收到开火协议
    /// </summary>
    private void OnMsgFire(MsgBase msgBse)
    {
        MsgFire msg = (MsgFire)msgBse;
        BaseTank tank = msg.ID == GameMain.ID ? GetCtrlTank() : GetTank(msg.ID);
        tank.SyncFire(msg);
    }

    /// <summary>
    /// 收到击中协议
    /// </summary>
    private void OnMsgHit(MsgBase msgBse)
    {
        MsgHit msg = (MsgHit)msgBse;
        // 查找坦克
        BaseTank tank = GetTank(msg.targetID);
        if (tank == null) return;
        tank.hp = msg.hp; // 设置坦克血量
        if (tank.hp <= 0) tank.Die(); // 如果血量小于等于0，坦克死亡

    }

    #endregion

    private void Init(Player tankInfo)
    {
        ResManager.Instance.LoadAssetAsync<GameObject>($"Tank_{tankInfo.skin}", handle =>
        {
            handles.Add($"Tank_{tankInfo.skin}");
            GameObject tank = Instantiate(handle);
            tank.transform.parent = tankParent.transform;
            BaseTank baseTank;
            if (tankInfo.ID == GameMain.ID)
            {
                baseTank = tank.AddComponent<CtrlTank>();
                mapCamera.GetComponent<CameraMove>().targetPlayer = tank.transform;
            }
            else
            {
                baseTank = tank.AddComponent<SyncTank>();
            }
            tanks.Add(tankInfo.ID, baseTank);
            baseTank.Init(tankInfo);

        }).Forget();
    }
}