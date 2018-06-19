using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//建造者模式
//从内存中加载Ab文件(和加载到内存分开)
public class IABResources
{
    AssetBundle assetbundle;

    public IABResources(AssetBundle bundle)
    {
        assetbundle = assetbundle;
    }

    public UnityEngine.Object LoadRes(string res)
    {
       return assetbundle.LoadAsset(res);
    }


    public UnityEngine.Object[] LoadAllRes(string res)
    {
        return assetbundle.LoadAssetWithSubAssets(res);
    }
}

 //将AB文件加载到内存(和从内存中加载ab文件分开)
public class AssetBundleLoad
{
    public Dictionary<string, IABResources> assetBundleList = new Dictionary<string, IABResources>();

    string assetBundleName;

    //给上层调用
    public void SetCurAssetBundleName(string name)
    {
        assetBundleName = name;
    }

    //给上层调用
    public IEnumerator LoadAssetBundleToRAM(string path)
    {
        WWW www = new WWW(path);

        while (!www.isDone)
        {
            yield return www;
        }

        IABResources bundle = new IABResources(www.assetBundle);

        assetBundleList.Add(assetBundleName, bundle);
    }
}


public class BuiderMode : MonoBehaviour 
{

	void Start () 
    {
        AssetBundleLoad Abload = new AssetBundleLoad();

        //上层直接调用
        Abload.SetCurAssetBundleName("test");

        string path = "file://" + Application.streamingAssetsPath + "/test/" + "test.unity3d";

        StartCoroutine(Abload.LoadAssetBundleToRAM(path));

        IABResources IAb = Abload.assetBundleList["test"];

        //最终加载出来
        UnityEngine.Object obj = IAb.LoadRes("test");
	}
	
	
	void Update () 
    {
		
	}
}
