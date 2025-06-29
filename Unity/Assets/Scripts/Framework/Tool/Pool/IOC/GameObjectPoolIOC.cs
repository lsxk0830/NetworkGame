using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 从对象池拿的数据会被添加标记，回收时清除标记
/// </summary>
public class GameObjectPoolIOC
{
    /// <summary>
    /// 根节点
    /// </summary>
    private Transform RootTransform;

    private Dictionary<int, Queue<GameObject>> Pool = new Dictionary<int, Queue<GameObject>>();
    private static Dictionary<GameObject, int> GoID = new Dictionary<GameObject, int>();

    public GameObjectPoolIOC()
    {
        RootTransform = new GameObject("PoolRoot").transform;
        RootTransform.position = Vector3.zero;
        GameObject.DontDestroyOnLoad(RootTransform);
    }

    #region 放入对象

    /// <summary>
    /// 对象放入对象池
    /// </summary>
    /// <param name="obj">对象</param>
    public void PushGameObject(GameObject go)
    {
        if (go.IsNull())
        {
            Debug.LogError($"要回收的的物体：{go.name} 为空");
            return;
        }

        go.GetComponent<IPool>()?.PoolReset();
        go.transform.parent = RootTransform.transform;
        go.SetActive(false);
        if (GoID.ContainsKey(go))
        {
            int id = GoID[go];
            RemoveOutMark(go);
            if (!Pool.ContainsKey(id))
                Pool[id] = new Queue<GameObject>();
            Pool[id].Enqueue(go);
        }
    }

    #endregion

    #region 获取对象

    /// <summary>
    /// 获取GameObject
    /// </summary>
    /// <param name="prefab">要获取的对象</param>
    /// <param name="parent">父物体</param>
    /// <returns></returns>
    public GameObject GetGameObject(GameObject prefab, Transform parent = null)
    {
        int id = prefab.GetInstanceID();
        GameObject go = GetFromPool(id);
        if (go == null)
        {
            go = GameObject.Instantiate<GameObject>(prefab);
            go.name = prefab.name + Time.time;
        }
        go.transform.parent = parent;
        if (parent == null)
            SceneManager.MoveGameObjectToScene(go, SceneManager.GetActiveScene());
        MarkAsOut(go, id);
        go.GetComponent<IPool>()?.PoolInit();
        return go;
    }

    #endregion

    #region 清空对象池

    /// <summary>
    /// 清空对象池
    /// </summary>
    public void ClearGameobjectPool()
    {
        Pool.Clear();
        GoID.Clear();
        GameObject.Destroy(RootTransform.gameObject);
    }

    #endregion

    #region 添加标记、移除标记、根据标记获取对象

    /// <summary>
    /// 根据标记获取对象
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameObject GetFromPool(int id)
    {
        if (Pool.ContainsKey(id) && Pool[id].Count > 0)
        {
            GameObject go = Pool[id].Dequeue();
            go.SetActive(true);
            return go;
        }
        return null;
    }

    /// <summary>
    /// 添加标记
    /// </summary>
    /// <param name="go">要标记的物体</param>
    /// <param name="id">要标记物体的ID</param>
    private void MarkAsOut(GameObject go, int id)
    {
        GoID.Add(go, id);
    }

    /// <summary>
    /// 移除标记
    /// </summary>
    /// <param name="go">要移除标记的物体</param>
    private static void RemoveOutMark(GameObject go)
    {
        if (GoID.ContainsKey(go))
            GoID.Remove(go);
        else
            Debug.Log("删除标记错误,GameObject尚未标记");
    }

    #endregion

}