using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : ManagerBase 
{
    public static UIManager instance;

    void Awake()
    {
        instance = this;
    }

    public void SendMessage(MsgBase tmpMsg)
    {
        //如果该消息就是属于UIManager,则直接发送
        if (tmpMsg.GetMsgManager() == MsgManager.UIManager)
        {
            ProcessEvent(tmpMsg);
        }
        else//否则发给消息处理中心进行处理
        {
            MsgCenter.instance.SendToMessage(tmpMsg);
        }
    }

    public GameObject GetGameObject(UIBase monoBase, string UIname)
    {
        return sonMembers[monoBase][UIname];
    }


    public Dictionary<UIBase, Dictionary<string, GameObject>> sonMembers = new Dictionary<UIBase, Dictionary<string, GameObject>>();

    public void RegisterGameObject(UIBase monoBase, string name, GameObject go)
    {
        if (!sonMembers.ContainsKey(monoBase))
        {
            Dictionary<string, GameObject> tmpDic = new Dictionary<string, GameObject>();
            tmpDic.Add(name, go);
            sonMembers.Add(monoBase, tmpDic);
        }
        else
        {
            Dictionary<string, GameObject> tmpDic = sonMembers[monoBase];

            if (!tmpDic.ContainsKey(name))
            {
                tmpDic.Add(name, go);
            }
        }
    }

    public void UnRegisterGameObject(UIBase monoBase,string name)
    {
        if (sonMembers.ContainsKey(monoBase))
        {
            Dictionary<string, GameObject> tmpDic = sonMembers[monoBase];

            if (tmpDic.ContainsKey(name))
            {
                tmpDic.Remove(name);
            }

            if (tmpDic.Count == 0)
                sonMembers.Remove(monoBase);
        }
    }

    private void OnDestroy()
    {
        sonMembers.Clear();
        sonMembers = null;
    }
}
