using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardsPanel : BasePanel
{
    [SerializeField][LabelText("签到动画面板")] private GameObject ResultPanel;
    [SerializeField][LabelText("签到按钮")] private Button ClaimButton;
    [SerializeField][LabelText("关闭按钮")] private Button CloseButton;
    [SerializeField][LabelText("天数")] private Transform Datalist;
    [SerializeField][LabelText("连续签到天数")] private int continuousSignInDays;
    [SerializeField][LabelText("金币")] private GameObject CoinIcon;
    [SerializeField][LabelText("钻石")] private GameObject DiamondIcon;
    [SerializeField][LabelText("金币钻石")] private GameObject CoinDiamondIcon;
    [SerializeField][LabelText("计数文本")] private TextMeshProUGUI CountText;

    public override void OnInit()
    {
        layer = PanelManager.Layer.Panel;
        ClaimButton = transform.Find("Container/ClaimButton").GetComponent<Button>();
        ResultPanel = transform.Find("ResultPanel").gameObject;
        Datalist = transform.Find("Container/Datalist");
        CloseButton = transform.Find("Container/CloseButton").GetComponent<Button>();
        CoinIcon = transform.Find("ResultPanel/Panel/1").gameObject;
        DiamondIcon = transform.Find("ResultPanel/Panel/2").gameObject;
        CoinDiamondIcon = transform.Find("ResultPanel/Panel/3").gameObject;
        CountText = transform.Find("ResultPanel/CountText").GetComponent<TextMeshProUGUI>();
    }

    public override void OnShow(params object[] args)
    {
        CoinIcon.SetActive(false);
        DiamondIcon.SetActive(false);
        CoinDiamondIcon.SetActive(false);
        gameObject.SetActive(true);
        CloseButton.onClick.AddListener(OnCloseButtonClick);
        ClaimButton.onClick.AddListener(OnClaimButtonClick);
        ClaimButton.interactable = false;
        EventManager.Instance.RegisterEvent(Events.MsgSignIn, OnMsgSignIn);
        NetManager.Instance.Send(new MsgSignIn() { Query = true }); // 请求签到信息
    }

    private void OnMsgSignIn(MsgBase msgBase)
    {
        EventManager.Instance.RemoveEvent(Events.MsgSignIn, OnMsgSignIn);
        Debug.Log("收到签到消息");
        MsgSignIn msg = (MsgSignIn)msgBase;
        continuousSignInDays = msg.ContinuousSignIn;
        if (msg.Query)
        {
            ClaimButton.interactable = true;
        }
        for (int i = 0; i < continuousSignInDays; i++)
        {
            Datalist.GetChild(i).Find("TickMark").gameObject.SetActive(true);
        }
        for (int i = continuousSignInDays; i < Datalist.childCount; i++)
        {
            Datalist.GetChild(i).Find("TickMark").gameObject.SetActive(false);
        }
    }

    public override void OnClose()
    {
        this.Log("关闭每日签到面板");
        gameObject.SetActive(false);
        ClaimButton.onClick.RemoveListener(OnClaimButtonClick);
        CloseButton.onClick.RemoveListener(OnCloseButtonClick);
        EventManager.Instance.InvokeEvent(Events.GoHome);
        EventManager.Instance.RemoveEvent(Events.MsgSignIn, OnMsgSignIn);
    }

    private void OnClaimButtonClick()
    {
        // UI 处理
        NetManager.Instance.Send(new MsgSignIn() { Query = false });
        Datalist.GetChild(continuousSignInDays).Find("TickMark").gameObject.SetActive(true);
        ClaimButton.interactable = false;
        continuousSignInDays++;
        StartCoroutine(_ShowResult());
    }

    private void OnCloseButtonClick()
    {
        OnClose();
    }

    private IEnumerator _ShowResult()
    {
        CoinIcon.SetActive(false);
        DiamondIcon.SetActive(false);
        CoinDiamondIcon.SetActive(false);
        switch (continuousSignInDays)
        {
            case 1:
                CountText.text = "+100";
                UserManager.Instance.GetUser(GameMain.ID).Coin += 100;
                CoinIcon.SetActive(true);
                break;
            case 2:
                CountText.text = "+100";
                UserManager.Instance.GetUser(GameMain.ID).Diamond += 100;
                DiamondIcon.SetActive(true);
                break;
            case 3:
                CountText.text = "+200";
                UserManager.Instance.GetUser(GameMain.ID).Coin += 200;
                CoinIcon.SetActive(true);
                break;
            case 4:
                CountText.text = "+200";
                UserManager.Instance.GetUser(GameMain.ID).Diamond += 200;
                DiamondIcon.SetActive(true);
                break;
            case 5:
                CountText.text = "+300";
                UserManager.Instance.GetUser(GameMain.ID).Coin += 300;
                DiamondIcon.SetActive(true);
                break;
            case 6:
                CountText.text = "+300";
                UserManager.Instance.GetUser(GameMain.ID).Diamond += 300;
                DiamondIcon.SetActive(true);
                break;
            case 7:
                CountText.text = "+500";
                UserManager.Instance.GetUser(GameMain.ID).Coin += 500;
                UserManager.Instance.GetUser(GameMain.ID).Diamond += 500;
                CoinDiamondIcon.SetActive(true);
                break;
            default:
                break;
        }

        EventManager.Instance.InvokeEvent(Events.UpdateCoinDiamond);
        ResultPanel.SetActive(true);
        ResultPanel.GetComponent<Animator>().Play("SingIn");

        yield return new WaitForSeconds(3.3f);
        ResultPanel.SetActive(false);
    }
}
