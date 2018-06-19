using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPlayerData
{
    private float bloodValue;

    public MonsterPlayerData(float hp)
    {
        bloodValue = hp;
    }

    public void ReduceBlood(float value)
    {
        bloodValue -= value;

        if (bloodValue < 0)
            bloodValue = 0;
    }

    public float GetBlood()
    {
        return bloodValue;
    }

}
