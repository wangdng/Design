using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LuaInterface;

public class LuaUITest : MonoBehaviour 
{
	void Start () 
	{
		LuaFunction luc = LuaClient.GetMainState ().GetFunction("LUILoad.Start");

		luc.Call ();
	}
}
