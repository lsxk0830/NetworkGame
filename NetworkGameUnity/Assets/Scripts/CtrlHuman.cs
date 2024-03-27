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
            }
        }
    }
}