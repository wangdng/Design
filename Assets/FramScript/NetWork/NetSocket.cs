using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Text;

public delegate void CallBackNormal(bool sucess, SocketError error, string excption);

public delegate void CallBackRecv(bool sucess, SocketError error, string excption, byte[] recvMsg, string message);

public enum SocketError
{
    Default = 0,
    TimeOut,
    SocketNull,
    SocketUnConnect,
    ConnectSucess,
    ConnectUnSucessUnKown,
    ConnectError,
    SendUnSucessUnKown,
    SendSucess,
    RecvUnSucessUnKown,
    RecvSucess,
    DisConnectUnKown,
    DisConnectSucess,
}

public class NetSocket
{
    public CallBackNormal callBackConnect;
    public CallBackNormal callBackSend;
    public CallBackNormal callBackDisconnect;

    public CallBackRecv callBackRecv;

    private SocketError errorSocket;

    private string addressIp;

    private ushort port;

    private Socket clientSocket;

    private SocketBuff recvBuff;

    private byte[] recvCache;

    #region 构造
    public NetSocket()
    {
        recvBuff = new SocketBuff(6, RecvCompleteCallBack);

        recvCache = new byte[1024];
    }
    #endregion


    #region 检测连接服务器是否超时
    bool CheckIsTimeOut(IAsyncResult at)
    {
        int i = 0;
        while (at.IsCompleted == false)
        {
            i++;
            if (i > 20)
            {
                return true;
            }
            Thread.Sleep(100);
        }

        return false;
    }
    #endregion


    #region 连接服务器

    public bool isConnected
    {
        get
        {
            if (clientSocket != null && clientSocket.Connected)
                return true;
            else
                return false;
        }
    }

    public void ConnectCallback(IAsyncResult result)
    {
        try
        {
            clientSocket.EndConnect(result);

            if (clientSocket.Connected == false)
            {
                errorSocket = SocketError.ConnectUnSucessUnKown;
                this.callBackConnect(false, errorSocket, "连接失败");
                return;
            }
            else
            {
                errorSocket = SocketError.ConnectSucess;
                this.callBackConnect(true, errorSocket, "连接成功");
            }
        }
        catch (Exception e)
        {
            errorSocket = SocketError.ConnectUnSucessUnKown;
            this.callBackConnect(false, errorSocket, e.ToString());
        }
    }

    public void AsynConnect(string ip, ushort tmpport, CallBackNormal connectCallback, CallBackRecv tmpRecvCallback)
    {
        errorSocket = SocketError.Default;

        this.addressIp = ip;

        this.port = tmpport;

        this.callBackConnect = connectCallback;

        this.callBackRecv = tmpRecvCallback;

        if (clientSocket != null && clientSocket.Connected)
        {
            errorSocket = SocketError.ConnectError;
            this.callBackConnect(false, errorSocket, "重复链接");
        }
        else if (clientSocket == null || !clientSocket.Connected)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse(ip);

            IPEndPoint ep = new IPEndPoint(address, port);

            IAsyncResult realut = clientSocket.BeginConnect(ep, ConnectCallback, null);

            if (CheckIsTimeOut(realut))
            {
                errorSocket = SocketError.TimeOut;
                this.callBackConnect(false, errorSocket, "连接超时");
            }
        }
    }
    #endregion

    #region 接受消息

    public void Recv()
    {
        if(clientSocket != null && clientSocket.Connected)
        {
            IAsyncResult ar = clientSocket.BeginReceive(recvCache, 0, recvCache.Length, SocketFlags.None, RecvCallBack, clientSocket);

            if(CheckIsTimeOut(ar))
            {
                errorSocket = SocketError.RecvUnSucessUnKown;
                this.callBackRecv(false, errorSocket, "接收消息时超时", null, "");
            }
        }
    }

    public void RecvCallBack(IAsyncResult ar)
    {
        try
        {
            if (!clientSocket.Connected)
            {
                errorSocket = SocketError.RecvUnSucessUnKown;
                this.callBackRecv(false, errorSocket, "接收消息时ClientSocket断开", null, "");
            }
            else
            {
                int length = clientSocket.EndReceive(ar);

                if (length == 0)
                    return;

                recvBuff.RecvByte(recvCache, length);
            }
        }
        catch(Exception e)
        {
            errorSocket = SocketError.RecvUnSucessUnKown;

            this.callBackRecv(false, errorSocket, "接收消息时出现异常:" + e.ToString(), null, "");
        }
        Recv();//循环 接受消息
    }

    //最终接受到一个和我们这条协议正好长度相等时的包时的回调,如本例时8
    void RecvCompleteCallBack(byte[] msg)
    {
        errorSocket = SocketError.RecvSucess;
        this.callBackRecv(true, errorSocket, "", msg, "");
    }

    #endregion

    #region 发送消息

    void SendCallBack(IAsyncResult result)
    {
        try
        {
            int length = clientSocket.EndSend(result);

            if(length > 0)
            {
                errorSocket = SocketError.SendSucess;
                this.callBackSend(true, errorSocket, "");
            }
            else
            {
                errorSocket = SocketError.SendUnSucessUnKown;
                this.callBackSend(true, errorSocket, "客户端发送消息时发送的数据长度为0");
            }
        }
        catch (Exception e)
        {
            errorSocket = SocketError.SendUnSucessUnKown;
            this.callBackSend(true, errorSocket, "客户端发送消息时抛出的异常为:" + e.ToString());
        }
    }

    public void AsynSend(byte[] sendData,CallBackNormal sendCallback)
    {
        errorSocket = SocketError.Default;
        this.callBackSend = sendCallback;

        if(clientSocket == null)
        {
            errorSocket = SocketError.SocketNull;
            this.callBackSend(false, errorSocket, "客户端发送数据时ClientSocket为Null");
            return;
        }

        if(!clientSocket.Connected)
        {
            errorSocket = SocketError.SendUnSucessUnKown;
            this.callBackSend(false, errorSocket, "客户端发送数据时ClientSocket断开了");
            return;
        }

        IAsyncResult  ar = clientSocket.BeginSend(sendData,0, sendData.Length,SocketFlags.None, SendCallBack, clientSocket );

        if(CheckIsTimeOut(ar))
        {
            errorSocket = SocketError.SendUnSucessUnKown;
            this.callBackSend(false, errorSocket, "客户端发送数据时超时了");
            return;
        }
    }
    #endregion

    #region 断开连接的处理

    void DisConnectCallBack(IAsyncResult result)
    {
        try
        {
            clientSocket.EndDisconnect(result);

            clientSocket.Close();
            clientSocket = null;

            errorSocket = SocketError.DisConnectSucess;
            this.callBackDisconnect(true, errorSocket, "客户端成功断开了连接");

        }
        catch (Exception e)
        {
            errorSocket = SocketError.DisConnectUnKown;
            this.callBackDisconnect(true, errorSocket, "客户端成功断开连接时抛出了异常为:" + e.ToString());
        }
    }

    public void AsynDisConnect(CallBackNormal disCallback)
    {
       errorSocket = SocketError.Default;

       this.callBackDisconnect = disCallback;

       if (clientSocket == null)
       {
           errorSocket = SocketError.DisConnectUnKown;
           this.callBackDisconnect(false, errorSocket, "客户端断开连接的时ClientSocket为Null");
       }
       else if(!clientSocket.Connected)
       {
           errorSocket = SocketError.DisConnectUnKown;
           this.callBackDisconnect(false, errorSocket, "客户端断开连接的时ClientSocket已经断开了");
       }
       else
       {
           IAsyncResult ar = clientSocket.BeginDisconnect(false, DisConnectCallBack, clientSocket);

           if(CheckIsTimeOut(ar))
           {
               errorSocket = SocketError.DisConnectUnKown;
               this.callBackDisconnect(false, errorSocket, "客户端断开连接时超时");
           }
       }
    }
    #endregion
}
