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
    private Room room;
    private bool success;

    public override void OnInit()
    {
        slider = transform.Find("LoadingBar").GetComponent<Slider>();
        prograss = transform.Find("LoadingBar/Fill Area/Fill/Program").GetComponent<TextMeshProUGUI>();
    }

    public override void OnShow(params object[] args)
    {
        success = false;
        gameObject.SetActive(true);
        EventManager.Instance.RegisterEvent(Events.MsgEnterBattle, EnterGame);
        Loading().Forget();
        room = (Room)args[0];
        //Debug.Log($"打开加载界面：{JsonConvert.SerializeObject(room)}");
        string sceneName = SwitchScene(room.mapId);
        SceneManagerAsync.Instance.LoadSceneAsync(sceneName).Forget(); // 加载场景
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        EventManager.Instance.RemoveEvent(Events.MsgEnterBattle, EnterGame);
    }

    /// <summary>
    /// 收到进入游戏协议
    /// </summary>
    private async void EnterGame(MsgBase msgBase)
    {
        EventManager.Instance.RemoveEvent(Events.MsgEnterBattle, EnterGame);
        MsgEnterBattle msg = (MsgEnterBattle)msgBase;
        Debug.Log($"进入游戏:MsgEnterBattle");
        if (msg.result == 0)
        {
            success = true;
            slider.value = 1;
            prograss.text = $"进度:{100}%";
            await UniTask.Delay(200);
            SceneManagerAsync.Instance.Success(success);
            await UniTask.Yield();
            //Debug.Log($"进入游戏:{success}");
            EventManager.Instance.InvokeEvent(Events.MsgEnterBattle, msg);
            OnClose();
        }
        else
            PanelManager.Instance.Open<TipPanel>("进入游戏失败");
    }

    /// <summary>
    /// 假的进度条
    /// </summary>
    private async UniTaskVoid Loading(int i = 0)
    {
        while (!success)
        {
            await UniTask.Delay(80);
            prograss.text = $"进度:{i}%";
            slider.value = i / 100f;
            i++;
            if (i == 100) i = 99;
            if (i == 98)
            {
                MsgLoadingCompletedBattle msg = this.GetObjInstance<MsgLoadingCompletedBattle>();
                msg.roomID = room.RoomID;
                NetManager.Instance.Send(msg);
                this.PushPool(msg);
            }
        }
    }

    /// <summary>
    /// 根据房间的地图ID生成场景
    /// </summary>
    private string SwitchScene(int mapID)
    {
        switch (mapID)
        {
            case 1:
                return "Game";
            default:
                return "Game";
        }
    }
}
