using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class IResourceLoader : IDisposable 
{
    public AssetBundle ABres;

    public IResourceLoader(AssetBundle tmpRes)
    {
        ABres = tmpRes;
    }

    /// <summary>
    /// 获得ABres中bundleName为tmpRes的AB
    /// </summary>
    /// <param name="tmpRes"></param>
    /// <returns></returns>
    public UnityEngine.Object this[string tmpRes]
    {
        get
        {
            if (ABres == null && !this.ABres.Contains(tmpRes))
            {
                Debug.LogError("this ABres is not exit:" + tmpRes);
                return null;
            }

            //最终加载AB资源
            return ABres.LoadAsset(tmpRes);
        }
    }


    public UnityEngine.Object[] LoadAll()
    {
        if (ABres == null )
        {
            Debug.LogError("this ABres is not exit:");
            return null;
        }

        return ABres.LoadAllAssets();
    }

    /// <summary>
    /// 获得budleName为tmpRes的AB与其子AB
    /// </summary>
    /// <param name="tmpRes"></param>
    /// <returns></returns>
    public UnityEngine.Object[] LoadAbFiles(string tmpRes)
    {
        if (ABres == null && !this.ABres.Contains(tmpRes))
        {
            Debug.LogError("this ABres is not exit:" + tmpRes);
            return null;
        }

        return ABres.LoadAssetWithSubAssets(tmpRes);
    }


    public void UnLoad(UnityEngine.Object obj)
    {
        UnityEngine.Resources.UnloadAsset(obj);
    }

	public void Dispose()
    {
        if (ABres == null)
        {
            Debug.LogError("ABres is not exit");
            return;
        }

        ABres.Unload(true);
    }


    //Debug
    public void DebugAllRes()
    {
        if (ABres != null)
        {
            string[] names = ABres.GetAllAssetNames();

            for (int i = 0; i < names.Length; i++ )
            {
                Debug.Log("ABres contain asset name ==" + names[i]);
            }
        }
    }
}
