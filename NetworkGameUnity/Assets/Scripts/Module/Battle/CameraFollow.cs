using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 distance = new Vector3(0, 8, -18); // 距离矢量
    public Camera mCamera; // 相机
    public Vector3 offset = new Vector3(0, 5f, 0); // 偏移值
    public float speed = 6; // 相机移动速度

    void Start()
    {
        mCamera = Camera.main;
        Vector3 initPos = transform.position - 30 * transform.forward + Vector3.up * 10;
        mCamera.transform.position = initPos;

        GloablMono.Instance.OnLateUpdate += OnLateUpdate;
    }

    private void OnLateUpdate(float f)
    {
        Vector3 pos = transform.position; // 坦克位置
        Vector3 forward = transform.forward; // 坦克方向
        Vector3 targetPos = pos + forward * distance.z; // 相机目标位置
        targetPos.y += distance.y;
        Vector3 cameraPos = mCamera.transform.position; // 相机位置
        cameraPos = Vector3.MoveTowards(cameraPos, targetPos, Time.deltaTime * speed);
        mCamera.transform.position = cameraPos;
        mCamera.transform.LookAt(pos + offset);
    }

    private void OnDestroy()
    {
        GloablMono.Instance.OnLateUpdate -= OnLateUpdate;
    }
}