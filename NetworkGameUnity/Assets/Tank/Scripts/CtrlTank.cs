using System;
using UnityEngine;

public class CtrlTank : BaseTank
{
    void Update()
    {
        // 移动控制
        MoveUpdate();
        // 炮塔控制
        TurretUpdate();
        // 开炮
        FireUpdate();
    }

    private void MoveUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0, x * steer * Time.deltaTime, 0);
        float y = Input.GetAxis("Vertical");
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.position += s;
    }

    private void TurretUpdate()
    {
        float axis = 0;
        if (Input.GetKey(KeyCode.Q))
            axis = -1;
        else if (Input.GetKey(KeyCode.E))
            axis = 1;
        // 旋转角度
        Vector3 le = turret.localEulerAngles;
        le.y += axis * Time.deltaTime * turretSpeed;
        turret.localEulerAngles = le;
    }

    private void FireUpdate()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (Time.time - lastFireTime < fired) return;

        Fire();
    }
}
