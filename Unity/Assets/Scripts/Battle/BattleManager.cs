using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗管理器。
/// </summary>
public class BattleManager : MonoSingleton<BattleManager>
{
    /// <summary>
    /// 战场中的坦克。添加坦克、删除坦克、获取坦克、获取玩家控制的坦克
    /// </summary>
    public static Dictionary<long, BaseTank> tanks;
    private Transform tankParent;
    private List<string> handles;
    private int friend;

    public GameObject BulletPrefab;
    public GameObject DiePrefab;
    public GameObject HitPrefab;

    protected override void OnAwake()
    {
        EventManager.Instance.RegisterEvent(Events.MsgEnterBattle, OnMsgEnterBattle);
        EventManager.Instance.RegisterEvent(Events.MsgBattleResult, OnMsgBattleResult);
        EventManager.Instance.RegisterEvent(Events.MsgLeaveBattle, OnMsgLeaveBattle);
        EventManager.Instance.RegisterEvent(Events.MsgSyncTank, OnMsgSyncTank);
        EventManager.Instance.RegisterEvent(Events.MsgFire, OnMsgFire);
        EventManager.Instance.RegisterEvent(Events.MsgHit, OnMsgHit);

        handles = new List<string>();
        tanks = new Dictionary<long, BaseTank>();
        tankParent = new GameObject("Tanks").transform;
        tankParent.position = Vector3.zero;

        ResManager.Instance.LoadAssetAsync<GameObject>("BulletPrefab", true,
        handle =>BulletPrefab = handle.gameObject,
        error => Debug.LogError($"Bullet Addressable加载失败")).Forget();

        ResManager.Instance.LoadAssetAsync<GameObject>("Die", true,
        handle => DiePrefab = handle.gameObject,
        error => Debug.LogError($"Die Addressable加载失败")).Forget();

        ResManager.Instance.LoadAssetAsync<GameObject>("Hit", true,
        handle => HitPrefab = handle.gameObject,
        error => Debug.LogError($"Hit Addressable加载失败")).Forget();
    }

    void OnDestroy()
    {
        EventManager.Instance.RemoveEvent(Events.MsgEnterBattle, OnMsgEnterBattle);
        EventManager.Instance.RemoveEvent(Events.MsgBattleResult, OnMsgBattleResult);
        EventManager.Instance.RemoveEvent(Events.MsgLeaveBattle, OnMsgLeaveBattle);
        EventManager.Instance.RemoveEvent(Events.MsgSyncTank, OnMsgSyncTank);
        EventManager.Instance.RemoveEvent(Events.MsgFire, OnMsgFire);
        EventManager.Instance.RemoveEvent(Events.MsgHit, OnMsgHit);

        tanks.Clear(); tanks = null;
        foreach (var handle in handles)
        {
            ResManager.Instance.ReleaseResource(handle);
        }
        handles.Clear(); handles = null;
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
    public BaseTank GetCtrlTank()
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
        foreach (var tankInfo in msg.tanks) // 生成坦克
        {
            Init(tankInfo);
        }
    }

    /// <summary>
    /// 收到战斗结束协议
    /// </summary>
    private void OnMsgBattleResult(MsgBase msgBse)
    {
        MsgEndBattle msg = (MsgEndBattle)msgBse;
        // 判断显示胜利还是失败
        bool isWin = false;
        BaseTank tank = GetCtrlTank();
        if (tank != null && tank.camp == msg.winCamp)
            isWin = true;
        PanelManager.Instance.Open<ResultPanel>(isWin);
    }

    /// <summary>
    /// 收到玩家退出协议
    /// </summary>
    private void OnMsgLeaveBattle(MsgBase msgBse)
    {
        MsgLeaveBattle msg = (MsgLeaveBattle)msgBse;
        // 查找坦克
        BaseTank tank = GetTank(msg.ID);
        if (tank == null)
            return;
        // 删除坦克
        RemoveTank(msg.ID);
        GameObject.Destroy(tank.gameObject);
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
        if (msg.ID == GameMain.ID) return;// 不能同步自己
        // 查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.ID);
        if (tank == null) return;
        tank.SyncFire(msg); // 开火
    }

    /// <summary>
    /// 收到击中协议
    /// </summary>
    private void OnMsgHit(MsgBase msgBse)
    {
        MsgHit msg = (MsgHit)msgBse;
        // 查找坦克
        BaseTank tank = GetTank(msg.targetID);
        if (tank == null)
            return;
        tank.Attacked(msg.ID, msg.damage); // 被击中
    }

    #endregion

    private void Init(Player tankInfo)
    {
        ResManager.Instance.LoadAssetAsync<GameObject>($"Tank_{tankInfo.skin}", false, handle =>
        {
            handles.Add($"Tank_{tankInfo.skin}");
            GameObject tank = Instantiate(handle);
            tank.transform.parent = tankParent.transform;
            BaseTank baseTank;
            if (tankInfo.ID == GameMain.ID)
            {
                baseTank = tank.AddComponent<CtrlTank>();
                friend = tankInfo.camp;
            }
            else
                baseTank = tank.AddComponent<SyncTank>();
            baseTank.camp = tankInfo.camp;
            baseTank.tag = $"Camp{tankInfo.camp}";
            tanks.Add(tankInfo.ID, baseTank);
            baseTank.Init(tankInfo);
        }).Forget();
    }
}