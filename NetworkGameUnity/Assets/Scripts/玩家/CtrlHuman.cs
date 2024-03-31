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
                NetManager.Send("Enter|127.1.1.1,100,200,300,45"); // "要做什么事情|谁在移动，目的地是什么"
            }
        }
    }
}