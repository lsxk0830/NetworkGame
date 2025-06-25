using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 0.5f; // 鼠标滑动灵敏度系数
    public Transform turret; // 炮塔目标
    public Transform fire; // 炮塔目标

    private CinemachineFreeLook freeLookCam;
    private float offsetY;
    private float accumulatedX; // 累计水平旋转角度

    public float maxRayLength = 100f; // 最大射线长度
    public Material red; // 红色材质（Enemy）
    public Material green; // 绿色材质（默认）
    private LineRenderer lineRenderer;

    void Start()
    {
        freeLookCam = GetComponent<CinemachineFreeLook>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;// 隐藏鼠标并锁定到屏幕中心
        Cursor.lockState = CursorLockMode.Confined;//鼠标限制在Game视图

        // 禁用Cinemachine默认输入
        freeLookCam.m_XAxis.m_InputAxisName = "";
        freeLookCam.m_YAxis.m_InputAxisName = "";
        freeLookCam.m_XAxis.m_AccelTime = 0;
        freeLookCam.m_YAxis.m_AccelTime = 0;

        // 初始化累计角度
        accumulatedX = freeLookCam.m_XAxis.Value;
        // 计算炮塔与相机的初始Y轴偏移
        offsetY = turret.eulerAngles.y - transform.eulerAngles.y;

        // 初始化LineRenderer组件
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        // 获取鼠标位移（已锁定模式，值在[-1,1]区间）
        float mouseX = Input.GetAxis("Mouse X");
        // 直接累加位移量到旋转角度
        accumulatedX += mouseX * rotationSpeed;
        // 更新Cinemachine轴值（立即生效）
        freeLookCam.m_XAxis.Value = accumulatedX;
        // 同步炮塔Y轴旋转（仅水平方向）
        Vector3 turretEuler = turret.eulerAngles;
        turret.rotation = Quaternion.Euler(turretEuler.x, accumulatedX + offsetY, turretEuler.z);
        Draw();
    }

    void OnDestroy()
    {
        // 退出时恢复鼠标状态
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Draw()
    {
        // 1. 计算射线方向（炮管正前方）
        Vector3 rayStart = fire.position;
        Vector3 rayDirection = fire.forward;

        // 2. 发射射线检测碰撞
        RaycastHit hit;
        bool isHit = Physics.Raycast(rayStart, rayDirection, out hit, maxRayLength);
        Debug.DrawRay(rayStart, rayDirection * 10, Color.green);

        // 3. 动态设置线段终点
        Vector3 lineEnd = isHit ? hit.point : rayStart + rayDirection * maxRayLength;
        lineRenderer.SetPosition(0, rayStart); // 起点：炮管末端
        lineRenderer.SetPosition(1, lineEnd); // 终点：碰撞点或最大长度点

        // 4. 根据碰撞标签切换线段颜色
        if (isHit && hit.collider.CompareTag("Enemy"))
        {
            lineRenderer.material = red; // 击中敌人：红色
        }
        else
        {
            lineRenderer.material = green; // 其他情况：绿色
        }
    }
}