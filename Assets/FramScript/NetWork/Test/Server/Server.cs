using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;

using System.Linq;

using UnityEngine.UI;

public delegate void ChangeTxt(string str);

public class SocketState
{
    Socket socket;

    byte[] buffer;

    public ChangeTxt changeTxt;

    public SocketState(Socket tmpSocket, ChangeTxt action)
    {
        socket = tmpSocket;

        buffer = new byte[1024];

        changeTxt = action;
    }

    void ReceiveCallBack(IAsyncResult result)
    {
        //从客户端接收多少数据
        int length = socket.EndReceive(result);

        string str = System.Text.Encoding.Default.GetString(buffer, 0, length);

        if(changeTxt != null)
        {
            changeTxt(str);
        }

        //包头长度为8,本例要发送的消息为"12345678";
        int bodyLength = 8;

        //本消息的消息id
        ushort msgId = 9010;

        //将包体长度转换为一个4个字节的byte数组
        byte[] bodyLengthByte = BitConverter.GetBytes(bodyLength);

        //将消息id转换为一个2个字节的byte数组
        byte[] msgIdByte = BitConverter.GetBytes(msgId);

        //消息体(如protobuff,json等)
        byte[] body = System.Text.Encoding.Default.GetBytes(str);

        //进行连接
        byte[] Headbyte = bodyLengthByte.Concat(msgIdByte).ToArray();

        byte[] allMsgByte = Headbyte.Concat(body).ToArray();

        //发送最终的消息
        BeginSend(allMsgByte);
    }

    public void BeginRec()
    {
        socket.BeginReceive(buffer, 0, 1024, SocketFlags.None, ReceiveCallBack, this);
    }

    void SendCallBack(IAsyncResult result)
    {
        int length = socket.EndSend(result);

        Debug.Log("发送给客户端的数据量为:" + length);
    }

    public void BeginSend(string data)
    {
        byte[] b = System.Text.Encoding.Default.GetBytes(data);

        socket.BeginSend(b, 0, b.Length, SocketFlags.None, SendCallBack, null);
    }

    public void BeginSend(byte[] data)
    {
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallBack, null);
    }
}

public class Server : MonoBehaviour
{
    private Socket  ServerSocket;

    List<SocketState> SocketArr;
    bool isListen = true;

    public Text txt;

    void Start()
    {
        InitServerSocket();
    }

    //接受客户端发过来的消息
    public void BeginRec()
    {
        while(isListen)
        {
            try
            {
                ServerSocket.BeginAccept(new AsyncCallback(AstCallback), ServerSocket);
            }
            catch(Exception  e)
            {

            }

            Thread.Sleep(1000);
        }
    }

    public void ChangeText(string str)
    {
        txt.text = str;
    }

    public void AstCallback(IAsyncResult result)
    {
        Socket Listener = (Socket)result.AsyncState;

        //阻塞的,会一直等着有客户端链接过来
        Socket ClientSocket = Listener.EndAccept(result);

        SocketState socketState = new SocketState(ClientSocket, ChangeText);

        SocketArr.Add(socketState);

        txt.text = "有一个客户端连接过来了";
    }

    public void InitServerSocket()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 18010);

        ServerSocket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        SocketArr = new List<SocketState>();

        ServerSocket.Bind(ep);

        ServerSocket.Listen(100);

        Thread tmpTread = new Thread(BeginRec);

        tmpTread.Start();
    }

    void OnApplicationQuit()
    {
        isListen = false;
        ServerSocket.Shutdown(SocketShutdown.Both);
        ServerSocket.Close();
    }

    private void Update()
    {
        if(SocketArr.Count >0)
        {
            for(int i=0; i < SocketArr.Count; i++)
            {
                SocketArr[i].BeginRec();
            }
        }
    }
}
