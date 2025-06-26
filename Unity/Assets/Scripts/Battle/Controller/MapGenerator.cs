using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图生成器
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("地图设置")]
    [LabelText("地图边长")] public int mapSize = 50;
    [LabelText("可破坏预制件")] public GameObject destructiblePrefab;
    [LabelText("地面")] public GameObject Ground;
    private Dictionary<string, ObstacleListener> obstacles;
    private Transform parent;
    private MsgObstacle msg;

    void Start()
    {
        parent = this.transform;
        GenerateMap();
        NetManager.Send(new MsgObstacle());
        EventManager.Instance.RegisterEvent(Events.MsgObstacle, OnObstacle);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveEvent(Events.MsgObstacle, OnObstacle);
        msg = null;
        obstacles.Clear();
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
    /// 收到障碍协议
    /// </summary>
    private void OnObstacle(MsgBase msgBse)
    {
        msg = (MsgObstacle)msgBse;
        GloablMono.Instance.TriggerFromOtherThread(() =>
        {
            if (obstacles == null)
            {
                Debug.LogError($"收到障碍协议");
                obstacles = new Dictionary<string, ObstacleListener>(msg.PosRotScales.Count);

                for (int i = 0; i < msg.PosRotScales.Count; i++)
                {
                    Vector3 pos = new Vector3(msg.PosRotScales[i].PosX, msg.PosRotScales[i].PosY, msg.PosRotScales[i].PosZ);
                    Vector3 rot = new Vector3(msg.PosRotScales[i].RotX, msg.PosRotScales[i].RotY, msg.PosRotScales[i].RotZ);
                    GameObject obstacle = Instantiate(destructiblePrefab, pos, Quaternion.Euler(rot));
                    ObstacleListener ol = obstacle.AddComponent<ObstacleListener>();
                    obstacles.Add(msg.PosRotScales[i].ObstacleID, ol);
                    obstacle.transform.localScale = new Vector3(msg.PosRotScales[i].ScaleX, msg.PosRotScales[i].ScaleY, msg.PosRotScales[i].ScaleZ);
                    obstacle.name = msg.PosRotScales[i].ObstacleID;
                    obstacle.transform.parent = parent;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(msg.destoryID) && obstacles.TryGetValue(msg.destoryID, out ObstacleListener ol))
                {
                    Destroy(ol.gameObject);
                    obstacles.Remove(msg.destoryID);
                }
                foreach (var item in msg.PosRotScales)
                {
                    if (obstacles.TryGetValue(item.ObstacleID, out ObstacleListener obl))
                    {
                        obl.UpdateInfo(item);
                    }
                }
            }
        });
    }
}