using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABRelationManager  
{
    public IABLoader assetLoader;

    //依赖项的AB名称,当前AB资源依赖哪些其他的AB资源
    public List<string> DependceBundleName;

    //被依赖项的AB名称,当前AB资源被哪些其他AB资源所依赖
    public List<string> RefferBundleName;

    private string BundleName;

    private bool IsLoadAssetBundleFinish = false;

    LoadABProgress progress;

    //传进来要加载的AB包的路径和名称
    public IABRelationManager(string bundleName,LoadABProgress Lp,LoadABFinish finishCallback)
    {
        DependceBundleName = new List<string>();

        RefferBundleName = new List<string>();

        IsLoadAssetBundleFinish = false;

        progress = Lp;

        BundleName = bundleName;

        assetLoader = new IABLoader(BundleName, progress, finishCallback);
    }

    public LoadABProgress GetProgress()
    {
        return progress;
    }

    public string GetBundleName()
    {
        return BundleName;
    }

    //添加依赖关系
    public void SetDependces(string[] bundleNames)
    {
        if (bundleNames.Length > 0)
            DependceBundleName.AddRange(bundleNames);
    }

    //获取依赖关系
    public List<string> GetDependces()
    {
        return DependceBundleName;
    }

    //移除一个依赖关系
    public void RemoveDepence(string bundleName)
    {
        for (int i = 0; i < DependceBundleName.Count; i++)
        {
            if (bundleName.Equals(DependceBundleName[i]))
                DependceBundleName.Remove(bundleName);
        }
    }


    //添加被依赖关系
    public void AddRefference(string bundleName)
    {
        if (!RefferBundleName.Contains(bundleName))
            RefferBundleName.Add(bundleName);
    }

    //得到被依赖关系
    public List<string> GetRefference()
    {
        return RefferBundleName;
    }


    //移除一个被依赖关系
    public bool RemoveReffer(string bundleName)
    {
        for (int i = 0; i < RefferBundleName.Count; i++)
        {
            //只有string类型的Equals和==是一样的,Equals比较的是地址,==比较的是内容
            if (bundleName.Equals(RefferBundleName[i]))
                RefferBundleName.Remove(bundleName);
        }

        //如果任何AB文件都不再依赖当前的AB文件,则将当前的AB文件释放
        if (RefferBundleName.Count <= 0)
        {
            Dispose();

            return true;
        }

        return false;
    }

    //获取单个AB文件
    //建造者模式,assetLoader是下层,IABRelationManager是上层,一层一层的调用
    public UnityEngine.Object GetSingleAbFile(string name)
    {
        return assetLoader.GetSingleAbFile(name);
    }


    //获取有子AB包的AB文件(多个,带子AB文件的)
    //建造者模式,assetLoader是下层,IABRelationManager是上层,一层一层的调用
    public UnityEngine.Object[] GetAbFiles(string name)
    {
        return assetLoader.GetAbFiles(name);
    }

    /// <summary>
    /// 从磁盘加载AB
    /// </summary>
    public void LoadAssetBundleFromFile()
    {
        assetLoader.LoadAssetBundleFormFile();
    }


    public void DebugAllAseet()
    {
        if (assetLoader != null)
            assetLoader.DebugAllAsset();
        else
            Debug.LogError("this assetLoader is null");
    }

    public void Dispose()
    {
        assetLoader.Dispose();
    }
}
