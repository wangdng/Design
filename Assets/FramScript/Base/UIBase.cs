using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIBase : MonoBase 
{
    public override void ProcessEvent(MsgBase msgbase)
    {
        
    }

    public T GetUICompent<T>(string Name)
    {
        return UIManager.instance.GetGameObject(this, Name).GetComponent<T>();
    }

    public void AddButtonListener(string name,UnityAction Event)
    {
        UIBehaviour obj = GetUICompent<UIBehaviour>(name);

        obj.AddButtonListener(Event);
    }

    public void RegisterSelf(MonoBase mono,params ushort[] msgs)
    {
        UIManager.instance.RegisterMsg(mono, msgs);
    }

    public void UnRegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        UIManager.instance.UnRegisterMsg(mono, msgs);
    }

    public void SendMessage(MsgBase tmpMsg)
    {
        UIManager.instance.SendMessage(tmpMsg);
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
