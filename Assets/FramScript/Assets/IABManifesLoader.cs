using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class IABManifesLoader  
{
    private static IABManifesLoader mIns;
   

    public static IABManifesLoader instacne
    {
        get
        {
            if (mIns == null)
            {
                mIns = new IABManifesLoader(); 
            }

            return mIns;
        }
    }


    public AssetBundleManifest assetManifest;

    public bool isLoadFinsh;

    public AssetBundle ManifestLoader;

    public string ManifestPath;


    public IABManifesLoader()
    {
        assetManifest = null;
        ManifestLoader = null;

        ManifestPath = IPathTools.GetAssetBundlePath() + "/" + IPathTools.GetPlatformFolderName();
    }


    public static string GetPlatformFolderName()
    {
#if UNITY_ANDROID
        return "Android";
#elif UNITY_IOS
        return "IOS";
#elif UNITY_STANDALONE_WIN
         return "Windows";
#elif  UNITY_STANDALONE_OSX
        return "OSX";
#endif
    }

    //加载Manifest文件
    public void LoadManifest()
    {
         AssetBundle localManifestLoader = AssetBundle.LoadFromFile(ManifestPath);

            //加载AB的Manifest文件时 LoadAsset传入的参数一定要是"AssetBundleManifest"
        assetManifest = localManifestLoader.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
    }

    //获取依赖
    public string[] GetDependces(string name)
    {
        return assetManifest.GetAllDependencies(name);
    }
}
