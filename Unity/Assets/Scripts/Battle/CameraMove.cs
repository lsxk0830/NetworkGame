using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour
{
    //摄像机 看向的目标
    private bool CameraIsMove = true;// 相机更新移动
    public Transform targetPlayer;
    [SerializeField] private float H = 10;

    private Vector3 pos;
    private GamePanel gamePanel;

    void Awake()
    {
        GloablMono.Instance.OnUpdate += OnUpdate;
        GloablMono.Instance.OnLateUpdate += OnLateUpdate;
        gamePanel = PanelManager.Instance.GetPanel<GamePanel>();

        // 动态创建 RT
        RenderTexture rt = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);

        // UI 面板与相机绑定同一 RT
        GetComponent<Camera>().targetTexture = rt;
        RawImage rawImage = gamePanel.GetComponentInChildren<RawImage>();
        rawImage.texture = rt;
    }

    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (H == 10)
            {
                CameraIsMove = false;
                H = 45;
                transform.position = new Vector3(25, 45, 25);
                gamePanel.Map.sizeDelta = new Vector2(800, 800);
            }
            else
            {
                CameraIsMove = true;
                H = 10;
                gamePanel.Map.sizeDelta = new Vector2(300, 300);
            }
        }
    }

    void OnLateUpdate()
    {
        if (!CameraIsMove) return;
        if (targetPlayer == null) return;
        //x和z和玩家一样
        pos.x = targetPlayer.position.x;
        pos.z = targetPlayer.position.z;
        //通过外部调整摄像机 高度
        pos.y = H;
        this.transform.position = pos;
    }

    void OnDestroy()
    {
        GloablMono.Instance.OnUpdate -= OnUpdate;
        GloablMono.Instance.OnLateUpdate -= OnLateUpdate;
    }
}
