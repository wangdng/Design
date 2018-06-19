using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using System.IO;
using UnityEngine.UI;

public class LuaUIBehaviour : MonoBehaviour
{
	LuaFunction luaFunc;

	void Awake()
	{
		CallMethodByGameObject ("LUIManager.RegisterGameObject", this.gameObject);
	}

    private void CallMethodByGameObject(string method,GameObject go)
    {
		luaFunc = LuaClient.GetMainState ().GetFunction (method);

		luaFunc.Call(go);
    }

	public void AddButtonListener(LuaFunction action)
	{
		if (action != null)
		{
			Button btn = transform.GetComponent<Button>();

			if (btn != null)
				btn.onClick.AddListener (delegate() {
					action.Call(gameObject);
				});
		}
	}


	public void AddSliderListener(LuaFunction action)
	{
		if (action != null)
		{
			Slider slider = transform.GetComponent<Slider>();

			if (slider != null)
				slider.onValueChanged.AddListener(delegate(float tempFloat) 
				{
					action.Call(gameObject,tempFloat);
				});
		}
	}
		

	public void AddInputListener(LuaFunction action)
	{
		if (action != null)
		{
			InputField input = transform.GetComponent<InputField>();

			if (input != null)
				input.onValueChanged.AddListener(delegate(string tmpStr) 
				{
					action.Call(gameObject,tmpStr);
				});
		}
	}

	public void AddToggleListener(LuaFunction action)
	{
		if (action != null)
		{
			Toggle toggle = transform.GetComponent<Toggle>();

			if (toggle != null)
				toggle.onValueChanged.AddListener(delegate(bool isActive) 
				{
					action.Call(gameObject,isActive);
				});
		}
	}
}
