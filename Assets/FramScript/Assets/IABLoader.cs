using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void LoadABProgress(string bundlename,float progress);

public delegate void LoadABFinish(string bundlename);

/// <summary>
/// 每一个AB资源加载工具类
/// </summary>
public class IABLoader 
{
    private string BundlePath;

    /// <summary>
    /// 包名
    /// </summary>
    private string BundleName;

    /// <summary>
    /// 加载进度
    /// </summary>
    public float LoadProgress;

    /// <summary>
    /// AB加载完成后从AB中加载对应的Object的工具类
    /// </summary>
    public IResourceLoader IResLoader;

    private LoadABProgress loadAbprogress;

    /// <summary>
    /// 加载完成回调
    /// </summary>
    private LoadABFinish loadAbfinish;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="bundleName">包名</param>
    /// <param name="loadPrgress"></param>
    /// <param name="loadFinish">加载完成回调</param>
    public IABLoader(string bundleName, LoadABProgress loadPrgress, LoadABFinish loadFinish)
    {
        BundleName = bundleName;
        LoadProgress = 0;

        loadAbprogress = loadPrgress;
        loadAbfinish = loadFinish;
    }


    public string GetBundlePath()
    {
        string BundlePath = IPathTools.GetAssetBundlePath() + "/" + BundleName;

        return BundlePath;
    }

#region 给上层加载AB使用

    /// <summary>
    /// 直接从硬盘加载AB
    /// </summary>
    public void LoadAssetBundleFormFile()
    {
        if(BundleName != null)
        {
            string assetPath = GetBundlePath();
            //这种加载方式在4.x时代只能加载未压缩的AB包,但是5之后可以加载压缩的AB包
            //这个方法实际就是Unity4.x的AssetBundle.CreateFromFile

            //这里不需要担心重复加载相同的AssetBundle,因为这里每一个AssetBundle都会对应一个IABLoader,并且加载完成后就缓存起来了
            AssetBundle ab = AssetBundle.LoadFromFile(assetPath);

            IResLoader = new IResourceLoader(ab);

            if (loadAbfinish != null)
                loadAbfinish(BundleName);
        }
    }

#endregion


#region 调试使用
    public void DebugAllAsset()
    {
        if (IResLoader == null)
            return;

        IResLoader.DebugAllRes();
    }
#endregion


#region  给上层调用

    //获得单一资源
    public UnityEngine.Object GetSingleAbFile(string name)
    {
        if (IResLoader == null)
        {
            Debug.LogError("IResLoader is null");
            return null;
        }
        return IResLoader[name];
    }

    //获得多个资源
    public UnityEngine.Object[] GetAbFiles(string name)
    {
        if (IResLoader != null)
        {
            Debug.LogError("IResLoader is null");
            return null;
        }
        return IResLoader.LoadAbFiles(name);
    }


    //卸载
    public void UnLoad(UnityEngine.Object obj)
    {
        if (IResLoader != null)
        {
            IResLoader.UnLoad(obj);
        }
    }

    //释放
    public void Dispose()
    {
        if (IResLoader != null)
        {
            IResLoader.Dispose();

            IResLoader = null;
        } 
    }

#endregion
	
}
