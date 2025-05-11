using UnityEngine;
using UnityEngine.UI;

public class FacePanelView : BasePanel
{
    public FacePanelController control;
    public Image face;
    public Button downloadBtn;
    public Button changeBtn;

    public override void OnInit()
    {
        face = transform.Find("FaceImage").GetComponent<Image>();
        downloadBtn = transform.Find("DownloadBtn").GetComponent<Button>();
        changeBtn = transform.Find("ChangeBtn").GetComponent<Button>();

        downloadBtn.onClick.AddListener(OnDownloadClick);
        changeBtn.onClick.AddListener(OnChangeClick);

        control = new FacePanelController();

        control.Show();
    }

    private void OnDownloadClick()
    {
        control.DownloadImage();
    }
    private void OnChangeClick()
    {
        control.ChangeImage(face.GetComponent<Image>()).Forget();
    }
    public override void OnClose()
    {
        base.OnClose();
        control.Close();
    }
}
