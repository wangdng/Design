using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public enum NPCState
{
    Idle,
    Attack,
    Run,
    Die
}

[RequireComponent (typeof(NavMeshAgent))]
public class NPCLogicCtrl : NPCBase
{
    NavMeshAgent agent;

    NPCState npcState;

    Transform target;

    MsgBase npcBehaiveMsg;

    public float speed = 0.0f;

    Animator anim;

    MsgFloat tmpMsg;

    private AnimEventManager eventManager ;

    public override void ProcessEvent(MsgBase msgbase)
    {
        switch (msgbase.MsgId)
        {
            case (ushort)NPCLogicEvent.CalcMove:
                {
                    CalcMove();
                }
                break;
            case (ushort)NPCLogicEvent.PlayAttack:
                {
                    npcBehaiveMsg.ChangeMsgId((ushort)NPCAnimEvent.NPCAttack);

                    SendMessage(npcBehaiveMsg);
                }
                break;
            case (ushort)NPCLogicEvent.PlayAttackEnd:
                {
                    //当怪物的攻击结束后重新每帧进行检测是否是移动还是攻击
                    npcState = NPCState.Run;
                }
                break;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        target = GameObject.FindWithTag("Player").transform;

        npcBehaiveMsg = new MsgBase();

        npcState = NPCState.Run;

        msgIds = new ushort[]
        {
            (ushort)NPCLogicEvent.CalcMove,
             (ushort)NPCLogicEvent.PlayAttack,
             (ushort)NPCLogicEvent.PlayAttackEnd
        };

        RegisterSelf(this, msgIds);

        anim = GetComponent<Animator>();

        tmpMsg = new MsgFloat();

        eventManager = new AnimEventManager();
    }

    private void Move()
    {
        Vector3 tmpDis = target.position - transform.position;

        if (tmpDis.magnitude > 3.1)
        {
            npcBehaiveMsg.ChangeMsgId((ushort)NPCAnimEvent.NPCRun);

            SendMessage(npcBehaiveMsg);

            npcState = NPCState.Run;
        }
        else
        {
            if (npcState == NPCState.Run)
            {
                Dictionary<string, AnimEventBase> npcAttackEvents = CreateNPCAttackEvent();

                eventManager.RegisterEvents(npcAttackEvents);

                npcState = NPCState.Attack;
            }
        }
    }

    Dictionary<string, AnimEventBase> CreateNPCAttackEvent()
    {
        EnemyAttackEvent npcAttackEvent = new EnemyAttackEvent();
        npcAttackEvent.Init(0, 0, OnNPCAttackEvent);

        MonsterPlayerBloodLoseEvent playerReduceBloodEvent = new MonsterPlayerBloodLoseEvent();
        playerReduceBloodEvent.Init(0, 0.7f, OnPlayerReduceBloodEvent);

        EnemyAttackEndEvent npcAttackendEvent = new EnemyAttackEndEvent();
        npcAttackendEvent.Init(0, 2.333f, OnNPCAttackEndEvent);

        Dictionary<string, AnimEventBase> tmpEvents = new Dictionary<string, AnimEventBase>();
        tmpEvents.Add("NPCAttack", npcAttackEvent);
        tmpEvents.Add("PlayerReduceBlood", playerReduceBloodEvent);
        tmpEvents.Add("NPCAttackEnd", npcAttackendEvent);

        return tmpEvents;
    }

    void OnPlayerReduceBloodEvent()
    {
        tmpMsg.ChangeMsgId((ushort)CharactorDataEvent.ReduceBlood);
        tmpMsg.SetValue(10);

        SendMessage(tmpMsg);
    }

    void OnNPCAttackEvent()
    {
        npcBehaiveMsg.ChangeMsgId((ushort)NPCLogicEvent.PlayAttack);

        SendMessage(npcBehaiveMsg);

        transform.LookAt(target);

        npcState = NPCState.Attack;
    }

    void OnNPCAttackEndEvent()
    {
        npcBehaiveMsg.ChangeMsgId((ushort)NPCLogicEvent.PlayAttackEnd);

        SendMessage(npcBehaiveMsg);
    }

    void CalcMove()
    {
        //只有动画为待机或者跑的时候才能进行位移计算,否则回出现滑步的现象
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            Vector3 tmpDis = target.position - transform.position;

            Vector3 tmpWalk = tmpDis - tmpDis.normalized * speed;

            transform.LookAt(target);

            agent.destination = tmpWalk + transform.position;
        }
    }

    private void LateUpdate()
    {
        eventManager.OnUpdate();
    }

    private void Update()
    {
        Move();

        if (npcState == NPCState.Run)
        {
            
        }
        else if(npcState == NPCState.Attack)
        {
            
        }
        else if(npcState == NPCState.Die)
        {

        }
        else if(npcState == NPCState.Idle)
        {

        }
    }
}
