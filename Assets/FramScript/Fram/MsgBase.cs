using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LuaInterface;

public class MsgBase  
{
    public ushort MsgId;

    public MsgManager GetMsgManager()
    {
        //比如(int)(3003/3000) = 1,然后再乘以3000就可以得到编号为3003的消息属于UIManager
        int realut = (int)(MsgId / FramTool.MsgSpan);

        return (MsgManager)(realut * FramTool.MsgSpan);
    }

    public MsgBase(ushort id)
    {
        MsgId = id;
    }

	//用来区分是不是网络消息
	public virtual byte GetState()
	{
		return 127;
	}

    public MsgBase()
    {
        MsgId = 0;
    }

    public void ChangeMsgId(ushort changid)
    {
        MsgId = changid;
    }
}

//如果需要发送带参数的消息就继承MsgBase自己重写一个类
public class MsgTransform:MsgBase
{
    public Transform transform;

    public MsgTransform(ushort msgid,Transform trans)
    {
        MsgId = msgid;

        transform = trans;
    }
}

//AssetBundle相关消息
//上层发送给我们的消息
public class HunkAssetMsg : MsgBase
{
    public ushort backMsgid;

    public string sceneName;

    public string bundleName;

    public string ABName;

    public bool isSingle;

    public HunkAssetMsg(string sceneName, string bundleName, string ABName, bool isSingle, ushort backMsgid, ushort msgId)
    {
        this.backMsgid = backMsgid;
        this.bundleName = bundleName;
        this.ABName = ABName;
        this.isSingle = isSingle;
        this.MsgId = msgId;
        this.sceneName = sceneName;
    }
}

//我们返回给上层的消息
public class HunkAssetBackMsg : MsgBase
{
    public UnityEngine.Object[] value;

    public HunkAssetBackMsg()
    {
        this.MsgId = 0;

        value = null;
    }

    public void Change(ushort msgId, params UnityEngine.Object[] values)
    {
        this.MsgId = msgId;

        value = values;
    }

    public void Change(ushort msgId)
    {
        this.MsgId = msgId;
    }

    public void Change(params UnityEngine.Object[] values)
    {
        value = values;
    }
}

public class MsgFloat : MsgBase
{
    float value;
    public MsgFloat()
    {

    }

    public MsgFloat(float tmpValue)
    {
        value = tmpValue;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetValue(float tmpValue)
    {
        value = tmpValue;
    }
}

public class NetWorkMsg : MsgBase
{
    byte[] buff;

    public NetWorkMsg()
    {

    }

    public NetWorkMsg(byte[] msg)
    {
        buff = msg;

        this.MsgId = BitConverter.ToUInt16(msg, 4);
    }

    public byte[] GetBuff()
    {
        return buff;
    }

    public void SetBuff(byte[] tmpbyte)
    {
        buff = tmpbyte;
    }
}

public class TcpConnectMsg: MsgBase
{
    public string ip;
    public ushort port;

    public TcpConnectMsg(ushort msgId,string Ip,ushort Port)
    {
        MsgId = msgId;
        ip = Ip;
        port = Port;
    }
}


public class TcpSendMsg : MsgBase
{
    public NetWorkMsg sendData;

	public TcpSendMsg()
	{
		
	}

    public TcpSendMsg(ushort msgId, NetWorkMsg msg)
    {
        this.MsgId = msgId;
        sendData = msg;
    }

	//lua中传过来的方法,其中Buffer需要转换为C#能用的protobuff(在LTCPSokcet.lua中调用了这个函数)
    //netid是协议号
	public void ChangeMsgToLua(ushort msgId,LuaByteBuffer Buffer,ushort netid)
	{
		this.MsgId = msgId;
		NetWorkMsg data = new NetWorkMsg ();
		data.SetBuff (Buffer.buffer);

		sendData = data;
	}
}

public class UdpConnectMsg : MsgBase
{
    public ushort port;

    public int buffCount;
    public UDPSocketdelegate UdpDelegate;

    public UdpConnectMsg(ushort msgId, ushort Port, int buffLength, UDPSocketdelegate tmpDelegate)
    {
        MsgId = msgId;
        port = Port;

        buffCount = buffLength;

        UdpDelegate = tmpDelegate;
    }
}

public class UdpSendMsg : MsgBase
{
    public ushort port;

    public string Ip;

    public byte[] sendData;

    public UdpSendMsg(ushort msgId,string ip,byte[] data,ushort Port)
    {
        MsgId = msgId;
        port = Port;
        sendData = data;

        Ip = ip;
    }
}

public class JoyStickMsg : MsgBase
{
    Vector2 joyStickData;

    public JoyStickMsg() { }

    public JoyStickMsg(ushort msgId, Vector2 data)
    {
        MsgId = msgId;
        joyStickData = data;
    }

    public Vector2 GetjoyStickData()
    {
        return joyStickData;
    }

    public void ChangejoyStickData(Vector2 data)
    {
        joyStickData = data;
    }
}

public class RunMsg : MsgBase
{
    float value;

    public RunMsg(ushort msgid,float data)
    {
        MsgId = msgid;
        value = data;
    }

    public void ChangeValue(float data)
    {
        value = data;
    }

    public float GetValue()
    {
        return value;
    }
}
