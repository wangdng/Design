using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

//这个通用脚本挂在继承于monoBase的GameObject下的子物体上,它只是向UIManager注册Gameobject和提供一些控件的交互事件
public class UIBehaviour : MonoBehaviour 
{
    //这里不需要担心UIManager会晚初始化,因为在脚本的顺序中已经把MsgCenter(挂在了主相机上)这个脚本的执行时间设置为了-300(一定是最先执行),在这个脚本的Awake里完成了各个Manager的初始化
	void Awake () 
    {
        //把控件注册到UIManager
        UIBase mb = FindParentMonoBase(transform);

        if (mb == null)
        {
            Debug.LogError("UIBehaviour 以上的节点中应该含有继承于MonoBase的节点,否则不正确");
            return;
        }
        UIManager.instance.RegisterGameObject(mb, name, gameObject);
	}

    UIBase FindParentMonoBase(Transform tf)
    {
        if (tf.parent != null)
        {
            UIBase mb = tf.parent.GetComponent<UIBase>();

            if (mb == null)
                mb = FindParentMonoBase(tf.parent);

            return mb;
        }

        return null;
    }

    //添加按钮点击事件
    public void AddButtonListener(UnityAction action)
    {
        if (action != null)
        {
            Button btn = transform.GetComponent<Button>();

            if (btn != null)
                btn.onClick.AddListener(action);
        }
    }

    //注销按钮点击事件
    public void RemoveButtonListener(UnityAction action)
    {
        if (action != null)
        {
            Button btn = transform.GetComponent<Button>();

            if (btn != null)
                btn.onClick.RemoveListener(action);
        }
    }

    //添加滑动条滑动事件
    public void AddSliderListener(UnityAction<float> action)
    {
        if (action != null)
        {
            Slider slider = transform.GetComponent<Slider>();

            if (slider != null)
                slider.onValueChanged.AddListener(action);
        }
    }

    //注销滑动条滑动事件
    public void RemoveSliderListener(UnityAction<float> action)
    {
        if (action != null)
        {
            Slider slider = transform.GetComponent<Slider>();

            if (slider != null)
                slider.onValueChanged.RemoveListener(action);
        }
    }

    //添加输入事件
    public void AddInputListener(UnityAction<string> action)
    {
        if (action != null)
        {
            InputField input = transform.GetComponent<InputField>();

            if (input != null)
                input.onValueChanged.AddListener(action);
        }
    }

    //注销输入事件
    public void RemoveInputListener(UnityAction<string> action)
    {
        if (action != null)
        {
            InputField input = transform.GetComponent<InputField>();

            if (input != null)
                input.onValueChanged.RemoveListener(action);
        }
    }

    //添加Toggle事件
    public void AddToggleListener(UnityAction<bool> action)
    {
        if (action != null)
        {
            Toggle toggle = transform.GetComponent<Toggle>();

            if (toggle != null)
                toggle.onValueChanged.AddListener(action);
        }
    }

    //注销Toggle事件
    public void RemoveToggleListener(UnityAction<bool> action)
    {
        if (action != null)
        {
            Toggle toggle = transform.GetComponent<Toggle>();

            if (toggle != null)
                toggle.onValueChanged.RemoveListener(action);
        }
    }
}
