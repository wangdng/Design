using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventNode
{
    public MonoBase mono;

    public EventNode next;

    public EventNode(MonoBase data)
    {
        mono = data;

        next = null;
    }
}

public class ManagerBase : MonoBase
{
    public Dictionary<ushort, EventNode> EventTree = new Dictionary<ushort, EventNode>();

    public override void ProcessEvent(MsgBase tmpMsg)
    {
        if (!EventTree.ContainsKey(tmpMsg.MsgId))
        {
            Debug.LogError("这个消息还没有注册:" + tmpMsg.MsgId);
            Debug.LogError("this msgid is belong:" + tmpMsg.GetMsgManager());
        }
        else
        {
            //如果包含就取出该消息对应的脚本链表
            EventNode eventNode = EventTree[tmpMsg.MsgId];

            //然后通知链表中的所有脚本
            do //do while 至少会执行一次循环
            {
                eventNode.mono.ProcessEvent(tmpMsg);
                eventNode = eventNode.next;
            } 
            while (eventNode != null);
        }
    }

    //注册一个脚本对应多个消息,将一个脚本和多个消息对应起来
    public void RegisterMsg(MonoBase mono,params ushort[] msgs)
    {
        for (int i = 0; i < msgs.Length; i++)
        {
            //每次循环生成一个相同的脚本EventNode
            EventNode newNode = new EventNode(mono);

            //将不同的消息和这个相同的脚本EventNode对应起来
            RegisterMsg(msgs[i], newNode);
        }
    }

    //通过一个消息注册一个或者多个脚本(EventNode是一个链表结构,多个脚本就存放在这个链表中),一个消息和多个脚本对应起来
    public void RegisterMsg(ushort msgId,EventNode node)
    {
        //这种情况是该消息还没有注册,那么直接将脚本EventNode添加到EventTrees中
        if (!EventTree.ContainsKey(msgId))
        {
            EventTree.Add(msgId, node);
        }
        else//这种情况是该消息已经存在,则将该脚本EventNode添加到和这个消息对应的EventNode的后面的节点(next)上
        {
            EventNode curNode = EventTree[msgId];

            //循环找到最后一个节点
            while (curNode.next != null)
            {
                curNode = curNode.next;
            }

            //将该脚本EventNode挂在最后一个节点的后面
            curNode = node;
        }
    }

    //根据消息数组循环删除某个消息对应的脚本链表中的传人的脚本(参数1)
    public void UnRegisterMsg(MonoBase mono, params ushort[] msgs)
    {
        for (int i = 0; i < msgs.Length; i++)
        {
            UnRegisterMsg(msgs[i], mono);
        }
    }

    public void UnRegisterMsg(ushort msgId, MonoBase mono)
    {
        if (!EventTree.ContainsKey(msgId))
        {
            Debug.LogError("EventTree cant have msgId:" + msgId);
        }
        else
        {
            EventNode tmp = EventTree[msgId];

            //这种情况说明传进来的脚本(参数2)正好位于该消息(参数1)对应的所有脚本所形成的链表的头部(头)
            if (tmp.mono == mono)
            {
                EventNode head = tmp;

                if (head.next != null)//这种情况是head后面还有节点
                {
                    head.mono = head.next.mono;
                    head.next = head.next.next;
                }
                else//这种情况是head后面没有有节点
                {
                    EventTree.Remove(msgId);
                }
            }
            else//这种情况说明传进来的脚本(参数2)不是该消息对应的第一个脚本
            {
                //第一种情况传进来的脚本正好是该消息对应的脚本链表的最后一个
                while (tmp.next != null && tmp.next.mono != mono)
                {
                    tmp = tmp.next;
                }

                if (tmp.next == null)
                {
                    Debug.LogError("该链表中没有找到传入的脚本");
                    return;
                }

                if (tmp.next.next != null)//该脚本中间的情况
                {
                    tmp.next = tmp.next.next;
                }
                else//该脚本在末尾
                {
                    tmp.next = null;
                }
            }
        }
    }
}
