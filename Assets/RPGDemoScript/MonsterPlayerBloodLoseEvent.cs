using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPlayerBloodLoseEvent : AnimEventBase
{
    private Action doEvent;
    public override void Init(float beginTime, float doTime, Action DoEvent = null)
    {
        StartTime = beginTime;
        DoTime = doTime;
        doEvent = DoEvent;
    }

    public override bool Execute(float dt)
    {
        timeBuffer += dt;

        if (timeBuffer >= StartTime)
        {
            if (timeBuffer >= DoTime)
            {
                timeBuffer = 0.0f;

                if (doEvent != null)
                {
                    doEvent();
                    return true;
                }
            }
        }
        return false;
    }
}
