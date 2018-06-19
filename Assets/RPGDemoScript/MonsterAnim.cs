using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个脚本挂在某个Gameobject上则这个Gameobject一定会挂上Animator组件,没有挂的话会自带挂上
[RequireComponent(typeof(Animator))]
public class MonsterAnim : CharactorBase
{
    public override void ProcessEvent(MsgBase msgbase)
    {
        switch (msgbase.MsgId)
        {
            case (ushort)CharactorAnimEvent.init:
                {
           
                }
                break;

            case (ushort)CharactorAnimEvent.Idle:
                {
                    animator.SetInteger("Index", 0);
                }
                break;

            case (ushort)CharactorAnimEvent.Run:
                {
                    RunMsg msg = (RunMsg)msgbase;
                    animator.SetInteger("Index", 1);
                    animator.SetFloat("Speed", msg.GetValue());
                }
                break;

            case (ushort)CharactorAnimEvent.Attack:
                {
                    if (animator.GetInteger("Index") != 0)
                        return;

                    animator.SetInteger("Index", 2);
                }
                break;

            case (ushort)CharactorAnimEvent.BigAttack:
                {
                    if (animator.GetInteger("Index") != 0)
                        return;

                    animator.SetInteger("Index", 3);
                }
                break;

            case (ushort)CharactorAnimEvent.Die:
                {
                    animator.SetInteger("Index", 4);
                    break;
                }

            case (ushort)CharactorAnimEvent.SmallAttack:
                {
                    if (animator.GetInteger("Index") != 0)
                        return;

                    animator.SetInteger("Index", 5);
                    break;
                }
 
            case (ushort)CharactorAnimEvent.SpecialAttack:
                {
                    if (animator.GetInteger("Index") != 0)
                        return;

                    animator.SetInteger("Index", 6);
                    break;
                }
        }
    }

    private Animator animator;

    private void Awake()
    {
        msgIds = new ushort[]
            {
                 (ushort)CharactorAnimEvent.init,
                 (ushort)CharactorAnimEvent.Idle,
                 (ushort)CharactorAnimEvent.Attack,
                 (ushort)CharactorAnimEvent.Run,
                 (ushort)CharactorAnimEvent.BigAttack,
                 (ushort)CharactorAnimEvent.Die,
                 (ushort)CharactorAnimEvent.SmallAttack,
                 (ushort)CharactorAnimEvent.SpecialAttack,
            };

        RegisterSelf(this, msgIds);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
