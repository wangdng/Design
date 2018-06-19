using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPFram :UIBase
{

    public UDPSocket udpSocket;

    public override void ProcessEvent(MsgBase msgbase)
    {
        switch (msgbase.MsgId)
        {
            case (ushort)NetWorkEvent.UdpConnect:
                {
                    UdpConnectMsg msg = (UdpConnectMsg)msgbase;

                    udpSocket = new UDPSocket(msg.buffCount, msg.UdpDelegate, msg.port);

                    break;
                }
            case (ushort)NetWorkEvent.UdpSendMsg:
                {
                    UdpSendMsg msg = (UdpSendMsg)msgbase;

                    if (udpSocket == null)
                    {
                        Debug.LogError("udp应该先进行链接才能发送数据");
                        return;
                    }

                    udpSocket.SendData(msg.sendData, msg.Ip, msg.port);

                    break;
                }
        }
    }

    private void Awake()
    {
        msgIds = new ushort[]
        {
            (ushort) NetWorkEvent.UdpConnect,
             (ushort) NetWorkEvent.UdpSendMsg,
        };

        RegisterSelf(this, msgIds);
    }

    public void StartUdpConnect()
    {
        UdpConnectMsg udpConnect = new UdpConnectMsg((ushort)NetWorkEvent.UdpConnect, 18181, 1024, UdpRecvDelegate);

        SendMessage(udpConnect);
    }

    public void UdpSendData()
    {
        byte[] sendData = System.Text.Encoding.Default.GetBytes("12345678");
        UdpSendMsg msg = new UdpSendMsg((ushort)NetWorkEvent.UdpSendMsg, "127,0,0,1", sendData, 18181);
    }

    public void UdpRecvDelegate(byte[] pbuff, int dwCount, string tmpIp, ushort port)
    {

    }
}
