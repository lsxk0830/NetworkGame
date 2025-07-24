using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Profiling;

public class GamePanel : BasePanel
{
    [LabelText("准星")] public Image FrontSight;
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

    public override void OnInit()
    {
        FrontSight = transform.Find("FrontSight").GetComponent<Image>();
        HPText = transform.Find("HPText").GetComponent<TextMeshProUGUI>();
        HitText = transform.Find("HitText").GetComponent<TextMeshProUGUI>();
        FPSText = transform.Find("FPSText").GetComponent<TextMeshProUGUI>();
        MemoryText = transform.Find("MemoryText").GetComponent<TextMeshProUGUI>();
        map = transform.Find("Map").GetComponent<RectTransform>();
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
