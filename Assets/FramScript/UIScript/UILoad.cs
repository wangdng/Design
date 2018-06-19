using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//继承UIBase挂在Panel上来处理Panel下的子物体的各种事件(包括所有子物体之间的交互(对内部)和与其他Panel之间的交互(对外部))
public class UILoad : UIBase 
{
    public Light MainLight;
    //处理我这个Panel中所有注册的事件
    public override void ProcessEvent(MsgBase msgbase)
    {
        GameObject image = UIManager.instance.GetGameObject(this,"Image");
        switch (msgbase.MsgId)
        {
            case (ushort)UIEvent.OpenLight:
                image.GetComponent<Image>().color = Color.red;
                Debug.Log("Open Light");
                break;

            case (ushort)UIEvent.CloseLight:
                image.GetComponent<Image>().color = Color.green;
                Debug.Log("Close Light");
                break;

            case (ushort)UIEvent.SendUIPos:
                image.GetComponent<Image>().color = Color.black;
                Debug.Log(transform.position);
                break;
        }
    }

    //这里不需要担心UIManager会晚初始化,因为在脚本的顺序中已经把MsgCenter这个脚本(挂在了主相机上)的执行时间设置为了-300(一定是最先执行),在这个脚本的Awake里完成了各个Manager的初始化
    void Awake()
    {
        //生成我这个Panel的逻辑中所需要关心的所有事件id,包括这个Panel下所有子物体的交互和对外部的交互(如点击一个按钮NPC走动事件)
        msgIds = new ushort[] 
        {
            (ushort)UIEvent.OpenLight,
            (ushort)UIEvent.CloseLight,
            (ushort)UIEvent.SendUIPos,
        };

        //注册事件
        RegisterSelf(this, msgIds);
    }

	void Start () 
    {
        AddButtonListener("LightOn", TurnLightOn);

        AddButtonListener("LightOff", TurnLightOff);

        AddButtonListener("SendUIPos", OnSendUIPos);
	}

    void TurnLightOn()
    {
        MainLight.enabled = true;
        //UIManager.instance.SendMessage(new MsgBase(msgIds[0]));

        //TcpConnectMsg msg = new TcpConnectMsg((ushort)NetWorkEvent.TcpConnect, "127.0.0.1", 18010);

        //SendMessage(msg);

        UIManager.instance.SendMessage(new MsgBase(msgIds[0]));
    }

    void TurnLightOff()
    {
        MainLight.enabled = false;
        UIManager.instance.SendMessage(new MsgBase(msgIds[1]));
    }

    void OnSendUIPos()
    {
        MsgTransform transMsg = new MsgTransform(msgIds[2], transform);

        UIManager.instance.SendMessage(transMsg);
    }


    //判断敌人在玩家的左边还是右边
    public bool JudeRight(Transform Player,Transform Enemy)
    {
        Vector3 EP = Enemy.position - Player.position;

        //Vector3.Cross的两个参数是有序的,如果调换一下,那么下边的mul.y < 0 的时候敌人在玩家右边
        Vector3 mul = Vector3.Cross(Player.forward, EP);

        //Unity的坐标轴是左手坐标系,根据左手坐标系原理可以得知此时敌人在玩家的右边
        if (mul.y > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
	
	
	void Update () 
    {
		
	}
}
