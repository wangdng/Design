using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//策略模式
public class PlayAnimBase
{
    public virtual void PlayAnim()
    {

    }
}

public class SlefPlayAnim:PlayAnimBase
{
    float x;

    public SlefPlayAnim(float d)
    {
        x = d;
    }

    public override void PlayAnim()
    {
        x*=0.12f;
        Debug.Log(x);
    }
}


public class EnemyPlayAnim : PlayAnimBase
{
    float x;
    public EnemyPlayAnim(float d)
    {
        x = d;
    }
    public override void PlayAnim()
    {
        x *= 0.2f;
        Debug.Log(x);
    }
}

//这个就是策略模式的工具类,有这个工具类就不用分别调用子类中的重写的函数了,只调用父类中的虚函数就行了
public class AnimPlayer
{
    public void PlayAnim(PlayAnimBase player)
    {
        player.PlayAnim();
    }
}

public class StrategyMode : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        //定义一个工具类,这样就可以使用同一个api了
        AnimPlayer animPlayer = new AnimPlayer();

        PlayAnimBase self = new SlefPlayAnim(100);
        PlayAnimBase enemy = new EnemyPlayAnim(100);

        //同一个api传入不同的参数即可得到不同的结果,这就是策略模式
        animPlayer.PlayAnim(self);
        animPlayer.PlayAnim(enemy);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
