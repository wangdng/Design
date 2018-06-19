using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//状态机
public abstract class FSMState
{
    public byte StateId;

    public virtual void OnBeforeEnter()
    {

    }

    public virtual void CopyPreStateData(FSMState preState)
    {

    }

    //这个实际上就是一个纯虚函数
    public abstract void OnEnter();

    public virtual void OnUpdate()
    {

    }

    public virtual void OnLeave()
    {

    }
}


public class FSMStateManager
{
    //已经存在的状态数量
    private byte curAdd;

    private byte curStateId;

    private FSMState[] fsmStates;

    public FSMStateManager(byte statenum)
    {
        curAdd = 0;
        curStateId = 0;
        fsmStates = new FSMState[statenum];
    }

    public void AddState(FSMState state)
    {
        if (curAdd < fsmStates.Length)
        {
            fsmStates[curAdd] = state;

            curAdd++;
        }
    }

    public void ChangeState(byte stateId)
    {
        FSMState PreState = fsmStates[curStateId];
        PreState.OnLeave();

        curStateId = stateId;

        fsmStates[curStateId].OnBeforeEnter();
        fsmStates[curStateId].CopyPreStateData(PreState);
        fsmStates[curStateId].OnEnter();
    }

    public void Update()
    {
        fsmStates[curStateId].OnUpdate();
    }
}


public class WalkState:FSMState
{
    //abstract抽象方法是必须要重写的,不用abstract修饰的方法不要求重写
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {

    }

    public override void OnLeave()
    {

    }
}


public class AttackState : FSMState
{
    //abstract抽象方法是必须要重写的,不用abstract修饰的方法不要求重写
    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void OnLeave()
    {

    }
}


public class IdleState : FSMState
{
    //abstract抽象方法是必须要重写的,不用abstract修饰的方法不要求重写
    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void OnLeave()
    {

    }
}

public class FSM : MonoBehaviour 
{
    private FSMStateManager FsmManager;


    public enum FSMStateID
    {
        Idle,
        Walk,
        Attack,
        MaxValue
    }

	void Start () 
    {
        FsmManager = new FSMStateManager((byte)FSMStateID.MaxValue);

        WalkState walkState = new WalkState();

        IdleState idleState = new IdleState();

        FsmManager.AddState(walkState);
        FsmManager.AddState(idleState);
	}


    public void PlayAttack()
    {
        FsmManager.ChangeState((byte)FSMStateID.Attack);
    }
	
	
	void Update ()
    {
        FsmManager.Update();
	}
}
