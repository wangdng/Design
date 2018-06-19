using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//工厂模式
public class Food
{
    public virtual void eat()
    {

    }
}

public class TomatFood : Food
{
    public override void eat()
    {
        Debug.Log("我吃了西红柿");
    }
}

public class EggFood : Food
{
    public override void eat()
    {
        Debug.Log("我吃了鸡蛋");
    }
}

//工厂类
public class FoodFactory 
{
    //工厂方法,传入一个参数返回一个对象,不关心是什么对象和产生的过程
    public void GetFood(string type)
    {
        Food food = null;
        if (type == "1")
        {
            food = new TomatFood();
       
        }
        else if (type == "2")
        {
            food = new EggFood();
        }

        if(food != null)
            food.eat();
    }
}
