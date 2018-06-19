using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillEndEvent : AnimEventBase
{
    Action DoEvent;
    public override void Init(float beginTime, float doTime, Action doEvent = null)
    {
        StartTime = beginTime;
        DoTime = doTime;
        DoEvent = doEvent;
    }

    public override bool Execute(float dt)
    {
        timeBuffer += dt;

        if (timeBuffer >= StartTime)
        {
            if (timeBuffer >= DoTime)
            {
                timeBuffer = 0.0f;

                if (DoEvent != null)
                {
                    DoEvent();
                    return true;
                }
            }
        }
        return false;
    }
}
