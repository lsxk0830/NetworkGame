using System;
using UnityEngine;
using UnityEngine.UI;

public class FacePanelView : BasePanel
{
    private FacePanelController control;
    private Image FaceImage;
    private Image HomePanelViewAvatar; // HomePanelView的头像
    [SerializeField][LabelText("下载头像")] private Button downloadBtn;
    [SerializeField][LabelText("更换头像")] private Button changeBtn;
    [SerializeField][LabelText("保存头像")] private Button saveBtn;

    public override void OnInit()
    {
        FaceImage = transform.Find("FaceImage").GetComponent<Image>();
        downloadBtn = transform.Find("DownloadBtn").GetComponent<Button>();
        changeBtn = transform.Find("ChangeBtn").GetComponent<Button>();
        saveBtn = transform.Find("SaveBtn").GetComponent<Button>();

        control = new FacePanelController();
        HomePanelView view = (HomePanelView)PanelManager.Instance.panels[typeof(HomePanelView).FullName];
        HomePanelViewAvatar = view.faceBtn.GetComponent<Image>();
        FaceImage.sprite = HomePanelViewAvatar.sprite;
    }

    public override void OnShow(params object[] args)
    {
        downloadBtn.onClick.AddListener(OnDownloadClick);
        changeBtn.onClick.AddListener(OnChangeClick);
        saveBtn.onClick.AddListener(OnSaveClick);
    }

    public override void OnClose()
    {
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

    #endregion UI点击
}
