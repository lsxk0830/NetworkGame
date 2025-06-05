using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 加载面板
/// </summary>
public class LoadingPanel : BasePanel
{
    private Slider slider;
    private TextMeshProUGUI prograss;
    private bool success;

    public override void OnInit()
    {
        slider = transform.Find("LoadingBar").GetComponent<Slider>();
        prograss = transform.Find("LoadingBar/Fill Area/Fill/Program").GetComponent<TextMeshProUGUI>();
    }

    public override void OnShow(params object[] args)
    {
        gameObject.SetActive(true);
        Delay().Forget();
        EventManager.Instance.RegisterEvent(Events.MsgEnterBattle, EnterGame);
        // ToDo ： 加载资源
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        EventManager.Instance.RemoveEvent(Events.MsgEnterBattle, EnterGame);
    }

    private void EnterGame(MsgBase msgBase)
    {
        MsgEnterBattle msg = (MsgEnterBattle)msgBase;
        Debug.Log($"进入游戏:{msg.mapId}");
        // ToDo 进入游戏
    }

    private async UniTask Delay(int i = 0)
    {
        while (!success)
        {
            await UniTask.Delay(200);
            prograss.text = $"进度:{1}%";
            slider.value = i / 100;
            i++;
            if (i == 100) i = 99;
        }
    }
}
