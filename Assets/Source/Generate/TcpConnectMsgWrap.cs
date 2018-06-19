﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class TcpConnectMsgWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(TcpConnectMsg), typeof(MsgBase));
		L.RegFunction("New", _CreateTcpConnectMsg);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("ip", get_ip, set_ip);
		L.RegVar("port", get_port, set_port);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateTcpConnectMsg(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				ushort arg0 = (ushort)LuaDLL.luaL_checknumber(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				ushort arg2 = (ushort)LuaDLL.luaL_checknumber(L, 3);
				TcpConnectMsg obj = new TcpConnectMsg(arg0, arg1, arg2);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: TcpConnectMsg.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ip(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			TcpConnectMsg obj = (TcpConnectMsg)o;
			string ret = obj.ip;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index ip on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_port(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			TcpConnectMsg obj = (TcpConnectMsg)o;
			ushort ret = obj.port;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index port on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ip(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			TcpConnectMsg obj = (TcpConnectMsg)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.ip = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index ip on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_port(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			TcpConnectMsg obj = (TcpConnectMsg)o;
			ushort arg0 = (ushort)LuaDLL.luaL_checknumber(L, 2);
			obj.port = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index port on a nil value");
		}
	}
}
