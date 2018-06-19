using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;

public class NetWorkServer
{
    private Queue<NetWorkMsg> sendMsgPool;//发送的消息池
    private Queue<NetWorkMsg> RecvMsgPool;//接收的消息池

    private NetSocket netSocket;

    Thread sendThread;

    public NetWorkServer(string ip,ushort port)
    {
        sendMsgPool = new Queue<NetWorkMsg>();
        RecvMsgPool = new Queue<NetWorkMsg>();

        netSocket = new NetSocket();

        netSocket.AsynConnect(ip, port, AsynConnectCallBack, AsynRecvCallBack);
    }

    void AsynConnectCallBack(bool sucess, SocketError error, string excption)
    {
        //如果连接成功启动发送线程
        if(sucess)
        {
            sendThread = new Thread(Send);
        }
    }

    void AsynRecvCallBack(bool sucess, SocketError error, string excption, byte[] recvMsg, string message)
    {
        if(sucess)
        {
            Receive(recvMsg);
        }
        else
        {

        }
    }

    #region 发送

    public void PutSendMsg(NetWorkMsg msg)
    {
        lock(sendMsgPool)
        {
            sendMsgPool.Enqueue(msg);
        }
    }

    void AsynSendCallBack(bool sucess, SocketError error, string excption)
    {
        if(sucess)
        {

        }
        else
        {

        }
    }
     
    void Send()
    {
        if(netSocket != null && netSocket.isConnected)
        {
            lock(sendMsgPool)
            {
                while(sendMsgPool.Count > 0)
                {
                    NetWorkMsg curSendMsg = sendMsgPool.Dequeue();

                    netSocket.AsynSend(curSendMsg.GetBuff(), AsynSendCallBack);
                }

                Thread.Sleep(100);
            }
        }
    }
    #endregion

    #region 接收
    void Receive(byte[] data)
    {
        NetWorkMsg tmpMsg = new NetWorkMsg(data);

        if (RecvMsgPool != null)
            RecvMsgPool.Enqueue(tmpMsg);
    }

    public void Update()
    {
        if (RecvMsgPool != null)
        {
            while (RecvMsgPool.Count > 0)
            {
                NetWorkMsg msg = RecvMsgPool.Dequeue();

                AnalyseData(msg);
            }
        }
    }

    //接收到消息通过消息处理中心来分发到对应的模块进行处理
    void AnalyseData(NetWorkMsg msg)
    {
        MsgCenter.instance.SendToMessage(msg);
    }
    #endregion

    #region 断开连接

    void DisConnectCallBack(bool sucess, SocketError error, string excption)
    {
        if(sucess)
        {
            sendThread.Abort();
        }
        else
        {

        }
    }

    public void DisConnect()
    {
        if(netSocket != null && netSocket.isConnected)
        {
            netSocket.AsynDisConnect(DisConnectCallBack);
        }
    }
    #endregion
}
