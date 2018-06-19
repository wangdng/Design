using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorManager : ManagerBase {

    public static CharactorManager instance;

    void Awake()
    {
        instance = this;
    }

    public void SendMessage(MsgBase tmpMsg)
    {
        //如果该消息就是属于AssetManager,则直接发送
        if (tmpMsg.GetMsgManager() == MsgManager.CharactorManager)
        {
            ProcessEvent(tmpMsg);
        }
        else//否则发给消息处理中心进行处理
        {
            MsgCenter.instance.SendToMessage(tmpMsg);
        }
    }
}
