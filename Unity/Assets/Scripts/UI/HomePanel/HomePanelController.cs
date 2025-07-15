using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class HomePanelController
{
    private readonly HomePanelModel model;
    private readonly HomePanelView view;

    public HomePanelController(HomePanelView view)
    {
        model = new HomePanelModel();
        this.view = view;
        model.ID = GameMain.ID;
    }

    public async UniTaskVoid UpdateUI()
    {
        User user = model.GetUser();
        view.UpdateUserInfo(user);
        if (user.AvatarPath != "defaultAvatar")
        {
            string path = Path.Combine($"{Application.persistentDataPath}/Avatar/{user.AvatarPath}");
            Debug.Log($"加载图片的路径:{path}");
            if (File.Exists(path))
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
            else
            {
                Debug.Log($"加载网络图片");
                HTTPManager.Instance.GetSetAvatarByDB(user.AvatarPath, view.GetAvatarImage()).Forget();
            }
        }
        //NetManager.Send(new MsgGetRoomList());
    }

    #region 用户操作处理

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void HandleQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

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

    #region 坦克控制
    public void StartTankRotation(Vector3 mousePosition)
    {
        model.isRotatingTank = true;
        model.lastMousePosition = mousePosition;
    }

    public void UpdateTankRotation()
    {
        if (!model.isRotatingTank) return;

        var currentPos = Input.mousePosition;
        var deltaX = currentPos.x - model.lastMousePosition.x;
        view.RotateTank(-deltaX * HomePanelModel.TankRotationSpeed * Time.deltaTime);
        model.lastMousePosition = currentPos;
    }

    public void EndTankRotation()
    {
        model.isRotatingTank = false;
    }
    #endregion
}