using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//观察者模式
public class ObserverMode : MonoBehaviour {

    Animation Player;


	void Start () 
    {
		
	}

    bool isFinish()
    {
        return Player.isPlaying == true ? false:true;
    }

    void Play()
    {
        time = 0;
        Player.Play();
    }

    float time = 0;
	
	void Update () 
    {
        if (Input.GetKey(KeyCode.B))
        {
            Play();
        }

        //每一帧都在观察动作是否完成,如果没有完成,在动画播放到0.5秒的时候播放一个粒子特效.这就是观察者模式
        if (!isFinish())
        {
            time += Time.deltaTime;

            if (time >= 0.5)
            {
                time = 0;

                Debug.Log("播放粒子特效");
            }
        }
	}
}
