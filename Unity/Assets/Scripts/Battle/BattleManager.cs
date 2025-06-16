using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗管理器。
/// </summary>
public class BattleManager : MonoBehaviour
{
    /// <summary>
    /// 战场中的坦克。添加坦克、删除坦克、获取坦克、获取玩家控制的坦克
    /// </summary>
    public static Dictionary<long, BaseTank> tanks = new Dictionary<long, BaseTank>();

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        // 添加监听
        EventManager.Instance.RegisterEvent(Events.MsgBattleResult, OnMsgBattleResult);
        EventManager.Instance.RegisterEvent(Events.MsgLeaveBattle, OnMsgLeaveBattle);
        EventManager.Instance.RegisterEvent(Events.MsgSyncTank, OnMsgSyncTank);
        EventManager.Instance.RegisterEvent(Events.MsgFire, OnMsgFire);
        EventManager.Instance.RegisterEvent(Events.MsgHit, OnMsgHit);
    }

    /// <summary>
    /// 添加坦克
    /// </summary>
    public static void AddTank(long ID, BaseTank tank)
    {
        tanks[ID] = tank;
    }

    /// <summary>
    /// 删除坦克
    /// </summary>
    public static void RemoveTank(long ID)
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
    /// 收到战斗结束协议
    /// </summary>
    private static void OnMsgBattleResult(MsgBase msgBse)
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
    private static void OnMsgLeaveBattle(MsgBase msgBse)
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
    private static void OnMsgSyncTank(MsgBase msgBse)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBse;
        if (msg.ID == GameMain.ID) // 不能同步自己
            return;
        // 查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.ID);
        if (tank == null)
            return;
        tank.SyncPos(msg); // 移动同步
    }

    /// <summary>
    /// 收到开火协议
    /// </summary>
    private static void OnMsgFire(MsgBase msgBse)
    {
        MsgFire msg = (MsgFire)msgBse;
        if (msg.ID == GameMain.ID) // 不能同步自己
            return;
        // 查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.ID);
        if (tank == null)
            return;
        tank.SyncFire(msg); // 开火
    }

    /// <summary>
    /// 收到击中协议
    /// </summary>
    private static void OnMsgHit(MsgBase msgBse)
    {
        MsgHit msg = (MsgHit)msgBse;
        // 查找坦克
        BaseTank tank = GetTank(msg.targetID);
        if (tank == null)
            return;
        tank.Attacked(msg.ID, msg.damage); // 被击中
    }

    #endregion

    /// <summary>
    /// 开始战斗
    /// </summary>
    public void EnterBattle(TankInfo[] tanks)
    {
        PanelManager.Instance.Close<RoomPanelView>(); // 可以放到房间系统的监听中
        PanelManager.Instance.Close<ResultPanel>();
        foreach (var tank in tanks) // 生成坦克
        {
            GenerateTank(tank);
        }
    }

    /// <summary>
    /// 生成坦克
    /// </summary>
    private static void GenerateTank(TankInfo tankInfo)
    {
        // GameObject
        string objName = $"Tank_{tankInfo.ID}";
        GameObject tankObj = new GameObject(objName);
        // AddComponent
        BaseTank tank;
        if (tankInfo.ID == GameMain.ID)
        {
            tank = tankObj.AddComponent<CtrlTank>();
            tankObj.AddComponent<CameraFollow>();
        }
        else
            tank = tankObj.AddComponent<SyncTank>();
        // 属性
        tank.camp = tankInfo.camp;
        tank.ID = tankInfo.ID;
        tank.hp = tankInfo.hp;
        // pos rot
        Vector3 pos = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        Vector3 rot = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);
        tank.transform.position = pos;
        tank.transform.eulerAngles = rot;
        // Init
        if (tankInfo.camp == 1)
            tank.Init("Tank_1");
        else
            tank.Init("Tank_2");
        // 列表
        AddTank(tankInfo.ID, tank);
    }
}