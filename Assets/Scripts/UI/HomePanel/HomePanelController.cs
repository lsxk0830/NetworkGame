using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class HomePanelController
{
    private readonly HomePanelView view;

    public HomePanelController(HomePanelView view)
    {
        this.view = view;
    }

    public async UniTaskVoid UpdateUI()
    {
        User user = UserManager.Instance.GetUser(GameMain.ID);
        if (user.AvatarPath != "defaultAvatar")
        {
            string directory = Path.Combine(Application.persistentDataPath, "Avatar");
            string path = Path.Combine(directory, user.AvatarPath + ".png");
            Debug.Log($"加载图片的路径:{path}");
            if (!Directory.Exists(directory) || !File.Exists(path))
            {
                Debug.Log($"加载网络图片");
                HTTPManager.Instance.GetSetAvatarByDB(user.AvatarPath, view.GetAvatarImage()).Forget();
            }
            else
            {
                Debug.Log($"加载本地图片");
                Texture2D texture = await HTTPManager.Instance.GetImage(path);
                if (texture == null) return;
                view.GetAvatarImage().sprite = Sprite.Create
                (texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
        }
        //NetManager.Send(new MsgGetRoomList());
    }

    public void UpdateUserInfo()
    {
        view.UpdateUserInfo(UserManager.Instance.GetUser(GameMain.ID));
    }

    #region 用户操作处理

    /// <summary>
    /// 设置头像面板
    /// </summary>
    public void HandleFace()
    {
        PanelManager.Instance.Open<FacePanelView>();
    }

    /// <summary>
    /// 开始游戏（打开房间大厅）
    /// </summary>
    public void HandlePlay()
    {
        PanelManager.Instance.Open<RoomHallPanelView>();
    }

    #endregion
}