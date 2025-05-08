using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    private TMP_Text text; // 提示文本
    private Button okBtn; // 好的按钮

    private Action clickCallback;

    public override void OnInit()
    {
        panelName = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }

    public override void OnShow(params object[] args)
    {
        text = go.transform.Find("Text").GetComponent<TMP_Text>();
        okBtn = go.transform.Find("OKBtn").GetComponent<Button>();
        if (args.Length == 1)
        {
            text.text = (string)args[0];
            clickCallback = Close;
            okBtn.onClick.AddListener(Click);
        }
        else if (args.Length == 2)
        {
            text.text = (string)args[0];
            clickCallback = (Action)args[1];
        }
        okBtn.onClick.AddListener(Click);
    }

    private void Click()
    {
        clickCallback?.Invoke();
    }
}