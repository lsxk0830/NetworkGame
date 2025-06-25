using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    public float moveSpeed = 8f;      // 移动速度
    public float rotateSpeed = 120f;  // 旋转速度（度/秒）
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 键盘输入获取
        float moveInput = Input.GetAxis("Vertical");    // W/S 控制前进后退
        float rotateInput = Input.GetAxis("Horizontal"); // A/D 控制左右旋转

        // 物理移动（基于坦克自身坐标系）
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + moveDirection);

        // 物理旋转（绕Y轴）
        float rotation = rotateInput * rotateSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, rotation, 0);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
    }
}