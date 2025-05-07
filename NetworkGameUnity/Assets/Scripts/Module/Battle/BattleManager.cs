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
    public static Dictionary<string, BaseTank> tanks = new Dictionary<string, BaseTank>();

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        // 添加监听
        NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
        NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);
        NetManager.AddMsgListener("MsgSyncTank", OnMsgSyncTank);
        NetManager.AddMsgListener("MsgFire", OnMsgFire);
        NetManager.AddMsgListener("MsgHit", OnMsgHit);
    }

    /// <summary>
    /// 添加坦克
    /// </summary>
    public static void AddTank(string id, BaseTank tank)
    {
        tanks[id] = tank;
    }

    /// <summary>
    /// 删除坦克
    /// </summary>
    public static void RemoveTank(string id)
    {
        tanks.Remove(id);
    }

    /// <summary>
    /// 获取坦克
    /// </summary>
    public static BaseTank GetTank(string id)
    {
        if (tanks.ContainsKey(id))
            return tanks[id];
        return null;
    }

    /// <summary>
    /// 获取玩家控制的坦克
    /// </summary>
    public static BaseTank GetCtrlTank()
    {
        return GetTank(GameMain.id);
    }

    /// <summary>
    /// 重置战场
    /// </summary>
    public static void Reset()
    {
        foreach (BaseTank tank in tanks.Values)
        {
            GameObject.Destroy(tank.gameObject);
        }
        tanks.Clear();
    }

    #region 网络协议监听

    /// <summary>
    /// 收到战斗协议
    /// </summary>
    private static void OnMsgEnterBattle(MsgBase msgBse)
    {
        MsgEnterBattle msg = (MsgEnterBattle)msgBse;
        EnterBattle(msg);
    }

    /// <summary>
    /// 收到战斗结束协议
    /// </summary>
    private static void OnMsgBattleResult(MsgBase msgBse)
    {
        MsgBattleResult msg = (MsgBattleResult)msgBse;
        // 判断显示胜利还是失败
        bool isWin = false;
        BaseTank tank = GetCtrlTank();
        if (tank != null && tank.camp == msg.winCamp)
            isWin = true;
        PanelManager.Open<ResultPanel>(isWin);
    }

    /// <summary>
    /// 收到玩家退出协议
    /// </summary>
    private static void OnMsgLeaveBattle(MsgBase msgBse)
    {
        MsgLeaveBattle msg = (MsgLeaveBattle)msgBse;
        // 查找坦克
        BaseTank tank = GetTank(msg.id);
        if (tank == null)
            return;
        // 删除坦克
        RemoveTank(msg.id);
        GameObject.Destroy(tank.gameObject);
    }

    /// <summary>
    /// 收到同步协议
    /// </summary>
    private static void OnMsgSyncTank(MsgBase msgBse)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBse;
        if (msg.id == GameMain.id) // 不能同步自己
            return;
        // 查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.id);
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
        if (msg.id == GameMain.id) // 不能同步自己
            return;
        // 查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.id);
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
        BaseTank tank = GetTank(msg.targetId);
        if (tank == null)
            return;
        tank.Attacked(msg.id, msg.damage); // 被击中
    }

    #endregion

    /// <summary>
    /// 开始战斗
    /// </summary>
    public static void EnterBattle(MsgEnterBattle msg)
    {
        Reset(); // 重置
        PanelManager.Close("RoomPanel"); // 可以放到房间系统的监听中
        PanelManager.Close("ResultPanel");
        foreach (var tank in msg.tanks) // 生成坦克
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
        string objName = $"Tank_{tankInfo.id}";
        GameObject tankObj = new GameObject(objName);
        // AddComponent
        BaseTank tank;
        if (tankInfo.id == GameMain.id)
        {
            tank = tankObj.AddComponent<CtrlTank>();
            tankObj.AddComponent<CameraFollow>();
        }
        else
            tank = tankObj.AddComponent<SyncTank>();
        // 属性
        tank.camp = tankInfo.camp;
        tank.id = tankInfo.id;
        tank.hp = tankInfo.hp;
        // pos rot
        Vector3 pos = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        Vector3 rot = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);
        tank.transform.position = pos;
        tank.transform.eulerAngles = rot;
        // Init
        if (tankInfo.camp == 1)
            tank.Init("TankC");
        else
            tank.Init("TankB");
        // 列表
        AddTank(tankInfo.id, tank);
    }
}