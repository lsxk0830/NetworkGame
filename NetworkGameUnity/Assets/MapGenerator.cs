using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("地图设置")]
    public int mapSize = 50;         // 地图边长
    public int obstacleCount = 30;   // 障碍物数量
    public float minObstacleHeight = 1f;
    public float maxObstacleHeight = 3f;

    [Header("预制体")]
    public GameObject destructiblePrefab;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // 生成地面
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.localScale = new Vector3(mapSize / 10f, 1, mapSize / 10f);
        ground.transform.position = new Vector3(mapSize / 2f, 0, mapSize / 2f);
        ground.GetComponent<Renderer>().material.color = Color.gray;

        // 创建四周不可破坏的墙
        CreateWall(new Vector3(mapSize / 2f, 1f, 0), new Vector3(mapSize, 2f, 1f));       // 下
        CreateWall(new Vector3(mapSize / 2f, 1f, mapSize), new Vector3(mapSize, 2f, 1f));  // 上
        CreateWall(new Vector3(0, 1f, mapSize / 2f), new Vector3(1f, 2f, mapSize));       // 左
        CreateWall(new Vector3(mapSize, 1f, mapSize / 2f), new Vector3(1f, 2f, mapSize));// 右

        // 生成随机障碍物
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(2, mapSize - 2), 0, Random.Range(2, mapSize - 2)
            );

            GameObject obstacle = Instantiate(destructiblePrefab, spawnPos, Quaternion.identity);
            obstacle.transform.localScale = new Vector3(Random.Range(1f, 3f), Random.Range(minObstacleHeight, maxObstacleHeight), Random.Range(1f, 3f)
            );
        }
    }

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.tag = "Indestructible";
        wall.GetComponent<Renderer>().material.color = Color.black;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateMap();
        }
    }
}