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

    private Transform parent;

    void Start()
    {
        parent = this.transform;
        GenerateMap();
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

        // 生成随机障碍物
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(2, mapSize - 2), 2, Random.Range(2, mapSize - 2));

            GameObject obstacle = Instantiate(destructiblePrefab, spawnPos, Quaternion.identity);
            obstacle.transform.localScale = new Vector3(Random.Range(1f, 3f), Random.Range(minObstacleHeight, maxObstacleHeight), Random.Range(1f, 3f));
            obstacle.name = $"obstacle_{i}";
            obstacle.transform.parent = parent;
        }
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateMap();
        }
    }
}