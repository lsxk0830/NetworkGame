using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTest : MonoBehaviour
{
    void Start()
    {
        // 创建平面作为地面
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.localScale = new Vector3(10, 1, 10); // 调整地面大小
        ground.GetComponent<Renderer>().material.color = Color.gray; // 设为灰色

        // 在场景四周生成围墙
        CreateWall(new Vector3(0, 1, 50), new Vector3(1, 2, 100)); // 左边界
        CreateWall(new Vector3(100, 1, 50), new Vector3(1, 2, 100)); // 右边界
        CreateWall(new Vector3(50, 1, 0), new Vector3(100, 2, 1));  // 下边界
        CreateWall(new Vector3(50, 1, 100), new Vector3(100, 2, 1));// 上边界
    }

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.tag = "Wall";
        wall.GetComponent<Renderer>().material.color = Color.black; // 黑色边界
    }

    public GameObject destructibleWallPrefab;
    public GameObject unbreakableWallPrefab;
    public int mapSize = 20;  // 地图尺寸
    public float obstacleDensity = 0.3f;  // 障碍物密度

    void GenerateMap()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                // 边缘生成不可破坏墙
                if (x == 0 || z == 0 || x == mapSize - 1 || z == mapSize - 1)
                {
                    Instantiate(unbreakableWallPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
                }
                // 内部随机生成障碍
                else if (Random.value < obstacleDensity)
                {
                    Instantiate(Random.value > 0.5f ? destructibleWallPrefab : unbreakableWallPrefab,
                               new Vector3(x, 0.5f, z), Quaternion.identity);
                }
            }
        }
    }
}
