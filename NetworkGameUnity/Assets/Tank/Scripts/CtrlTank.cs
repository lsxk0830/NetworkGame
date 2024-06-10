using UnityEngine;

public class CtrlTank : BaseTank
{
    public float steer = 20; // 旋转角度
    public float speed = 5; // 移动速度

    new void Update()
    {
        base.Update();

        MoveUpdate();
    }

    private void MoveUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0, x * steer * Time.deltaTime, 0);
        float y = Input.GetAxis("Vertical");
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.position += s;
    }
}
