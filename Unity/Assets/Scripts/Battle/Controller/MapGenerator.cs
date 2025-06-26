using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图生成器
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("地图设置")]
    [LabelText("地图边长")] public int mapSize = 50;
    [LabelText("障碍物数量")] public int obstacleCount = 30;
    [LabelText("最小障碍物高度")] public float minObstacleHeight = 1f;
    [LabelText("最大障碍物高度")] public float maxObstacleHeight = 3f;
    [LabelText("可破坏预制件")] public GameObject destructiblePrefab;
    [LabelText("地面")] public GameObject Ground;
    private List<GameObject> obstacles;
    private Transform parent;
    private MsgObstacle msg;

    void Start()
    {
        parent = this.transform;
        GenerateMap();
        EventManager.Instance.RegisterEvent(Events.MsgObstacle, OnObstacle);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveEvent(Events.MsgObstacle, OnObstacle);
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

    void Update()
    {
        if (obstacles == null) return;
        for (int i = 0; i < obstacles.Count; i++)
        {
            msg.PosRotScales[i].ObstacleID = obstacles[i].name;
            msg.PosRotScales[i].PosX = obstacles[i].transform.position.x;
            msg.PosRotScales[i].PosY = obstacles[i].transform.position.x;
            msg.PosRotScales[i].PosZ = obstacles[i].transform.position.x;
            msg.PosRotScales[i].RotX = obstacles[i].transform.rotation.eulerAngles.x;
            msg.PosRotScales[i].RotY = obstacles[i].transform.rotation.eulerAngles.y;
            msg.PosRotScales[i].RotZ = obstacles[i].transform.rotation.eulerAngles.z;
            msg.PosRotScales[i].ScaleX = obstacles[i].transform.localScale.x;
            msg.PosRotScales[i].ScaleY = obstacles[i].transform.localScale.y;
            msg.PosRotScales[i].ScaleX = obstacles[i].transform.localScale.z;
        }
    }

    /// <summary>
    /// 收到障碍协议
    /// </summary>
    private void OnObstacle(MsgBase msgBse)
    {
        msg = (MsgObstacle)msgBse;
        if (obstacles == null)
        {
            obstacles = new List<GameObject>(msg.PosRotScales.Length);
            for (int i = 0; i < msg.PosRotScales.Length; i++)
            {
                Vector3 pos = new Vector3(msg.PosRotScales[i].PosX, msg.PosRotScales[i].PosY, msg.PosRotScales[i].PosZ);
                Vector3 rot = new Vector3(msg.PosRotScales[i].RotX, msg.PosRotScales[i].RotY, msg.PosRotScales[i].RotZ);
                GameObject obstacle = Instantiate(destructiblePrefab, pos, Quaternion.Euler(rot));
                obstacles.Add(obstacle);
                obstacle.transform.localScale = new Vector3(msg.PosRotScales[i].ScaleX, msg.PosRotScales[i].ScaleY, msg.PosRotScales[i].ScaleZ);
                obstacle.name = msg.PosRotScales[i].ObstacleID;
                obstacle.transform.parent = parent;
            }
        }
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].name = msg.PosRotScales[i].ObstacleID;
            obstacles[i].transform.position = new Vector3(msg.PosRotScales[i].PosX, msg.PosRotScales[i].PosY, msg.PosRotScales[i].PosZ);
            obstacles[i].transform.rotation = Quaternion.Euler(new Vector3(msg.PosRotScales[i].RotX, msg.PosRotScales[i].RotY, msg.PosRotScales[i].RotZ));
            obstacles[i].transform.localScale = new Vector3(msg.PosRotScales[i].ScaleX, msg.PosRotScales[i].ScaleY, msg.PosRotScales[i].ScaleZ);
        }
    }
}