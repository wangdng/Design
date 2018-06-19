using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFactory : MonoBehaviour {

    object[] allSprites;

    int index = 0;

	void Start () 
    {
        allSprites = Resources.LoadAll("Number");
	}


    //工厂方法,只需要传一个参数进去就能给我生成一个对象处来,我们不关心产生对象的过程
    void GetSprite(int index)
    {
        GameObject obj = new GameObject();

        obj.AddComponent<Image>();

        obj.GetComponent<Image>().sprite = allSprites[index] as Sprite;

        obj.transform.parent = transform;

        obj.transform.position = new Vector3(index * 20, 0, 0);
    }
	

	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            index++;
            index = index % 9;

            GetSprite(index);
        }
	}
}
