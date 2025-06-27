using System;
using UnityEngine;

/// <summary>
/// 障碍物的移动旋转缩放监听
/// </summary>
public class ObstacleListener : MonoBehaviour
{
    [SerializeField] private float movementThreshold = 0.1f; // 移动阈值（单位：米）
    private Vector3 lastPosition;
    private MsgObstacleOne msgOne;

    internal void Init()
    {
        lastPosition = transform.position;
        msgOne = new MsgObstacleOne();
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        float sqrDistance = (currentPosition - lastPosition).sqrMagnitude;

        // 如果移动距离的平方 > 阈值的平方，才认为发生了有效移动
        if (sqrDistance > movementThreshold * movementThreshold)
        {
            OnMoved();
            lastPosition = currentPosition;
        }
    }

    void OnMoved()
    {
        Debug.Log($"物体 {gameObject.name} 移动了（距离超过阈值）");
        // 发送消息（例如调用 MessageCenter）
        msgOne.PosRotScale = new ObstaclePosRotScale()
        {
            ObstacleID = gameObject.name,
            PosX = transform.position.x,
            PosY = transform.position.y,
            PosZ = transform.position.z,
            RotX = transform.rotation.eulerAngles.x,
            RotY = transform.rotation.eulerAngles.y,
            RotZ = transform.rotation.eulerAngles.z,
            ScaleX = transform.localScale.x,
            ScaleY = transform.localScale.y,
            ScaleZ = transform.localScale.z
        };
        NetManager.Send(msgOne);
    }

    internal void UpdateInfo(ObstaclePosRotScale item)
    {
        transform.position = new Vector3(item.PosX, item.PosY, item.PosZ);
        transform.rotation = Quaternion.Euler(new Vector3(item.RotX, item.RotY, item.RotZ));
        transform.localScale = new Vector3(item.ScaleX, item.ScaleY, item.ScaleZ);
        lastPosition = transform.position;
    }

    void OnDestroy()
    {
        msgOne = null;
        NetManager.Send(msgOne);
    }
}