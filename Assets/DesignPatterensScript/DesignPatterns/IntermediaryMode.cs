using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorBase:MonoBehaviour
{
    public virtual void ReduceBlood()
    {

    }

    public virtual void playAttack()
    {

    }
}


public class ActorPlayer:ActorBase
{
    public override void ReduceBlood()
    {

    }

    public override void playAttack()
    {

    }
}

public class ActorNpc : ActorBase
{
    public override void ReduceBlood()
    {

    }

    public override void playAttack()
    {

    }
}


//中介,用中介提供的计算伤害产生条件的方法来计算两个物体之间是否会产出伤害
public class CaculateDistance
{
    public static bool NeedReduceBlood(ActorBase player,ActorBase Npc)
    {
        if (Vector3.Distance(player.transform.position, Npc.transform.position) < 10)
            return true;

        return false;
    }

    //计算攻击者是否能够攻击被攻击者(条件是被攻击者是否在攻击者的左右3米且前方5米的范围)
    public static bool isCanAttack(ActorBase Attacker, ActorBase BeAttacker)
    {
        //成立条件数量
        byte conditionNum = 0;

        //从攻击者到被攻击者的向量
        Vector3 AtoB = BeAttacker.transform.position - Attacker.transform.position;

        //计算攻击者到被攻击者的向量(单位化后)和攻击者右方向量(单位化后)的点积
        float rightAngle = Vector3.Dot(AtoB.normalized, Attacker.transform.right.normalized);

        //说明被攻击和攻击者右方的单位向量的夹角小于90度
        if (rightAngle > 0)
        {
            float RightDistance = AtoB.sqrMagnitude * rightAngle;

            if (RightDistance < 3)
            {
                conditionNum++;
            }
        }


        //transform没有left，直接取负即是左
        float leftAngle = Vector3.Dot(AtoB.normalized, -Attacker.transform.right.normalized);

        //说明被攻击和攻击者左方的单位向量的夹角小于90度
        if (leftAngle > 0)
        {
            float LeftDistance = AtoB.sqrMagnitude * leftAngle;

            if (LeftDistance < 3)
            {
                conditionNum++;
            }
        }

        //计算攻击者到被攻击者的向量(单位化后)和攻击者向上的向量(单位化后)的点积
        float Upangle = Vector3.Dot(AtoB.normalized, Attacker.transform.up.normalized);

        if (Upangle > 0)
        {
            float UpDistance = AtoB.sqrMagnitude * Upangle;

            if (UpDistance < 5)
            {
                conditionNum++;
            }
        }

        //在左上方或者右上方时都满足条件,前方条件必须满足,所以3个条件只要满足两个即可
        if (conditionNum == 2)
            return true;

        return false;
    }
}


//中介者模式
public class IntermediaryMode : MonoBehaviour 
{
	void Start () 
    {
        ActorPlayer player = new ActorPlayer();
        ActorNpc npc = new ActorNpc();

        if (CaculateDistance.NeedReduceBlood(player, npc))
        {
            player.ReduceBlood();
        }

        //利用中介者来计算敌人是否进入了玩家可以攻击的范围
        if (CaculateDistance.isCanAttack(player, npc))
        {
            player.playAttack();
        }
	}
	
	void Update () 
    {
		
	}
}
