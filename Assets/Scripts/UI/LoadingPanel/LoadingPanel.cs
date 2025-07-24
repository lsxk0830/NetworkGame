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

    public async override void OnShow(params object[] args)
    {
        success = false;
        gameObject.SetActive(true);
        EventManager.Instance.RegisterEvent(Events.MsgEnterBattle, EnterGame);
        room = (Room)args[0];
        //Debug.Log($"打开加载界面：{JsonConvert.SerializeObject(room)}");
        string sceneName = SwitchScene(room.mapId);
        await UniTask.Yield(); // 等待一帧，确保UI更新
        Loading().Forget();
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
    private void EnterGame(MsgBase msgBase)
    {
        EventManager.Instance.RemoveEvent(Events.MsgEnterBattle, EnterGame);
        MsgEnterBattle msg = (MsgEnterBattle)msgBase;
        if (msg.result == 0)
        {
            success = true;
            SceneManagerAsync.Instance.Success(option =>
            {
                slider.value = 1;
                prograss.text = $"进度:{100}%";
                OnClose();
            });
        }
        else
            PanelManager.Instance.Open<TipPanel>("进入游戏失败");
    }

    /// <summary>
    /// 进度条
    /// </summary>
    private async UniTaskVoid Loading()
    {
        int i = 1;
        // 进度条平滑化处理
        while (!success)
        {
            Debug.Log($"加载进度: {i}%");
            prograss.text = $"进度:{i}%";
            slider.value = i / 100f;
            await UniTask.Delay(80);
            i++;
            if (i >= 100)
            {
                Debug.Log($"加载进度: {i}%");
                MsgLoadingCompletedBattle msg = this.GetObjInstance<MsgLoadingCompletedBattle>();
                msg.roomID = room.RoomID;
                NetManager.Instance.Send(msg);
                this.PushPool(msg);
                break;
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
