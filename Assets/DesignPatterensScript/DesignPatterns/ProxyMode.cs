using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PlayAnimationEvent();

//代理模式
//代理播放动画和处理动画事件的类
public class DelegatePlayAnim
{
    Animation anim;
    PlayAnimationEvent playanimEvent;

    public DelegatePlayAnim(PlayAnimationEvent AnimEvent)
    {
        playanimEvent = AnimEvent;
    }

    public void PlayAnim()
    {
        timeCount = 0;
        anim.Play();
    }

    bool isFinish()
    {
        return anim.isPlaying == true ? false : true;
    }

    float timeCount = 0;

    void Update()
    {
        if (!isFinish())
        {
            timeCount += Time.deltaTime;

            if (timeCount >= 0.5)
            {
                timeCount = 0;
                //处理播放动画事件
                playanimEvent();
            }
        }
    }
}

public class ProxyMode : MonoBehaviour 
{
    DelegatePlayAnim[] delegateAnims = new DelegatePlayAnim[10];

    public void PlayAnimEvent()
    {
        //动画事件回调
    }


	void Start () 
    {
        //只要这里使用代理动画类,那么动画事件回调就会调用我们的PlayAnimEvent方法
        DelegatePlayAnim delegateAnim = new DelegatePlayAnim(PlayAnimEvent);

        delegateAnim.PlayAnim();

        //例如实现多个动画回调事件也很简单,这样就实现了动画的播放层和调用层分开,耦合性降低
        for (int i = 0; i < 10; i++)
        {
            //这里我们每个动画的动画事件回调都会调用到我们的PlayAnimEvent
            delegateAnims[i] = new DelegatePlayAnim(PlayAnimEvent);
            delegateAnims[i].PlayAnim();
        }
	}
	
	void Update () 
    {
		
	}
}
