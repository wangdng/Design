using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaEventProcess : MonoBase 
{
	MonoBase child;

	static LuaEventProcess mIns;

	public static LuaEventProcess instance
	{
		get
		{
			return mIns;
		}
	}


	public override void ProcessEvent (MsgBase msgbase)
	{
		if (child != null)
			child.ProcessEvent (msgbase);
	}

	void Awake()
	{
		mIns = this;
	}

	public void SetChild(MonoBase cd)
	{
		child = cd;
	}
}
