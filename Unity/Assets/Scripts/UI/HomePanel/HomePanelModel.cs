using System.Collections.Generic;
using UnityEngine;

public class HomePanelModel
{
    // 玩家数据
    public long ID;
    public int winCount { get; set; }
    public int lostCount { get; set; }

    // 房间数据
    public List<Room> rooms = new List<Room>();

    // 坦克控制状态
    [System.NonSerialized] public bool isRotatingTank;
    [System.NonSerialized] public Vector3 lastMousePosition;
    public const float TankRotationSpeed = 18f;

    // 网络状态
    public bool isWaitingServerResponse;

    public User GetUser()
    {
        return UserManager.Instance.GetUser(ID);
    }
}