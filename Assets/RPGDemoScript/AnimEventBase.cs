using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public abstract class AnimEventBase
{
    protected float timeBuffer = 0.0f;

    //开始计时
    protected float StartTime = 0.0f;

    //到了做某些动作的时候了(如播放某个动作或者播放特效)
    protected float DoTime = 0.0f;

    public abstract void Init(float beginTime,float doTime,Action doEvent = null);

    public abstract bool Execute(float dt);
}
