using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class NPCAnimCtrl : NPCBase
{
    private Animator anim;

    public override void ProcessEvent(MsgBase msgbase)
    {
        switch(msgbase.MsgId)
        {
            case (ushort)NPCAnimEvent.NPCIdle:
                {
                    if(anim.GetInteger("Index") != 0)
                        anim.SetInteger("Index", 0);
                }
                break;
            case (ushort)NPCAnimEvent.NPCRun:
                {
                    if (anim.GetInteger("Index") != 1)
                        anim.SetInteger("Index", 1);

                    MsgBase CalcMoveMsg = new MsgBase((ushort)NPCLogicEvent.CalcMove);

                    SendMessage(CalcMoveMsg);
                }
                break;
            case (ushort)NPCAnimEvent.NPCAttack:
                {
                    if (anim.GetInteger("Index") != 2)
                        anim.SetInteger("Index", 2);
                }
                break;
            case (ushort)NPCAnimEvent.NPCdie:
                {
                    if (anim.GetInteger("Index") != 3)
                        anim.SetInteger("Index", 3);
                }
                break;
        }
    }

    private void Awake()
    {
        msgIds = new ushort[]
        {
            (ushort)NPCAnimEvent.NPCdie,
            (ushort)NPCAnimEvent.NPCRun,
            (ushort)NPCAnimEvent.NPCAttack,
            (ushort)NPCAnimEvent.NPCIdle
        };

        RegisterSelf(this, msgIds);

        anim = GetComponent<Animator>();
    }

}
