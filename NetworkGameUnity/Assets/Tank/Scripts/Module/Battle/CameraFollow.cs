using UnityEngine;

namespace Tank
{
    public class CameraFollow : MonoBehaviour
    {
        public Vector3 distance = new Vector3(0, 8, -18); // 距离矢量
        public Camera mCamera; // 相机
        public Vector3 offset = new Vector3(0, 5f, 0); // 偏移值
        public float speed = 3; // 相机移动速度

        void Start()
        {
            mCamera = Camera.main;
            Vector3 pos = transform.position;
            Vector3 forward = transform.forward;
            Vector3 initPos = pos - 30 * forward + Vector3.up * 10;
            mCamera.transform.position = initPos;
        }

        void LateUpdate()
        {
            Vector3 pos = transform.position; // 坦克位置
            Vector3 forward = transform.forward; // 坦克方向
            Vector3 targetPos = pos; // 相机目标位置
            targetPos = pos + forward * distance.z;
            targetPos.y += distance.y;
            Vector3 cameraPos = mCamera.transform.position; // 相机位置
            cameraPos = Vector3.MoveTowards(cameraPos, targetPos, Time.deltaTime * speed);
            mCamera.transform.position = cameraPos;
            mCamera.transform.LookAt(pos + offset);
        }
    }
}