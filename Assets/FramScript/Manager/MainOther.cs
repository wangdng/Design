using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainOther : MonoBehaviour
{
	//这里时第二阶梯需要启动和加载的类
	void Awake()
	{
		
		LuaAndCMsgCenter luaCenter = gameObject.AddComponent<LuaAndCMsgCenter> ();

		LuaEventProcess.instance.SetChild (luaCenter);

	}
}
