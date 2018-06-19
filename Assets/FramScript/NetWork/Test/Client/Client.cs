using System.Collections;
using System.Collections.Generic;

using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;

using System.Linq;

using UnityEngine;

public class Client : MonoBehaviour
{
    private Socket ClientSocket;

    private SocketBuff SB;

    private byte[] buffer = new byte[1024];

    void RecvCompleteCallBack(byte[] msg)
    {
        //去掉包头
        int bodyMsgLength = msg.Length - 6;

        //包体的字节数组
        byte[] bodyByte = new byte[bodyMsgLength];

        //从总的消息中(包含包头)把包体的字节数组拷贝到bodyByte中
        Buffer.BlockCopy(msg, 6, bodyByte, 0, bodyMsgLength);

        string str = System.Text.Encoding.UTF8.GetString(bodyByte);

        Debug.Log("从服务端接受过来的消息是:" + str);
    }

    void Start()
    {
        SB = new SocketBuff(6, RecvCompleteCallBack);
        InitSocket();
    }

    public void InitSocket()
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");

        IPEndPoint ep = new IPEndPoint(ip, 18010);

        ClientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

        ClientSocket.BeginConnect(ep, new AsyncCallback(ConnectCallback), ClientSocket);
    }

    public void ConnectCallback(IAsyncResult result)
    {
        ClientSocket.EndConnect(result);

        Debug.Log("客户端链接完成");

    }

    void ReceiveCallBack(IAsyncResult result)
    {
        //客户端接收多少数据
        int length = ClientSocket.EndReceive(result);

        //string str = System.Text.Encoding.Default.GetString(buffer, 0, length);

        //Debug.Log("从服务端接受的数据为:" + str);

        //这里就是一个拆包和粘包的过程了,在具体接受消息的过程中一条协议的包的长度是固定的,比如8个字节
        //但是这里length有可能小于或者等于或者大于8,所以这里就有了一个拆包和粘包的过程
        SB.RecvByte(buffer, length);
    }

    public void BeginRec()
    {
        ClientSocket.BeginReceive(buffer, 0, 1024, SocketFlags.None, ReceiveCallBack, this);
    }

    void SendCallBack(IAsyncResult result)
    {
        int length = ClientSocket.EndSend(result);

        Debug.Log("发送给服务端的数据量为:" + length);
    }

    public void BeginSend(string data)
    {
        byte[] b = System.Text.Encoding.Default.GetBytes(data);

        ClientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, SendCallBack, null);
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetMouseButton(0))
        {
            BeginSend("12345678");
        }

        BeginRec();
	}

    private void OnApplicationQuit()
    {
        ClientSocket.Shutdown(SocketShutdown.Both);
        ClientSocket.Close();
    }
}
