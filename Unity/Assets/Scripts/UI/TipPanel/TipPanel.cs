using System;
using TMPro;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    private TMP_Text text; // 提示文本
    private Button okBtn; // 好的按钮

    private Action clickCallback;

    public override void OnInit()
    {
        layer = PanelManager.Layer.Tip;

        text = transform.Find("Text").GetComponent<TMP_Text>();
        okBtn = transform.Find("OKBtn").GetComponent<Button>();
    }

    public override void OnShow(params object[] args)
    {
        if (args.Length == 1)
        {
            text.text = (string)args[0];
            clickCallback = OnClose;
        }
        else if (args.Length == 2)
        {
            text.text = (string)args[0];
            clickCallback = (Action)args[1];
        }
        okBtn.onClick.AddListener(Click);
    }

    public override void OnClose()
    {
        okBtn.onClick.RemoveListener(Click);
        gameObject.SetActive(false);
    }

    private void Click()
    {
        clickCallback?.Invoke();
        gameObject.SetActive(false);
        okBtn.onClick.RemoveListener(Click);
    }
}