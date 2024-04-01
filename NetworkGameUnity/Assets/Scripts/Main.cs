using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // 任务模型预制体
    public GameObject humanPrefab;

    // 人物列表
    public BaseHuman myHuman;

    public Dictionary<string, BaseHuman> otherHumans;

    private void Start()
    {
        otherHumans = new Dictionary<string, BaseHuman>();

        // 网络模块
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("Attack", OnAttack);
        NetManager.Connect("127.0.0.1", 8888);

        // 添加一个角色
        GameObject obj = Instantiate(humanPrefab);
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();

        // 发送Enter协议
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        string sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += eul.y;
        NetManager.Send(sendStr);

        // 请求玩家列表
        NetManager.Send("List|");
    }

    private void Update()
    {
        NetManager.Update();
    }

    private void OnEnter(string msg)
    {
        Debug.Log($"OnEnter:{msg}");

        // 解析参数
        string[] split = msg.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        if (desc == NetManager.GetDesc()) return; // 是自己
        // 添加角色
        GameObject obj = Instantiate(humanPrefab);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0, eulY, 0);
        BaseHuman h = obj.AddComponent<SyncHuman>();
        h.desc = desc;
        otherHumans.Add(desc, h);
    }

    private void OnList(string msg)
    {
        Debug.Log($"OnList:{msg}");
        // 解析参数
        string[] split = msg.Split(",");
        int count = (split.Length - 1) / 6;
        for (int i = 0; i < count; i++)
        {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            float hp = int.Parse(split[i * 6 + 5]);
            // 是自己
            if (desc == NetManager.GetDesc())
                continue;
            // 添加一个角色
            GameObject obj = Instantiate(humanPrefab);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }

    private void OnMove(string msg)
    {
        Debug.Log($"OnMove:{msg}");
        // 解析消息
        string[] split = msg.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        // 移动
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman human = otherHumans[desc];
        Vector3 targetPos = new Vector3(x, y, z);
        human.MoveTo(targetPos);
    }

    private void OnLeave(string msg)
    {
        Debug.Log($"OnLeave:{msg}");
        // 解析参数
        string[] split = msg.Split(",");
        string desc = split[0];
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        Destroy(h.gameObject);
        otherHumans.Remove(desc);
    }

    private void OnAttack(string msg)
    {
        string[] split = msg.Split(",");
        string desc = split[0];
        float eulY = float.Parse(split[1]);
        if (!otherHumans.ContainsKey(desc))
            return;
        SyncHuman h = (SyncHuman)otherHumans[desc];
        h.SyncAttack(eulY);
    }
}