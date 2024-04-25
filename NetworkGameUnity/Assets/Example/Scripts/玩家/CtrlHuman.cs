using UnityEngine;

namespace Example
{
    public class CtrlHuman : BaseHuman
    {
        private new void Start()
        {
            base.Start();
        }

        private new void Update()
        {
            base.Update();
            if (Input.GetMouseButtonDown(0)) // 移动
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

            if (Input.GetMouseButtonDown(1)) // 攻击
            {
                if (isAttacking) return;
                if (isMoving) return;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hit);
                transform.LookAt(hit.point);
                Attack();

                // 发送协议
                string sendStr = "Attack|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += transform.eulerAngles.y + ",";
                NetManager.Send(sendStr);

                // 攻击判定
                Vector3 lineEnd = transform.position + 0.5f * Vector3.up; // 腰部位置
                Vector3 lineStart = lineEnd + 20 * transform.forward;

                if (Physics.Linecast(lineStart, lineEnd, out hit))
                {
                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj == gameObject)
                        return;
                    SyncHuman h = hitObj.GetComponent<SyncHuman>();
                    if (h == null)
                        return;
                    sendStr = "Hit|";
                    sendStr += NetManager.GetDesc() + ",";
                    sendStr += h.desc + ",";
                    NetManager.Send(sendStr);
                }
            }
        }
    }
}