using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void LoadAssetBundleCallBack(string sceneName,string bundleName);

//存的是一个AssertBundle里面的某一个AB文件
public class ABFile
{
    //用List是因为一个AB文件里也可能含有多个子AB文件,如一个Sprite里面可能含有很多个子图片
    public List<Object> objs;

    public ABFile(params Object[] tmpObj)
    {
        objs = new List<Object>();

        objs.AddRange(tmpObj);
    }

    public void ReleaseObj()
    {
        for (int i = 0; i < objs.Count; i++)
        {
            Resources.UnloadAsset(objs[i]);
        }
    }
}

//缓存加载出来的AssetBundle,即使该AssetBundle中只有一个asset,也用个类来缓存,取得时候取0位即可
public class ABFileList
{
    public Dictionary<string, ABFile> ABFiles;

    //根据某个AB文件来构造ABFileList
    public ABFileList(string ABName, ABFile ab)
    {
        ABFiles = new Dictionary<string, ABFile>();

        ABFiles.Add(ABName, ab);
    }

    //添加AB文件
    public void AddAbFile(string ABName, ABFile ab)
    {
        ABFiles.Add(ABName, ab);
    }

    //释放所有的AB文件
    public void ReleaseAllAbFileObj()
    {
        List<string> keys = new List<string>();
        keys.AddRange(ABFiles.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            ReleaseAbfileObj(keys[i]);
        }
    }

    //释放单个AB文件
    public void ReleaseAbfileObj(string ABName)
    {
        if (ABFiles.ContainsKey(ABName))
        {
            ABFile ab = ABFiles[ABName];

            ab.ReleaseObj();
        }
        else
        {
            Debug.Log("ABFiles is not contain:" + ABName);
        }
    }

    //获取某个AB文件
    public List<Object> GetAbFile(string ABName)
    {
        if (ABFiles.ContainsKey(ABName))
        {
            ABFile ab = ABFiles[ABName];

            return ab.objs;
        }

        return null;
    }
}

public class IABManager 
{
    //这个是将所有已经加载的AssetBundle存起来,一个IABRelationManager就是一个AB包
    Dictionary<string, IABRelationManager> loadHelper = new Dictionary<string, IABRelationManager>();

    //这个是将所有已经加载的AssetBundle中的所有AB文件缓存起来,每一个ABFileList都对应一个AssetBundle中的所有AB文件
    public Dictionary<string, ABFileList> LoadObjs = new Dictionary<string, ABFileList>();

    string sceneName;

    string BundlePath;


    public IABManager(string scenename)
    {
        //场景名字
        sceneName = scenename;
    }

    //获取依赖
    string[] GetDependces(string bundlename)
    {
        return IABManifesLoader.instacne.GetDependces(bundlename);
    }

    //给外面使用
    public void LoadAssetBundle(string BundleName,LoadABProgress Lp,LoadABFinish finishCallback, LoadAssetBundleCallBack callback)
    {
        //loadHelper是用来存放IABRelationManager的字典
        if (!loadHelper.ContainsKey(BundleName))
        {
            //每一个IABRelationManager就是一个AB
            IABRelationManager loader = new IABRelationManager(BundleName,Lp, finishCallback);

            //将该IABRelationManager存放在loadHelper中
            loadHelper.Add(BundleName, loader);

            //这里的callback就会调用下面的LoadAssetBundles
            callback(sceneName, BundleName);
        }
        else
        {
            Debug.Log("loadHelper is already have BundleName:" + BundleName);
        }
    }

    public IEnumerator LoadAssetBundles(string BundleName,LoadABFinish finsh)
    {
        //从loadHelper中取出对应的IABRelationManager
        IABRelationManager loader = loadHelper[BundleName];

        string[] depends = GetDependces(BundleName);

        //添加依赖关系,loader依赖了哪些AB资源
        loader.SetDependces(depends);

        //加载依赖项
        //注意,当AB包使用Unity内置Shader(如Unlit/Texture),需要在Edit->ProjectSetting->Graphics中的Always Including Shaders中将该shader添加进去,否则会出现shader丢失的情况,添加后需要重新打包
        for (int i = 0; i < depends.Length; i++)
        {
           yield return LoadAssetBundleDependces(depends[i], BundleName, loader.GetProgress(), finsh);
        }

        loader.LoadAssetBundleFromFile();
    }

 
    //加载依赖的AssertBundle
    public IEnumerator LoadAssetBundleDependces(string bundleName,string refName,LoadABProgress progress,LoadABFinish finishCallBack)
    {
        //加载依赖项的AB
        IABRelationManager loader = new IABRelationManager(bundleName, progress, finishCallBack);

        if (!loadHelper.ContainsKey(bundleName))
        {   
            if (refName != null)
            {
                //添加被依赖关系,loader被哪些AB所依赖
                loader.AddRefference(refName);
            }

            loadHelper.Add(bundleName, loader);

            yield return LoadAssetBundles(bundleName, finishCallBack);
        }
        else
        {
            if (refName != null)
            {
                loader.AddRefference(refName);
            }
        }
    }

#region 由下层提供API
    public void DebugAllAseet(string bundleName)
    {
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager curABRelation = loadHelper[bundleName];

            curABRelation.DebugAllAseet();
        }
    }
#endregion

    //是否将已经加载的assetbundle存放到loadHelper中
    public bool IsLoadAssetBundle(string bundleName)
    {
        if (loadHelper.ContainsKey(bundleName))
            return true;
        else
            return false;
    }

    //获取单个AB文件
    public Object GetSingleABFile(string bundleName, string ABName)
    {
        if (LoadObjs.ContainsKey(bundleName))
        {
            ABFileList abfiles = LoadObjs[bundleName];

            List<Object> abs = abfiles.GetAbFile(ABName);

            //找到了对应的AB文件,为什么这里从ABFileList里取出来的某一个AB资源还是list,因为在构建ABFile时传入的是可变参数params,所以返回的是一个list,但是一般都是只有一个value
            if (abs != null)
            {
                if (abs[0] != null)
                    return abs[0];
                else
                    LoadObjs.Remove(bundleName);
            }
        }

        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager curABRelation = loadHelper[bundleName];

            Object tmpObj = curABRelation.GetSingleAbFile(ABName);

            //构建ABFile时传入的是可变参数params,所以ABFile.abs是一个list,但是一般都是只有一个value
            ABFile ab = new ABFile(tmpObj);

            //缓存中存在包名为bundleName的ABFileList,但是因为能走到这里,说明这个ABFileList不包含这个AB文件(但是是相同的bundleName)
            if (LoadObjs.ContainsKey(bundleName))
            {
                ABFileList abs = LoadObjs[bundleName];

                abs.AddAbFile(ABName, ab);
            }
            else//缓存中没有包名为bundleName的ABFileList(创建一个新的ABFileList来存放包名为bundleName的AB,以后只要和这个bundleName相同名称的AB都会被加入到这个ABFileList中)
            {
                ABFileList abs = new ABFileList(ABName, ab);

                LoadObjs.Add(bundleName, abs);
            }

            return tmpObj;
        }

        return null;
    }

    //获取带子AB文件的Ab文件
    public Object[] GetABFiles(string bundleName, string ABName)
    {
        if (LoadObjs.ContainsKey(bundleName))
        {
            ABFileList abfiles = LoadObjs[bundleName];

            List<Object> abs = abfiles.GetAbFile(ABName);

            //找到了对应的AB文件
            if (abs != null)
            {
                if (abs[0] != null)
                    return abs.ToArray();
                else
                    LoadObjs.Remove(bundleName);
            }
        }

        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager curABRelation = loadHelper[bundleName];

            Object[] tmpObj = curABRelation.GetAbFiles(ABName);

            ABFile ab = new ABFile(tmpObj);

            //缓存中存在包名为bundleName的ABFileList,但是因为能走到这里,说明这个ABFileList不包含这个AB文件
            if (LoadObjs.ContainsKey(bundleName))
            {
                ABFileList abs = LoadObjs[bundleName];

                abs.AddAbFile(ABName, ab);
            }
            else//缓存中没有包名为bundleName的ABFileList
            {
                ABFileList abs = new ABFileList(ABName, ab);

                LoadObjs.Add(bundleName, abs);
            }

            return tmpObj;
        }

        return null;
    }

#region 释放缓存
    //释放某一个AssetBundle中的某个AB文件
    public void DisposeSingleABFile(string bundleName, string ABName)
    {
        if (LoadObjs.ContainsKey(bundleName))
        {
            ABFileList abs = LoadObjs[bundleName];

            abs.ReleaseAbfileObj(ABName);
        }
    }

    //释放某一个AssetBundle中的某个AB文件
    public void DisposeABFiles(string bundleName)
    {
        if (LoadObjs.ContainsKey(bundleName))
        {
            ABFileList abs = LoadObjs[bundleName];

            abs.ReleaseAllAbFileObj();
        }

        Resources.UnloadUnusedAssets();
    }

    //释放所有的AB包
    public void DisposeAllABFiles()
    {
        List<string> keys = new List<string>();
        keys.AddRange(LoadObjs.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            DisposeABFiles(keys[i]);
        }

        LoadObjs.Clear();

        Resources.UnloadUnusedAssets();
    }

    //卸载一个AssetBudnle
    public void DisposeBudnle(string bundleName)
    {
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];

            //获得该AssetBundle的所有依赖项
            List<string> dependces = loader.GetDependces();

            //将所有依赖项中对该AssetBundle的被依赖关系移除
            for (int i = 0; i < dependces.Count; i++)
            {
                if (loadHelper.ContainsKey(dependces[i]))
                {
                    IABRelationManager depend = loadHelper[dependces[i]];
                    
                    if (depend.RemoveReffer(bundleName))
                    {
                        DisposeBudnle(depend.GetBundleName());
                    }
                }
            }

            //如果当前AB文件不再被任何资源所依赖那么就将将该AB彻底卸载
            if (loader.GetRefference().Count <= 0)
            {
                loader.Dispose();

                loadHelper.Remove(bundleName);
                LoadObjs.Remove(bundleName);
            }
        }
    }

    public void DisposeAllBundle()
    {
        List<string> keys = new List<string>();
        keys.AddRange(loadHelper.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            IABRelationManager loader = loadHelper[keys[i]];

            loader.Dispose();
        }
        loadHelper.Clear();
        LoadObjs.Clear();
    }

    public void DisposeAllBundleAndABFiles()
    {
        DisposeAllABFiles();

        DisposeAllBundle();
    }

#endregion

}
