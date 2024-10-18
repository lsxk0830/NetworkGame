using TMPro;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    private TMP_Text text; // 提示文本
    private Button okBtn; // 好的按钮
    public override void OnInit()
    {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }

    public override void OnShow(params object[] args)
    {
        text = skin.transform.Find("Text").GetComponent<TMP_Text>();
        okBtn = skin.transform.Find("OKBtn").GetComponent<Button>();
        okBtn.onClick.AddListener(OnOkClick);
        if (args.Length == 1)
            text.text = (string)args[0];
    }

    private void OnOkClick()
    {
        Close();
    }

    public override void OnClose()
    {

    }
}