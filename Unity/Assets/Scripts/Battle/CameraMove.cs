using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //摄像机 看向的目标
    public Transform targetPlayer;
    public float H = 20;

    private Vector3 pos;

    void Awake()
    {
        GloablMono.Instance.OnLateUpdate += OnLateUpdate;
    }

    void OnLateUpdate()
    {
        if (targetPlayer == null) return;
        //x和z和玩家一样
        pos.x = targetPlayer.position.x;
        pos.z = targetPlayer.position.z;
        //通过外部调整摄像机 高度
        pos.y = H;
        this.transform.position = pos;
    }

    void OnDestroy()
    {
        GloablMono.Instance.OnLateUpdate -= OnLateUpdate;
    }
}
