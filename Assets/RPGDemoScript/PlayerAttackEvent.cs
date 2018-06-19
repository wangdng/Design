using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttackEvent : AnimEventBase {

    private Action DoEvent;

    public override void Init(float startTime, float doTime, Action doEvent)
    {
        StartTime = startTime;
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
