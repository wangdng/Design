using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackEndEvent : AnimEventBase
{
    private Action doEvent;
    public override void Init(float beginTime, float doTime, Action Doevent = null)
    {
        StartTime = beginTime;
        DoTime = doTime;
        doEvent = Doevent;
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
