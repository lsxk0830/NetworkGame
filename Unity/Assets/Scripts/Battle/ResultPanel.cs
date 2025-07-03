using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : BasePanel
{
    /// <summary>
    /// 胜利提示照片
    /// </summary>
    private Image winImage;
    /// <summary>
    /// 失败提示照片
    /// </summary>
    private Image lostImage;
    /// <summary>
    /// 确定按钮
    /// </summary>
    private Button okBtn;

    public override void OnInit()
    {
        layer = PanelManager.Layer.Tip;
        // 寻找组件
        winImage = transform.Find("WinImage").GetComponent<Image>();
        lostImage = transform.Find("LostImage").GetComponent<Image>();
        okBtn = transform.Find("OkBtn").GetComponent<Button>();
    }

    public override void OnShow(params object[] args)
    {
        // 监听
        gameObject.SetActive(true);
        okBtn.onClick.AddListener(OnOkClick);
        // 显示哪个照片
        if (args.Length == 1)
        {
            bool isWin = (bool)args[0];
            if (isWin)
            {
                winImage.gameObject.SetActive(true);
                lostImage.gameObject.SetActive(false);
            }
            else
            {
                winImage.gameObject.SetActive(false);
                lostImage.gameObject.SetActive(true);
            }
        }
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        okBtn.onClick.RemoveListener(OnOkClick);

    }

    private void OnOkClick()
    {
        SceneManager.LoadSceneAsync("Tank", LoadSceneMode.Single).completed += (op) =>
        {
            // 卸载战斗场景
            SceneManager.UnloadSceneAsync("Game");
            PanelManager.Instance.Open<HomePanelView>();
            OnClose();
        };
    }
}