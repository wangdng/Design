using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : ManagerBase {

    public static AssetManager instance;

    void Awake()
    {
        instance = this;
    }

    public void SendMessage(MsgBase tmpMsg)
    {
        //如果该消息就是属于AssetManager,则直接发送
        if (tmpMsg.GetMsgManager() == MsgManager.AssetManager)
        {
            ProcessEvent(tmpMsg);
        }
        else//否则发给消息处理中心进行处理
        {
            MsgCenter.instance.SendToMessage(tmpMsg);
        }
    }

   //public GameObject GetGameObject(string name)
    //{
        //return sonMembers[name];
    //}

    //对于AssetManager是没有GameObject存起来然后方便查找,所以这里注释掉
    //public Dictionary<string, GameObject> sonMembers = new Dictionary<string, GameObject>();

    //public void RegisterGameObject(string name, GameObject go)
    //{
        //if (!sonMembers.ContainsKey(name))
        //{
           // sonMembers.Add(name, go);
        //}
    //}

   //public void UnRegisterGameObject(string name)
    //{
        //if (sonMembers.ContainsKey(name))
        //{
            //sonMembers.Remove(name);
        //}
    //}
}
