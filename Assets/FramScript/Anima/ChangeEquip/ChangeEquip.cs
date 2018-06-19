using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//这种换装是最好的方式
//如果要换的装备本身带有动画(如鱼竿),那么替换到人物身上(人物本身也含有动画)就可以使用这种方式来换装
//有个前提条件是没换装之前人物身上的要被替换的部分(如本例中的鱼竿)和人物本身要做在一个FBX里面(要替换部分的骨骼要做到人物身上)
//如果要换的东西本身不带动画(如刀剑)只要找到人物身上的手部骨骼直接绑定在上面即可,可以不使用本方法
public class ChangeEquip : MonoBehaviour
{
    //被换的人
    public Transform Player;

    //要换的装备
    public Transform PartObject;

    public void ChangePlayerEquip(Transform PartTransform, Transform PlayerTransform)
    {
        //新鱼竿下所有的Skinmeshrender组件
        SkinnedMeshRenderer[] PartSkinMeshs = PartTransform.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer skin in PartSkinMeshs)
        {
            ProcessChange(skin, PlayerTransform);
        }
    }

    void ProcessChange(SkinnedMeshRenderer changSkin,Transform RootTransform)
    {
        //根据新鱼竿下的某个子Skinmeshrender组件的名字重新创建一个GameObject
        GameObject newObj = new GameObject(changSkin.name);

        //将新创建的GameObject直接放在人物(根骨骼)的下面
        newObj.transform.parent = RootTransform.transform;

        //为新创建的GameObject添加一个SkinnedMeshRenderer组件
        SkinnedMeshRenderer newSkinMeshRender = newObj.AddComponent<SkinnedMeshRenderer>();

        Transform[] Mybones = new Transform[changSkin.bones.Length];

        for (int i = 0; i < changSkin.bones.Length; i++)
        {
            Mybones[i] = FindBone(changSkin.bones[i].name, RootTransform);
        }

        newSkinMeshRender.rootBone = RootTransform;

        newSkinMeshRender.bones = Mybones;

        newSkinMeshRender.sharedMesh = changSkin.sharedMesh;

        newSkinMeshRender.materials = changSkin.materials;
    }

    Transform FindBone(string boneName, Transform RootTransform)
    {
        Transform reault = null;

        if (boneName == RootTransform.gameObject.name)
        {
            reault = RootTransform;

            return reault;
        }

        for (int i = 0; i < RootTransform.childCount; i++)
        {
            reault = FindBone(boneName, RootTransform.GetChild(i));

            if(reault != null)
                return reault;
        }

        return reault;
    }

	void Start ()
    {
        ChangePlayerEquip(PartObject, Player);
    }

}
