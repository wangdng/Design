﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class NetWorkMsgWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(NetWorkMsg), typeof(MsgBase));
		L.RegFunction("GetBuff", GetBuff);
		L.RegFunction("SetBuff", SetBuff);
		L.RegFunction("New", _CreateNetWorkMsg);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateNetWorkMsg(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				NetWorkMsg obj = new NetWorkMsg();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else if (count == 1)
			{
				byte[] arg0 = ToLua.CheckByteBuffer(L, 1);
				NetWorkMsg obj = new NetWorkMsg(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: NetWorkMsg.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetBuff(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			NetWorkMsg obj = (NetWorkMsg)ToLua.CheckObject<NetWorkMsg>(L, 1);
			byte[] o = obj.GetBuff();
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetBuff(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			NetWorkMsg obj = (NetWorkMsg)ToLua.CheckObject<NetWorkMsg>(L, 1);
			byte[] arg0 = ToLua.CheckByteBuffer(L, 2);
			obj.SetBuff(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
