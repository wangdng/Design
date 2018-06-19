using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;

using System.Net;

using System.Threading;

public delegate void UDPSocketdelegate(byte[] pbuff,int dwCount,string tmpIp,ushort port);

public class UDPSocket 
{
    UDPSocketdelegate SocketDelegate;

    Socket  udpSocket;

    IPEndPoint udpIp;

    byte[] buff;

    Thread RecvThread;

    bool isRunning;

    public UDPSocket(int buffLength, UDPSocketdelegate tmpDelegate,ushort port)
    {
        udpIp = new IPEndPoint(IPAddress.Any, port);

        UDPConnect();

        SocketDelegate = tmpDelegate;

        buff = new byte[buffLength];

        RecvThread = new Thread(RecvDataThread);

        RecvThread.Start();
    }

    //链接,UDP不稳定,需要有重连的请求,所以单独放在一个函数中处理
    public void UDPConnect()
    {
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        udpSocket.Bind(udpIp);
    }

    void RecvDataThread()
    {
        while (isRunning)
        {
            if (udpSocket == null || udpSocket.Available < 1)
            {
                Thread.Sleep(100);
                continue;
            }

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);

            EndPoint remote = (EndPoint)ep;

            //UDP接收数据数量,将接收的数据源存在remote中(UDP可以从任何ip地址接收数据,只要是任何ip上有一个相同的端口号的进程给该UDP发送消息,都能接收到)
            int recvDataCount = udpSocket.ReceiveFrom(buff, ref remote);

            //将UDP接收的数据传给上层
            if (SocketDelegate != null)
            {
                SocketDelegate(buff, recvDataCount, remote.AddressFamily.ToString(), (ushort)ep.Port);
            }
        }
    }

    public int SendData(byte[] sendData,string ip,ushort port)
    {
        if (udpSocket == null || !udpSocket.Connected)
        {
            UDPConnect();
        }

        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), (int)port);

        int sendDataCount = udpSocket.SendTo(sendData, sendData.Length,SocketFlags.None, ep);

        return sendDataCount;
    }
}
