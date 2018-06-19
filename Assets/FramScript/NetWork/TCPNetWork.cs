using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

using System;

public class TCPNetWork : NetWorkBase
{
    NetWorkServer socket;

    public override void ProcessEvent(MsgBase msgbase)
    {
        switch(msgbase.MsgId)
        {
            case (ushort)NetWorkEvent.TcpConnect:
                {
                    TcpConnectMsg msg = (TcpConnectMsg)msgbase;

                    //进行和服务器的连接
                    socket = new NetWorkServer(msg.ip, msg.port);
                }
                break;
            case (ushort)NetWorkEvent.TcpSendMsg:
                {
                    TcpSendMsg msg = (TcpSendMsg)msgbase;

                    //将发送的消息放进发送消息池
                    socket.PutSendMsg(msg.sendData);
                }
                break;
        }
    }
    private void Awake()
    {
       msgIds = new ushort[]
       {
           //请求连接服务器
            (ushort)NetWorkEvent.TcpConnect,
            //请求发送数据
            (ushort)NetWorkEvent.TcpSendMsg,
       };

       RegisterSelf(this, msgIds);
    }

    void Update()
    {
        if(socket != null)
        {
            //在主线程中不断接收消息
            socket.Update();
        }
    }
}

//通用模板类来反序列化从server接收到的消息(byte[])为protobuff对应的类
public class TcpNetMsgs<T> : NetWorkMsg where T : IExtensible
{
    //从Socket中接收byte[]进行反序列化
    public IExtensible GetPBClass<U>() where U : IExtensible
    {
        byte[] data = new byte[GetBuff().Length - 6];

        Buffer.BlockCopy(GetBuff(), 6, data, 0, data.Length);

        return IProtoTools.Deserialize<U>(data);
    }

    public IExtensible GetPBClass<U>(byte[] msg) where U : IExtensible
    {
        SetBuff(msg);

        return GetPBClass<U>();
    }

    //序列化
    public TcpNetMsgs(T tmp, ushort msgId)
    {
        this.MsgId = msgId;

        byte[] tmpByte = IProtoTools.Serialize(tmp);

        //把消息体的长度转换为4个字节的数据(tmpByte.Length是int类型的数据,使用BitConverter.GetBytes转换后就变成了一个4个字节的数据)
        byte[] protoBuffLength = BitConverter.GetBytes(tmpByte.Length);

        //将消息id转换为2个字节的数据(msgId是ushort类型的数据,使用BitConverter.GetBytes转换后就变成了一个2个字节的数据)
        byte[] msgid = BitConverter.GetBytes(msgId);

        //最终数据是包体长度和包头(6个字节)的和
        byte[] finalData = new byte[tmpByte.Length + 6];

        Buffer.BlockCopy(protoBuffLength, 0, finalData, 0, 4);

        Buffer.BlockCopy(msgid, 0, finalData, 4, 2);

        Buffer.BlockCopy(tmpByte, 0, finalData, 6, tmpByte.Length);

        SetBuff(finalData);
    }

    //优化消息使用,我们每次不用都重新new一个TcpNetMsgs来使用,当两条消息的MsgId相同时,但是消息内容不一样时先全局化一个TcpNetMsgs,然后直接调用此函数就可以改变消息内容,而不改变消息id
    public void ChangeMsgData<V>(V tmpData) where V : IExtensible
    {
        byte[] tmpByte = IProtoTools.Serialize(tmpData);

        //把消息体的长度转换为4个字节的数据(tmpByte.Length是int类型的数据,使用BitConverter.GetBytes转换后就变成了一个4个字节的数据)
        byte[] protoBuffLength = BitConverter.GetBytes(tmpByte.Length);

        //将消息id转换为2个字节的数据(msgId是ushort类型的数据,使用BitConverter.GetBytes转换后就变成了一个2个字节的数据)
        byte[] msgid = BitConverter.GetBytes(MsgId);

        //最终数据是包体长度和包头(6个字节)的和
        byte[] finalData = new byte[tmpByte.Length + 6];

        Buffer.BlockCopy(protoBuffLength, 0, finalData, 0, 4);

        Buffer.BlockCopy(msgid, 0, finalData, 4, 2);

        Buffer.BlockCopy(tmpByte, 0, finalData, 6, tmpByte.Length);

        SetBuff(finalData);
    }

    //消息内容一样,消息id不一样
    public void ChangeMsgID(ushort msgId)
    {
        this.MsgId = msgId;

        byte[] OriginalMsg = GetBuff();

        byte[] newData = new byte[OriginalMsg.Length];

        byte[] newMsgid = BitConverter.GetBytes(msgId);

        Buffer.BlockCopy(OriginalMsg, 0, newData, 0, 4);
        Buffer.BlockCopy(newMsgid, 0, newData, 4, 2);
        Buffer.BlockCopy(OriginalMsg, 6, newData, 6, OriginalMsg.Length -6);

        SetBuff(newData);
    }
}
