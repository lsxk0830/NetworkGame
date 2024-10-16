using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tank
{
    public class RoomListPanel : BasePanel
    {
        /// <summary>
        /// 账号文本
        /// </summary>
        private TMP_Text idText;
        /// <summary>
        /// 战绩文本
        /// </summary>
        private TMP_Text scoreText;
        /// <summary>
        /// 创建房间按钮
        /// </summary>
        private Button createButton;
        /// <summary>
        /// 刷新房间按钮
        /// </summary>
        private Button reflashButton;
        /// <summary>
        /// 列表容器
        /// </summary>
        private Transform content;
        /// <summary>
        /// 房间物体
        /// </summary>
        private GameObject roomObj;

        public override void OnInit()
        {
            skinPath = "RoomListPanel";
            layer = PanelManager.Layer.Panel;
        }

        public override void OnShow(params object[] para)
        {
            // 寻找组件
            idText = skin.transform.Find("InfoPanel/IdText").GetComponent<TMP_Text>();
            scoreText = skin.transform.Find("InfoPanel/ScoreText").GetComponent<TMP_Text>();
            createButton = skin.transform.Find("CtrlPanel/CreateBtn").GetComponent<Button>();
            reflashButton = skin.transform.Find("CtrlPanel/ReflashBtn").GetComponent<Button>();
            content = skin.transform.Find("ListPanel/Scroll View/Viewport/Content");
            roomObj = skin.transform.Find("Room").gameObject;
            // 按钮事件
            createButton.onClick.AddListener(OnCreatClick);
            reflashButton.onClick.AddListener(OnReflashClick);
            //不激活房间
            roomObj.SetActive(false);
            //显示id
            idText.text = GameMain.id;
            //协议监听
            NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
            NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
            NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
            NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
            //发送查询
            MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
            NetManager.Send(msgGetAchieve);
            MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
            NetManager.Send(msgGetRoomList);
        }

        public override void OnClose()
        {
            //协议取消监听
            NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
            NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
            NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
            NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        }

        #region 协议事件

        /// <summary>
        /// 收到查询成绩协议
        /// </summary>
        private void OnMsgGetAchieve(MsgBase msgBse)
        {
            MsgGetAchieve msg = (MsgGetAchieve)msgBse;
            scoreText.text = $"{msg.win} 胜 << {msg.lost} 负";
        }

        /// <summary>
        /// 收到获取房间列表协议
        /// </summary>
        private void OnMsgGetRoomList(MsgBase msgBse)
        {
            MsgGetRoomList msg = (MsgGetRoomList)msgBse;
            // 清除房间列表
            for (int i = content.childCount - 1; i >= 0; i++)
            {
                GameObject go = content.GetChild(i).gameObject;
                Destroy(go);
            }
            // 如果没有房间，不需要进一步处理
            if (msg.rooms == null) return;

            for (int i = 0; i < msg.rooms.Length; i++)
            {
                GenerateRoom(msg.rooms[i]);
            }
        }

        /// <summary>
        /// 收到新建房间协议
        /// </summary>
        /// <param name="msgBse"></param>
        private void OnMsgCreateRoom(MsgBase msgBse)
        {
            MsgCreateRoom msg = (MsgCreateRoom)msgBse;
            // 成功创建房间
            if (msg.result == 0)
            {
                PanelManager.Open<TipPanel>("创建成功");
                PanelManager.Open<RoomPanel>();
                Close();
            }
            else
            {
                PanelManager.Open<TipPanel>("创建房间失败");
            }
        }

        /// <summary>
        /// 收到进入房间协议
        /// </summary>
        private void OnMsgEnterRoom(MsgBase msgBse)
        {
            MsgEnterRoom msg = (MsgEnterRoom)msgBse;
            if (msg.result == 0)
            {
                PanelManager.Open<RoomPanel>();
                Close();
            }
            else
                PanelManager.Open<TipPanel>("进入房间失败");
        }

        private void GenerateRoom(RoomInfo roomInfo)
        {
            //创建物体
            GameObject go = Instantiate(roomObj);
            go.transform.SetParent(content);
            go.SetActive(true);
            go.transform.localScale = Vector3.one;
            //获取组件
            Transform trans = go.transform;
            TMP_Text idText = trans.Find("IdText").GetComponent<TMP_Text>();
            TMP_Text countText = trans.Find("CountText").GetComponent<TMP_Text>();
            TMP_Text statusText = trans.Find("StatusText").GetComponent<TMP_Text>();
            Button btn = trans.Find("JointButton").GetComponent<Button>();
            //填充信息
            idText.text = roomInfo.id.ToString();
            countText.text = roomInfo.count.ToString();
            statusText.text = roomInfo.status == 0 ? "准备中" : "战斗中";
            //按钮事件
            btn.name = idText.text;
            btn.onClick.AddListener(() => { OnJoinClick(btn.name); });
        }

        /// <summary>
        /// 点击加入房间按钮
        /// </summary>
        /// <param name="idString">房间序号</param>
        private void OnJoinClick(string idString)
        {
            MsgEnterRoom msg = new MsgEnterRoom();
            msg.id = int.Parse(idString);
            NetManager.Send(msg);
        }
        #endregion
        #region UI事件

        /// <summary>
        /// 点击新建房间按钮
        /// </summary>
        private void OnCreatClick()
        {
            MsgCreateRoom msg = new MsgCreateRoom();
            NetManager.Send(msg);
        }

        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        public void OnReflashClick()
        {
            MsgGetRoomList msg = new MsgGetRoomList();
            NetManager.Send(msg);
        }

        #endregion
    }
}