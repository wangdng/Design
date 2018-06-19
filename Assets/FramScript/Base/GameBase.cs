﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBase : MonoBase 
{
    public override void ProcessEvent(MsgBase msgbase)
    {

    }

    public void RegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        GameManager.instance.RegisterMsg(mono, msgs);
    }

    public void UnRegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        GameManager.instance.UnRegisterMsg(mono, msgs);
    }

    public void SendMessage(MsgBase tmpMsg)
    {
        GameManager.instance.SendMessage(tmpMsg);
    }

    public ushort[] msgIds = null;

    void OnDestroy()
    {
        if (msgIds != null || msgIds.Length > 0)
        {
            UnRegisterSelf(this, msgIds);
        }
    }
}
