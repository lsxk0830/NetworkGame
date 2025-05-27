using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FacePanelView : BasePanel, IPointerClickHandler
{
    private FacePanelController control;
    private Image FaceImage;
    private Image HomePanelViewAvatar; // HomePanelView的头像
    [SerializeField][LabelText("下载头像")] private Button downloadBtn;
    [SerializeField][LabelText("更换头像")] private Button changeBtn;
    [SerializeField][LabelText("保存头像")] private Button saveBtn;
    [SerializeField][LabelText("忽略点击区域")] private RectTransform[] ignoreAreas;
    public override void OnInit()
    {
        FaceImage = transform.Find("FaceImage").GetComponent<Image>();
        downloadBtn = transform.Find("DownloadBtn").GetComponent<Button>();
        changeBtn = transform.Find("ChangeBtn").GetComponent<Button>();
        saveBtn = transform.Find("SaveBtn").GetComponent<Button>();
        ignoreAreas = new RectTransform[4]
        {
            FaceImage.GetComponent<RectTransform>() ,
            downloadBtn.GetComponent<RectTransform>(),
            changeBtn.GetComponent<RectTransform>(),
            saveBtn.GetComponent<RectTransform>(),
        };
        control = new FacePanelController();
        HomePanelView view = (HomePanelView)PanelManager.Instance.panels[typeof(HomePanelView).FullName];
        HomePanelViewAvatar = view.faceBtn.GetComponent<Image>();
        FaceImage.sprite = HomePanelViewAvatar.sprite;
    }

    public override void OnShow(params object[] args)
    {
        gameObject.SetActive(true);
        downloadBtn.onClick.AddListener(OnDownloadClick);
        changeBtn.onClick.AddListener(OnChangeClick);
        saveBtn.onClick.AddListener(OnSaveClick);
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        downloadBtn.onClick.RemoveListener(OnDownloadClick);
        changeBtn.onClick.RemoveListener(OnChangeClick);
        saveBtn.onClick.RemoveListener(OnSaveClick);
    }

    #region UI点击

    /// <summary>
    /// 下载图片
    /// </summary>
    private void OnDownloadClick()
    {
        control.DownloadImage(FaceImage.sprite);
    }

    /// <summary>
    /// 切换图片
    /// </summary>
    private void OnChangeClick()
    {
        control.ChangeImage(FaceImage).Forget();
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    private void OnSaveClick()
    {
        control.Save(FaceImage, HomePanelViewAvatar);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (var area in ignoreAreas)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(area, eventData.position, eventData.pressEventCamera))
            {
                return; // 如果点击在忽略区域，直接返回
            }
        }
        OnClose();
    }

    #endregion UI点击
}
