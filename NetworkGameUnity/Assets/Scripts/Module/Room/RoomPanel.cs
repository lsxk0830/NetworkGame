using UnityEngine;
using UnityEngine.UI;

namespace Tank
{
    public class RoomPanel : BasePanel
    {
        private Button startBtn; // 开战按钮
        private Button closeBtn; // 退出按钮
        private Transform content; // 列表容器
        private GameObject playerObj; // 玩家信息物体

        public override void OnInit()
        {
            skinPath = "RoomPanel";
            layer = PanelManager.Layer.Panel;
        }

        public override void OnShow(params object[] para)
        {
            // 寻找组件
            startBtn = skin.transform.Find("CtrlPanel/StartBtn").GetComponent<Button>();
            closeBtn = skin.transform.Find("CtrlPanel/CloseBtn").GetComponent<Button>();
            content = skin.transform.Find("ListPanel/Scroll View/Viewport/Content");
            playerObj = skin.transform.Find("Player").gameObject;
            //不激活玩家信息
            playerObj.SetActive(false);
            //按钮事件
            startBtn.onClick.AddListener(OnStartClick);
            closeBtn.onClick.AddListener(OnCloseClick);
            // 协议监听
            NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
            NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
            NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);
            // 发送查询
            MsgGetRoomInfo msg = new MsgGetRoomInfo();
            NetManager.Send(msg);
        }

        public override void OnClose()
        {
            // 协议取消监听
            NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
            NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
            NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
        }

        #region 协议事件

        /// <summary>
        /// 收到玩家列表协议
        /// </summary>
        private void OnMsgGetRoomInfo(MsgBase msgBse)
        {
            MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBse;
            // 清除玩家列表
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                GameObject go = content.GetChild(i).gameObject;
                Destroy(go);
            }
            // 重新生成列表
            if (msg.Players == null) return;
            for (int i = 0; i < msg.Players.Length; i++)
            {
                GeneratePlayerInfo(msg.Players[i]);
            }
        }

        /// <summary>
        /// 收到退出房间协议
        /// </summary>
        private void OnMsgLeaveRoom(MsgBase msgBse)
        {
            MsgLeaveRoom msg = (MsgLeaveRoom)msgBse;
            if (msg.result == 0) // 成功退出房间
            {
                PanelManager.Open<TipPanel>("退出房间");
                PanelManager.Open<RoomListPanel>();
                Close();
            }
            else
                PanelManager.Open<TipPanel>("退出房间失败");
        }

        /// <summary>
        /// 收到开战协议
        /// </summary>
        private void OnMsgStartBattle(MsgBase msgBse)
        {
            MsgStartBattle msg = (MsgStartBattle)msgBse;
            if (msg.result == 0)//开战
                Close();
            else // 开战失败
                PanelManager.Open<TipPanel>("开战失败!两队至少都需要一名玩家，只有队长可以开始战斗！");
        }

        /// <summary>
        /// 创建一个玩家信息单元
        /// </summary>
        private void GeneratePlayerInfo(PlayerInfo playerInfo)
        {
            GameObject go = Instantiate(playerObj);
            go.transform.SetParent(content);
            go.SetActive(true);
            go.transform.localScale = Vector3.one;
            // 获取组件
            Transform trans = go.transform;
            Text idText = trans.Find("IdText").GetComponent<Text>();
            Text campText = trans.Find("CampText").GetComponent<Text>();
            Text scoreText = trans.Find("ScoreText").GetComponent<Text>();
            // 填充信息
            idText.text = playerInfo.id;
            campText.text = playerInfo.camp == 1 ? "红" : "蓝";
            if (playerInfo.isOwner == 1)
                campText.text = campText.text + "! ";
            scoreText.text = playerInfo.win + " 胜 " + playerInfo.lost + " 负";
        }


        #endregion

        #region UI事件

        /// <summary>
        /// 点击开战按钮
        /// </summary>
        private void OnStartClick()
        {
            MsgStartBattle msg = new MsgStartBattle();
            NetManager.Send(msg);
        }

        /// <summary>
        /// 点击退出按钮
        /// </summary>
        private void OnCloseClick()
        {
            MsgLeaveRoom msg = new MsgLeaveRoom();
            NetManager.Send(msg);
        }

        #endregion
    }
}