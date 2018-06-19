using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

public class LuaAssetBundleLoadTest : MonoBehaviour
{
	public Button bt;


    void Start()
    {

        string file = Application.dataPath + "/Lua/Asset/LAssetBundleLoad";

		//记住在游戏初始化的时候要挂上LuaLuaClient脚本,然后再使用这个方法来调用lua逻辑
		LuaClient.GetMainState ().DoFile (file);

		LuaFunction fun = LuaClient.GetMainState ().GetFunction("LAssetBundleLoader.Awake");

        fun.Call();

		bt.onClick.AddListener (OnButtonClick);
    }

	void OnButtonClick()
	{
		LuaFunction fun = LuaClient.GetMainState ().GetFunction("LAssetBundleLoader.TestAssetBundle");

		if (fun != null)
			fun.Call ();
	}
}
