using UnityEngine.UI;

namespace Tank
{
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
            skinPath = "ResultPanel";
            layer = PanelManager.Layer.Tip;
        }

        public override void OnShow(params object[] args)
        {
            // 寻找组件
            winImage = skin.transform.Find("WinImage").GetComponent<Image>();
            lostImage = skin.transform.Find("LostImage").GetComponent<Image>();
            okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
            // 监听
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

        public override void OnClose() { }

        private void OnOkClick()
        {
            PanelManager.Open<RoomPanel>();
            Close();
        }
    }
}