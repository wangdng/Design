using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuaUtil
{
    public static void ShowText(GameObject go,string text)
    {
		go.GetComponent<Text> ().text = text;
    }
}
