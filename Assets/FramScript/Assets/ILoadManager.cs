using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ILoadManager : MonoBehaviour 
{
    public static ILoadManager instance;

    /// <summary>
    /// 场景字典
    /// </summary>
    public Dictionary<string, IABSceneManager> loadManger = new Dictionary<string, IABSceneManager>();

    void Awake()
    {
        instance = this;

        IABManifesLoader.instacne.LoadManifest();
    }

    /// <summary>
    /// 读取配置文件,主要用来将UI_Main转换为Scene01/UI_Main
    /// </summary>
    /// <param name="sceneName"></param>
    public void ReadConfig(string sceneName)
    {
        if (!loadManger.ContainsKey(sceneName))
        {
            //构建一个IABSceneManager来管理某一个场景的AB资源
            IABSceneManager sceneManger = new IABSceneManager(sceneName);

            sceneManger.ReadConfig(sceneName);

            //将该场景IABSceneManager加入到loadManger
            loadManger.Add(sceneName, sceneManger);
        }
    }

    /// <summary>
    /// 加载AssetBundle
    /// </summary>
    /// <param name="sceneName">场景名字(如在给资源标记AssetBundle里的那个Scene01)</param>
    /// <param name="key">包名称(如果包名为Scene01/UI_Main,那这里这个key就是UI_Main)</param>
    /// <param name="progress"></param>
    /// <param name="finish"></param>
    public void LoadAsset(string sceneName,string key,LoadABProgress progress,LoadABFinish finish)
    {
        if (!loadManger.ContainsKey(sceneName))
        {
            ReadConfig(sceneName);
        }

        //ReadConfig会构建一个场景名称为sceneName的IABSceneManager,并且添加到loadManger中
        IABSceneManager loader = loadManger[sceneName];

        LoadAssetBundleCallBack callback = (x,y) =>
        {
            if (loadManger.ContainsKey(x))
            {
                IABSceneManager curSceneManager = loadManger[x];

                //从这里开始加载AB包
                StartCoroutine(curSceneManager.LoadAssetSys(y, finish));
            }
            else
            {
                Debug.LogError("错误的场景");
            }
        };

        loader.LoadAsset(key, progress, finish, callback);
    }

    #region 由下层提供
    /// <summary>
    /// 获得一个已经LoadAsset()后的AssetBundle
    /// </summary>
    /// <param name="sceneName">场景名字</param>
    /// <param name="BundleName">资源包名</param>
    /// <param name="ABName">资源包中的具体asset的名称</param>
    /// <returns></returns>
    public Object GetSingleABFile(string sceneName,string BundleName,string ABName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            return tmpManager.GetSingelABFile(BundleName, ABName);
        }
        return null;
    }

    /// <summary>
    /// 获得带场景名称的包名(这里如果传入UI_Main,那么返回Scene01/UI_Main)
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="BundleName"></param>
    /// <returns></returns>
    public string GetBundleReateName(string sceneName, string BundleName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            return tmpManager.GetBundleReateName(BundleName);
        }

        return null;
    }

    /// <summary>
    /// 是否已经加载了一个包名为BudnleName的AssetBundle
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="BudnleName"></param>
    /// <returns></returns>
    public bool IsLoadAssetBundle(string sceneName, string BudnleName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            return tmpManager.IsLoadAssetBundle(BudnleName);
        }

        return false;
    }

    /// <summary>
    /// 调用LoadAssetWithSubAssets()从AssetBundle中加载该AssetBundle所引用的资源文件(如Texture,mesh等)
    /// </summary>
    /// <param name="BundleName"></param>
    /// <param name="ABName"></param>
    /// <returns></returns>
    public Object[] GetABFiles(string sceneName, string BundleName, string ABName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            return tmpManager.GetABFiles(BundleName, ABName);
        }

        return null;
    }

    /// <summary>
    /// 释放一个AssetBundle中名称为ABName的asset
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="BundleName"></param>
    /// <param name="ABName"></param>
    public void UnLoadSingleABFile(string sceneName, string BundleName, string ABName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            tmpManager.DisposeSingleABFile(BundleName, ABName);
        }
    }

    /// <summary>
    /// 释放包名为BundleName中的所有AB文件
    /// </summary>
    /// <param name="BundleName"></param>
    /// <param name="ABName"></param>
    /// <returns></returns>
    public void UnLoadABFiles(string sceneName, string BundleName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            tmpManager.DisposeABFiles(BundleName);
        }
    }

    /// <summary>
    /// 释放所有的AssetBundle
    /// </summary>
    /// <param name="sceneName"></param>
    public void UnLoadAllABFiles(string sceneName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            tmpManager.DisposeAllABFiles();
        }
    }

    /// <summary>
    /// 释放一个AssetBundle,并且会将该AssetBundle和其依赖项的被依赖关系删除,如果某一个依赖项删除了和该AssetBundle的依赖关系后再也没有其他文件所依赖了就将该依赖项也释放
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="BundleName"></param>
    public void UnLoadSingleBundle(string sceneName, string BundleName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            tmpManager.DisposeBundle(BundleName);
        }
    }

    /// <summary>
    /// 释放对应场景中所有的AssetBundle
    /// </summary>
    /// <param name="sceneName"></param>
    public void UnLoadAllBundle(string sceneName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            tmpManager.DisposeAllBnudle();

            //因为这里所有的Budnle都被释放所有回收一下
            System.GC.Collect();
        }
    }

    /// <summary>
    /// 释放对应场景所有的AssetBundle和已经Load的asset
    /// </summary>
    /// <param name="sceneName"></param>
    public void UnLoadAllBundleAndABFils(string sceneName)
    {
        if (loadManger.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManger[sceneName];

            tmpManager.DisposeAllBundleAndABFiles();

            //因为这里所有的Budnle都被释放所有回收一下
            System.GC.Collect();
        }
    }
#endregion

    void OnDestroy()
    {
        loadManger.Clear();

        System.GC.Collect();
    }
}
