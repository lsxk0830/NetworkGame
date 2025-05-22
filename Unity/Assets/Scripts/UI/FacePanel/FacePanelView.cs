using UnityEngine.UI;

public class FacePanelView : BasePanel
{
    public FacePanelController control;
    private Image FaceImage;
    public Button downloadBtn;
    public Button changeBtn;

    public override void OnInit()
    {
        FaceImage = transform.Find("FaceImage").GetComponent<Image>();
        downloadBtn = transform.Find("DownloadBtn").GetComponent<Button>();
        changeBtn = transform.Find("ChangeBtn").GetComponent<Button>();

        control = new FacePanelController();
        HomePanelView view = (HomePanelView)PanelManager.Instance.panels[typeof(HomePanelView).FullName];
        FaceImage.sprite = view.faceBtn.GetComponent<Image>().sprite;
    }

    public override void OnShow(params object[] args)
    {
        downloadBtn.onClick.AddListener(OnDownloadClick);
        changeBtn.onClick.AddListener(OnChangeClick);
    }

    public override void OnClose()
    {
        downloadBtn.onClick.RemoveListener(OnDownloadClick);
        changeBtn.onClick.RemoveListener(OnChangeClick);
    }

    /// <summary>
    /// 下载图片
    /// </summary>
    private void OnDownloadClick()
    {
        control.DownloadImage();
    }

    /// <summary>
    /// 切换图片
    /// </summary>
    private void OnChangeClick()
    {
        control.ChangeImage(FaceImage).Forget();
    }

}
