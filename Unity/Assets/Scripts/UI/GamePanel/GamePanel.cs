using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GamePanel : BasePanel
{
    [LabelText("准星")] public Image FrontSight;
    [LabelText("血量")][SerializeField] private TextMeshProUGUI HPText;
    [LabelText("击中伤害")][SerializeField] private TextMeshProUGUI HitText;
    [LabelText("总共造成的伤害")][SerializeField] private int hitCount;
    [LabelText("总共造成的伤害")][SerializeField] private RectTransform map;
    public RectTransform Map => map;

    public override void OnInit()
    {
        FrontSight = transform.Find("FrontSight").GetComponent<Image>();
        HPText = transform.Find("HPText").GetComponent<TextMeshProUGUI>();
        HitText = transform.Find("HitText").GetComponent<TextMeshProUGUI>();
        map = transform.Find("Map").GetComponent<RectTransform>();
    }

    public override void OnShow(params object[] args)
    {
        gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
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
