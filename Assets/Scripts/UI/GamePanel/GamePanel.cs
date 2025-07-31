using UnityEngine;
using TMPro;
using UnityEngine.Profiling;

public class GamePanel : BasePanel
{
    [LabelText("准星")] public Transform FrontSight;
    [LabelText("血量")][SerializeField] private TextMeshProUGUI HPText;
    [LabelText("击中伤害")][SerializeField] private TextMeshProUGUI HitText;
    [LabelText("FPS")][SerializeField] private TextMeshProUGUI FPSText;
    [LabelText("内存占用")][SerializeField] private TextMeshProUGUI MemoryText;
    [LabelText("总共造成的伤害")][SerializeField] private int hitCount;
    [LabelText("总共造成的伤害")][SerializeField] private RectTransform map;
    public RectTransform Map => map;
    public float updateInterval = 0.5f; // 更新间隔时间
    private float lastUpdateTime; // 上次更新时间
    private int frameCount; // 帧计数
    private float accumulatedFps; // 累计FPS
    [LabelText("开火时不看Player")][SerializeField] public LayerMask IsFrontSight;
    [LabelText("正常相机看到的层")][SerializeField] public LayerMask NormalFrontSight;

    private Camera mainCamera;

    public override void OnInit()
    {
        IsFrontSight = LayerMask.GetMask("Default", "Enemy", "Friend", "CanDestroy", "Bullet");
        NormalFrontSight = LayerMask.GetMask("Default", "Enemy", "Friend", "CanDestroy", "Player", "Bullet");
        FrontSight = transform.Find("FrontSight");
        HPText = transform.Find("HPText").GetComponent<TextMeshProUGUI>();
        HitText = transform.Find("HitText").GetComponent<TextMeshProUGUI>();
        FPSText = transform.Find("FPSText").GetComponent<TextMeshProUGUI>();
        MemoryText = transform.Find("MemoryText").GetComponent<TextMeshProUGUI>();
        map = transform.Find("Map").GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public override void OnShow(params object[] args)
    {
        gameObject.SetActive(true);
        hitCount = 0;
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    private void OnUpdate()
    {
        // 定时更新UI
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            // 1. FPS计算
            float currentFps = 1f / Time.unscaledDeltaTime;
            accumulatedFps += currentFps;
            frameCount++;
            float avgFps = accumulatedFps / frameCount;

            FPSText.text = $"FPS: {avgFps:0.}";
            FPSText.color = GetValueColor(avgFps, 60, 30);

            // 2. 内存占用
            long totalMemory = Profiler.GetTotalAllocatedMemoryLong() / 1048576; // MB
            MemoryText.text = $"Memory: {totalMemory}MB";
            MemoryText.color = GetValueColor(totalMemory, 500, 1000);

            lastUpdateTime = Time.time;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (FrontSight.gameObject.activeSelf)
            {
                mainCamera.cullingMask = NormalFrontSight;
                FrontSight.gameObject.SetActive(false);
            }
            else
            {
                mainCamera.cullingMask = IsFrontSight;
                FrontSight.gameObject.SetActive(true);
            }
        }
    }

    // 根据数值返回颜色（绿-黄-红）
    private Color GetValueColor(float value, float good, float warn)
    {
        if (value < good) return Color.green;
        if (value < warn) return Color.yellow;
        return Color.red;
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        GloablMono.Instance.OnUpdate -= OnUpdate;
    }

    /// <summary>
    /// 造成伤害
    /// </summary>
    internal void UpdateHP(int HP)
    {
        HPText.text = $"HP：{HP}";
    }

    /// <summary>
    /// 造成伤害
    /// </summary>
    internal void UpdateHit(int damage)
    {
        hitCount += damage;
        HitText.text = $"伤害：{hitCount}";
    }
}
