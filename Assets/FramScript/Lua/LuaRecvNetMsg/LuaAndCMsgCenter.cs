using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

//lua和C#消息中转站
public class LuaAndCMsgCenter : MonoBase 
{
	LuaFunction luaFuc;

	private static LuaAndCMsgCenter mIns;


	public static LuaAndCMsgCenter instacne
	{
		get{ return mIns;}
	}


	void Awake()
	{
		mIns = this;
	}

    //可以直接使用单例来调用,中转lua和C#的消息
    //游戏启动的时候lua中的LMsgCenter就会被加载,此时luaFuc就会被注册,所以就可以直接使用这个方法将消息发送到lua中
    public override void ProcessEvent(MsgBase msgbase)
	{
        //127表示有protobuff的数据要传送到lua中去,否则都是普通的消息
        //用一个固定的127来区分是不是向lua传递网络消息,如果不是127就表示向lua传递网络消息,因为lua不能直接接收C#的protobuff序列化或者反序列化之后的byte,需要使用LuaByteBuffer来转换一下
        if (msgbase.GetState () != 127) 
		{
			NetWorkMsg netMsg = (NetWorkMsg)msgbase;

			byte[] data = netMsg.GetBuff ();

			//这个就是将C#中的Protobuff转换给lua能直接使用的字节,C#中的Protobuff字节流不能直接给lua使用
			LuaByteBuffer buff = new LuaByteBuffer (data);

			//将网络消息回掉到lua中去
			luaFuc.Call (true, netMsg.MsgId, msgbase.GetState (), buff);
		} 
		else//不是网络消息
		{
			luaFuc.Call (false, msgbase);
		}
	}

	public void SetLuaCallBack(LuaFunction luac)
	{
		luaFuc = luac;
	}
}
