using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Debuger
{
    public static bool isEnable = true;

    private static UDPSocket udpSocket;

    public static UDPSocket UdpSocket
    {
        get
        {
            if (udpSocket == null)
                udpSocket = new UDPSocket(1024, null, 18001);//因为我们是看log,所以只需要向电脑端发送log消息,不需要接收消息,所以第二个参数为null

            return udpSocket;
        }
    }

    //如果是在iPhone和Android平台,我们可以通过udp向电脑发送debug信息,直接在电脑上看log,就不需要连接eclipse或者xcode看log了
    public static void Log(string message)
    {
        if (isEnable)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor
                 || Application.platform == RuntimePlatform.OSXEditor)
            {
                Debug.Log(message);
            }
            else
            {
                byte[] sendData = System.Text.Encoding.Default.GetBytes(message);
                //这里的第二个参数是电脑的ip,可以填"255,255,255,255",这样只要局域网里有端口号为18001的电脑都能接收我们的真机发过来的log
                UdpSocket.SendData(sendData, "", 18001);
            }
        }
    }
}
