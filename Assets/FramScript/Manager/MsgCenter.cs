using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgCenter : MonoBehaviour 
{
    public static MsgCenter instance;

	//整个游戏启动时第一阶梯需要加载和启动项
    void Awake()
    {
        instance = this;

        gameObject.AddComponent<UIManager>();
        gameObject.AddComponent<NPCManager>();
        gameObject.AddComponent<ParticleManager>();
        gameObject.AddComponent<NetWorkManager>();
        gameObject.AddComponent<GameManager>();
        gameObject.AddComponent<AssetManager>();
        gameObject.AddComponent<CharactorManager>();
		gameObject.AddComponent<LuaEventProcess> ();//这个是再封装一层来处理要发给lua的消息的,为什么不直接使用LuaAndCMsgCenter,是因为LuaAndCMsgCenter需要包含using LuaInterface;
        //但是MsgCenter是游戏启动时第一个就要执行的类,这个时候LuaInterface还没有加载进来,所以会报错,就再封装一层,再MainOther类中再将LuaAndCMsgCenter加载进去
    }

    public static void SendToMessageByLua(MsgBase tmpMsg)
    {
        instance.SendToMessage(tmpMsg);
    }

    public void SendToMessage(MsgBase tmpMsg)
    {
        MsgManager curManager = tmpMsg.GetMsgManager();

        //要发送给Lua的消息,GameManager时C#消息类型的最小的编号,小于它的话就是发送给lua要处理的消息,一定是LuaAndCMsgCenter加载之后才能使用
        if (tmpMsg.GetMsgManager() < MsgManager.GameManager) 
		{
			LuaEventProcess.instance.ProcessEvent (tmpMsg);
		} 
		else 
		{
			switch (curManager)
			{
			case MsgManager.AssetManager:
				AssetManager.instance.SendMessage(tmpMsg);
				break;

			case MsgManager.GameManager:

				break;

			case MsgManager.NetWorkManager:
				NetWorkManager.instance.SendMessage(tmpMsg);
				break;

			case MsgManager.NPCManager:
				NPCManager.instance.SendMessage(tmpMsg);
				break;

			case MsgManager.ParticleManager:

				break;

			case MsgManager.CharactorManager:
				CharactorManager.instance.SendMessage(tmpMsg);
				break;

			case MsgManager.UIManager:
				UIManager.instance.SendMessage(tmpMsg);
				break;

			default:
				break;
			}
		}

        
    }
}
