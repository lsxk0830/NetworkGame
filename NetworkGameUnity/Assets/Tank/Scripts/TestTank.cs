using UnityEngine;

public class TestTank : MonoBehaviour
{
    void Start()
    {
        GameObject tankObj = new GameObject("MyTank");
        CtrlTank CtrlTank = tankObj.AddComponent<CtrlTank>();
        CtrlTank.Init("tankPrefab");
        // 相机
        tankObj.AddComponent<CameraFollow>();

        CtrlTank.isDie();
    }
}
