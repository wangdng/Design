﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class LuaUIBehaviourWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(LuaUIBehaviour), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("AddButtonListener", AddButtonListener);
		L.RegFunction("AddSliderListener", AddSliderListener);
		L.RegFunction("AddInputListener", AddInputListener);
		L.RegFunction("AddToggleListener", AddToggleListener);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddButtonListener(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaUIBehaviour obj = (LuaUIBehaviour)ToLua.CheckObject<LuaUIBehaviour>(L, 1);
			LuaFunction arg0 = ToLua.CheckLuaFunction(L, 2);
			obj.AddButtonListener(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddSliderListener(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaUIBehaviour obj = (LuaUIBehaviour)ToLua.CheckObject<LuaUIBehaviour>(L, 1);
			LuaFunction arg0 = ToLua.CheckLuaFunction(L, 2);
			obj.AddSliderListener(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddInputListener(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaUIBehaviour obj = (LuaUIBehaviour)ToLua.CheckObject<LuaUIBehaviour>(L, 1);
			LuaFunction arg0 = ToLua.CheckLuaFunction(L, 2);
			obj.AddInputListener(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddToggleListener(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaUIBehaviour obj = (LuaUIBehaviour)ToLua.CheckObject<LuaUIBehaviour>(L, 1);
			LuaFunction arg0 = ToLua.CheckLuaFunction(L, 2);
			obj.AddToggleListener(arg0);
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
}

