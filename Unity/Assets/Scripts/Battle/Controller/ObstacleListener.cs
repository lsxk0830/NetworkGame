using UnityEngine;

/// <summary>
/// 障碍物的移动旋转缩放监听
/// </summary>
public class ObstacleListener : MonoBehaviour
{
    [SerializeField] private float movementThreshold = 0.1f; // 移动阈值（单位：米）
    private Vector3 lastPosition;

    internal void Init()
    {
        lastPosition = transform.position;
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
        //Debug.Log($"物体 {gameObject.name} 移动了（距离超过阈值）");
        // 发送消息（例如调用 MessageCenter）
        MsgObstacleOne msg = this.GetObjInstance<MsgObstacleOne>();
        msg.PosRotScale = this.GetObjInstance<ObstaclePosRotScale>();
        msg.PosRotScale.ObstacleID = gameObject.name;
        msg.PosRotScale.PosX = transform.position.x;
        msg.PosRotScale.PosY = transform.position.y;
        msg.PosRotScale.PosZ = transform.position.z;
        msg.PosRotScale.RotX = transform.rotation.eulerAngles.x;
        msg.PosRotScale.RotY = transform.rotation.eulerAngles.y;
        msg.PosRotScale.RotZ = transform.rotation.eulerAngles.z;
        msg.PosRotScale.ScaleX = transform.localScale.x;
        msg.PosRotScale.ScaleY = transform.localScale.y;
        msg.PosRotScale.ScaleZ = transform.localScale.z;
        msg.IsDestory = false; // 不是销毁
        NetManager.Instance.Send(msg);
        this.PushPool(msg);
        this.PushPool(msg.PosRotScale);
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
        MsgObstacleOne msg = this.GetObjInstance<MsgObstacleOne>();
        msg.PosRotScale = this.GetObjInstance<ObstaclePosRotScale>();
        msg.IsDestory = true; //销毁
        NetManager.Instance.Send(msg);
        this.PushPool(msg);
        this.PushPool(msg.PosRotScale);
    }
}