﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class MsgCenterWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(MsgCenter), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("SendToMessageByLua", SendToMessageByLua);
		L.RegFunction("SendToMessage", SendToMessage);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("instance", get_instance, set_instance);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendToMessageByLua(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MsgBase arg0 = (MsgBase)ToLua.CheckObject<MsgBase>(L, 1);
			MsgCenter.SendToMessageByLua(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendToMessage(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MsgCenter obj = (MsgCenter)ToLua.CheckObject<MsgCenter>(L, 1);
			MsgBase arg0 = (MsgBase)ToLua.CheckObject<MsgBase>(L, 2);
			obj.SendToMessage(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_instance(IntPtr L)
	{
		try
		{
			ToLua.Push(L, MsgCenter.instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_instance(IntPtr L)
	{
		try
		{
			MsgCenter arg0 = (MsgCenter)ToLua.CheckObject<MsgCenter>(L, 2);
			MsgCenter.instance = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
