using UnityEngine;

public class CtrlHuman : BaseHuman
{
    private new void Start()
    {
        base.Start();
    }

    private new void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit);
            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);

                // 发送Move协议
                string sendStr = "Move|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += hit.point.x + ",";
                sendStr += hit.point.y + ",";
                sendStr += hit.point.z + ",";
                NetManager.Send(sendStr); // "要做什么事情|谁在移动，目的地是什么"
            }
        }
    }
}