using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图生成器
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("地图设置")]
    [LabelText("地图边长")] public int mapSize = 50;
    [LabelText("障碍物方块")] public GameObject destructiblePrefab;
    [LabelText("地面")] public GameObject Ground;
    private Dictionary<int, ObstacleListener> obstacles;
    private Transform parent;

    void Start()
    {
        parent = this.transform;
        GenerateMap();
        NetManager.Instance.Send(new MsgObstacleAll());
        EventManager.Instance.RegisterEvent(Events.MsgObstacleAll, OnAllObstacle);
        EventManager.Instance.RegisterEvent(Events.MsgObstacleOne, OnOneObstacle);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveEvent(Events.MsgObstacleAll, OnAllObstacle);
        EventManager.Instance.RemoveEvent(Events.MsgObstacleOne, OnOneObstacle);
        obstacles?.Clear();
        obstacles = null;
    }

    void GenerateMap()
    {
        // 生成地面
        GameObject ground = Instantiate(Ground);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(mapSize / 10f, 1, mapSize / 10f);
        ground.transform.position = new Vector3(mapSize / 2f, 0, mapSize / 2f);
        ground.transform.parent = parent;

        // 创建四周不可破坏的墙
        CreateWall(new Vector3(mapSize / 2f, 1f, 0), new Vector3(mapSize, 2f, 1f));       // 下
        CreateWall(new Vector3(mapSize / 2f, 1f, mapSize), new Vector3(mapSize, 2f, 1f));  // 上
        CreateWall(new Vector3(0, 1f, mapSize / 2f), new Vector3(1f, 2f, mapSize));       // 左
        CreateWall(new Vector3(mapSize, 1f, mapSize / 2f), new Vector3(1f, 2f, mapSize));// 右
    }

    /// <summary>
    /// 生成墙壁
    /// </summary>
    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "wall";
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.tag = "Indestructible"; // 不可被破坏
        wall.GetComponent<Renderer>().material.color = Color.black;
        wall.transform.parent = parent;
    }

    /// <summary>
    /// 收到所有障碍物体协议
    /// </summary>
    private void OnAllObstacle(MsgBase msgBse)
    {
        MsgObstacleAll msg = (MsgObstacleAll)msgBse;

        if (obstacles == null)
        {
            //Debug.LogError($"收到障碍协议");
            obstacles = new Dictionary<int, ObstacleListener>(msg.PosRotScales.Count);

            for (int i = 0; i < msg.PosRotScales.Count; i++)
            {
                Vector3 pos = new Vector3(msg.PosRotScales[i].PosRotScale.PosX, msg.PosRotScales[i].PosRotScale.PosY, msg.PosRotScales[i].PosRotScale.PosZ);
                Vector3 rot = new Vector3(msg.PosRotScales[i].PosRotScale.RotX, msg.PosRotScales[i].PosRotScale.RotY, msg.PosRotScales[i].PosRotScale.RotZ);
                GameObject obstacle = Instantiate(destructiblePrefab, pos, Quaternion.Euler(rot));
                obstacle.transform.localScale = new Vector3(msg.PosRotScales[i].PosRotScale.ScaleX, msg.PosRotScales[i].PosRotScale.ScaleY, msg.PosRotScales[i].PosRotScale.ScaleZ);
                obstacle.name = msg.PosRotScales[i].ObstacleID.ToString();
                obstacle.transform.parent = parent;
                ObstacleListener ol = obstacle.AddComponent<ObstacleListener>();
                ol.Init();
                obstacles.Add(msg.PosRotScales[i].ObstacleID, ol);
            }
        }
        EventManager.Instance.RemoveEvent(Events.MsgObstacleAll, OnAllObstacle);
    }

    /// <summary>
    /// 收到单个障碍物体协议
    /// </summary>
    private void OnOneObstacle(MsgBase msgBse)
    {
        MsgObstacleOne msg = (MsgObstacleOne)msgBse;

        if (obstacles.TryGetValue(msg.ObstacleID, out ObstacleListener ol))
        {
            if (msg.IsDestory)
            {
                Destroy(ol.gameObject);
                obstacles.Remove(msg.ObstacleID);
            }
            else
            {
                ol.UpdateInfo(msg.PosRotScale);
            }
        }
    }
}