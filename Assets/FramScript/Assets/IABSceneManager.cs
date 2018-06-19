using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class IABSceneManager  
{
    IABManager abManager;

    /// <summary>
    /// 将Record.txt文件中记录的AB所在场景和名称存起来
    /// </summary>
    public Dictionary<string, string> allAssets = new Dictionary<string, string>();

    public IABSceneManager(string sceneName)
    {
        abManager = new IABManager(sceneName);
    }

    //获取真正的ABName
    //如bundleName是load
    //那么GetBundleReateName返回的就是scene01/load.ld
    public string GetBundleReateName(string bundleName)
    {
        return allAssets[bundleName];
    }

    public void ReadConfig(string sceneName)
    {
        string filePath = IPathTools.GetAssetBundlePath() + "/" + sceneName + "Record.txt";

        ReadConfigByPath(filePath);
    }

    private void ReadConfigByPath(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open);

        StreamReader sw = new StreamReader(fs);

        int AllLineCount = int.Parse(sw.ReadLine());

        for (int i = 0; i < AllLineCount; i++)
        {
            string str = sw.ReadLine();

            string[] tmpstr = str.Split("|".ToCharArray());

            allAssets.Add(tmpstr[0], tmpstr[1]);
        }

        fs.Close();

        sw.Close();
    }

#region 由下层提供

    //加载一个AssetBundle
    public void LoadAsset(string key, LoadABProgress progress, LoadABFinish finish, LoadAssetBundleCallBack callback)
    {
        if (allAssets.ContainsKey(key))
        {
            //key即为Record记录文件中的|前面的字段如Load,allAssets[key]即为Record记录文件中|后面要加载的AB的名称如scene01/load.ld
            string bundleName = allAssets[key];

            //abManager是在创建IABSceneManager时其构造函数中创建的用来具体加载AB
            abManager.LoadAssetBundle(bundleName, progress, finish,callback);
        }
        else
        {
            Debug.Log("config is not contain this key:" + key);
        }
    }

    public bool IsLoadAssetBundle(string BundleName)
    {
        return abManager.IsLoadAssetBundle(allAssets[BundleName]);
    }

    //这里只是一个接口进行异步加载
    public IEnumerator LoadAssetSys(string BundleName,LoadABFinish finish)
    {
        yield return abManager.LoadAssetBundles(BundleName, finish);
    }

    /// <summary>
    /// 调用LoadAsset()从AssetBundle中加载资源
    /// </summary>
    /// <param name="BundleName"></param>
    /// <param name="ABName"></param>
    /// <returns></returns>
    public Object GetSingelABFile(string BundleName, string ABName)
    {
        return abManager.GetSingleABFile(BundleName, ABName);
    }

    /// <summary>
    /// 调用LoadAssetWithSubAssets()从AssetBundle中加载该AssetBundle所引用的资源文件(如Texture,mesh等)
    /// </summary>
    /// <param name="BundleName"></param>
    /// <param name="ABName"></param>
    /// <returns></returns>
    public Object[] GetABFiles(string BundleName, string ABName)
    {
        return abManager.GetABFiles(BundleName, ABName);
    }

    /// <summary>
    /// 释放一个包名为BundleName中名称为ABName的asset
    /// </summary>
    /// <param name="BundleName"></param>
    /// <param name="ABName"></param>
    public void DisposeSingleABFile(string BundleName, string ABName)
    {
        abManager.DisposeSingleABFile(BundleName, ABName);
    }

    /// <summary>
    /// 释放包名为BundleName的所有AssetBundle资源
    /// </summary>
    /// <param name="BundleName"></param>
    public void DisposeABFiles(string BundleName)
    {
        abManager.DisposeABFiles(BundleName);
    }

   /// <summary>
   /// 释放所有已经加载的AB
   /// </summary>
    public void DisposeAllABFiles()
    {
        abManager.DisposeAllABFiles();
    }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="bundleName"></param>
    public void DisposeBundle(string bundleName)
    {
        abManager.DisposeBudnle(bundleName);
    }

    //释放所有的AssertBudle
    //这里是把budle释放,一般是当资源加载完成的时候就调用这个函数,当退出系统的时候再调用DisposeAllABFiles方法来释放实例化的资源
    public void DisposeAllBnudle()
    {
        abManager.DisposeAllBundle();

        allAssets.Clear();
    }

    //释放所有的AssertBudle和所有的AB文件
    public void DisposeAllBundleAndABFiles()
    {
        abManager.DisposeAllBundleAndABFiles();

        allAssets.Clear();
    }


    public void DebugAllAssert(string bundleName)
    {
        abManager.DebugAllAseet(bundleName);
    }

#endregion

}
