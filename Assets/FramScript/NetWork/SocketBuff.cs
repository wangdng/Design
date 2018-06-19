using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public delegate void CallBackRecvMsg(byte[] tmpByte);

public class SocketBuff 
{
    private byte[] HeadByte;

    //这个时包头长度,它包括消息体的长度(不包含包头的长度)和msgId的长度,消息体的长度为4,msgId的长度为2
    private int HeadLength;

    private byte[] allRecvData;//总接受到的数据

    private int alreadyRecvLength;//已经接受到的数据长度

    private int allRecvLength;//总共接受到的数据长度

    CallBackRecvMsg callBackRecvMsg;

    public SocketBuff(int tmpHedyLength, CallBackRecvMsg callback)
    {
        HeadLength = tmpHedyLength;

        HeadByte = new byte[HeadLength];

        callBackRecvMsg = callback;
    }

    public void RecvByte(byte[] recvData,int realLength)
    {
        if (realLength == 0)
            return;

        //目前接受到的数据长度小于一个头的长度
        if(alreadyRecvLength < HeadByte.Length)
        {
            RecvHead(recvData, realLength);
        }
        else//目前接受到的数据长度>=一个头的长度了
        {
            int allDataLength = alreadyRecvLength + realLength;//realLength是一次发过来的消息拆开包头长度后剩下的长度,alreadyRecvLength时包头长度

            //检测当前接受到的数据是否和我们规定的一条消息总长度相同
            if (allDataLength == allRecvLength)
            {
                RecvOneAll(recvData, realLength);
            }
            else if(allDataLength > allRecvLength)//大于我们规定的一条消息的总长度
            {
                RecvLarger(recvData, realLength);
            }
            else
            {
                RecvSmall(recvData, realLength);//小于我们规定的一条消息的总长度
            }
        }
    }

    public void RecvLarger(byte[] recvData, int realLength)
    {
        int tmpLength = allRecvLength - alreadyRecvLength;

        Buffer.BlockCopy(recvData, 0, allRecvData, alreadyRecvLength, tmpLength);
        alreadyRecvLength += tmpLength;

        RecvOneMsgOver();

        int LeftDataLength = realLength - tmpLength;

        byte[] tmpBuffer = new byte[LeftDataLength];

        Buffer.BlockCopy(recvData, tmpLength, tmpBuffer, 0, LeftDataLength);

        //将多余的消息数据再次进行解析
        RecvByte(tmpBuffer, LeftDataLength);

    }

    public void RecvSmall(byte[] recvData, int realLength)
    {
        Buffer.BlockCopy(recvData, 0, allRecvData, alreadyRecvLength, realLength);
        alreadyRecvLength += realLength;
    }

    public void RecvOneAll(byte[] recvByte,int realLength)
    {
        //将除了头后剩下的消息数据拷贝到总的缓存区中
        Buffer.BlockCopy(recvByte, 0, allRecvData, alreadyRecvLength, realLength);

        alreadyRecvLength += realLength;
        RecvOneMsgOver();
    }

    //接受完成了一条完整的消息数据
    public void RecvOneMsgOver()
    {
        if(callBackRecvMsg != null)
        {
            callBackRecvMsg(allRecvData);
        }
        alreadyRecvLength = 0;
        allRecvLength = 0;
        allRecvData = null;
    }

    public void RecvHead(byte[] recvByte, int curlLength)
    {
        int tmpDis = HeadByte.Length - alreadyRecvLength;//已经接受到的数据长度还不够一个头的长度,还差多少个凑成一个头

        int tmpLength = alreadyRecvLength + curlLength;//已经接受到的数据和现在接受到数据长度的和

        //如果目前已经接受到的数据和当前接受到的数据和依然小于一个头的长度
        if(tmpLength < HeadByte.Length)
        {
            //就将所以数据都拷贝到头部缓存中
            Buffer.BlockCopy(recvByte, 0, HeadByte, alreadyRecvLength, curlLength);

            alreadyRecvLength += curlLength;
        }
        else//如果目前已经接受到的数据和当前接受到的数据和大于等于一个头的总长度
        {
            //将recvByte中tmpDis长度的数据拷贝到剩下的头部缓存中(目前已经接受的数据占到了alreadyRecvLength,而tmpDis是HeadByte.Length - alreadyRecvLength即是头部缓存中还剩下的空间大小)
            Buffer.BlockCopy(recvByte, 0, HeadByte, alreadyRecvLength, tmpDis);
            alreadyRecvLength += tmpDis;

            //得到总的数据长度
            //BitConverter.ToInt32(HeadByte,0)作用是从一个byte数组中从0位开始读取后面的4个字节的数据,并把它转换为int数据类型,这里正好获取了消息体的长度
            allRecvLength = BitConverter.ToInt32(HeadByte,0) + HeadLength;

            allRecvData = new byte[allRecvLength];

            //先将头部缓存区的数据全部拷贝到allRecvData中
            Buffer.BlockCopy(HeadByte, 0, allRecvData, 0, HeadByte.Length);

            int leftDataLength = curlLength - tmpDis;

            //此时一个包头接收完毕,剩下的接收包体
            if (leftDataLength > 0)
            {
                byte[] tmpLeft = new byte[leftDataLength];

                Buffer.BlockCopy(recvByte, tmpDis, tmpLeft, 0, leftDataLength);

                //接收包体
                RecvByte(tmpLeft, leftDataLength);
            }
            else
            {
                RecvOneMsgOver();
            }
        }
    }
}
