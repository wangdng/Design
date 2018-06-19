using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssetBundleManager : MonoBehaviour
{
    //当前已经拷贝到沙盒路径下的AB文件数量
    private int copy2SandboxFileNum = 0;

    private int downLoadAssetBundleNum = 0;

    //全部AB文件字典
    private  Dictionary<string,string> streamAssetbundles = new Dictionary<string,string>();

    //streamingAssetsPath对应AB文件的平台路径(如E:\DesignPatterns\DesignPatterns\Assets\StreamingAssets\AssetBundle\Android)
    private string streamAssetBundleRoot = "";

    //沙盒目录中对应AB文件的平台路径(如C:\Users\SmartKey\AppData\LocalLow\DefaultCompany\Design Patterns\AssetBundle\Android)
    private string sandBoxAssetBundleRoot = "";

    //平台
    private string platFormName = "";

    private Slider valueSlider;

    string serverAssetBundleRootPath = "";

    private Dictionary<string, Hash128> serverAssetBundleHashs = new Dictionary<string, Hash128>();
    private Dictionary<string, Hash128> localAssetBundleHashs = new Dictionary<string, Hash128>();

    private byte[] serverVersionData = null;

    private byte[] serverConfigAssetBundleData = null;

    /// <summary>
    /// 最终需要从服务器下载的AssetBundle
    /// </summary>
    private List<string> needDownLoadAssetNames = new List<string>();

    private int maxDownLoadNum = 0;

    /// <summary>
    /// 本地AssetBundle加载进度(从StreamAssets文件夹拷贝到沙盒文件夹下的过程)
    /// </summary>
    private int sliderProgress = 0;

    private Text valueTxt;

    private enum UpdateAssetState
    {
        None,
        Copy2SandBox,
        StartUpLoadFromServer,
        UpLoading,
        Complete,
    }

    private UpdateAssetState updateAssetState = UpdateAssetState.None;

    private void FixedUpdate()
    {
        if (updateAssetState == UpdateAssetState.Copy2SandBox)
        {
            valueSlider.value = sliderProgress / 100.0f;
            valueTxt.text = "正在加载资源包,不消耗流量....." + sliderProgress + "%";
            if (sliderProgress == 100)
                updateAssetState  = UpdateAssetState.StartUpLoadFromServer;
        }
        else if(updateAssetState == UpdateAssetState.StartUpLoadFromServer)
        {
            sliderProgress = 0;
            DownLoadAssetBundleFormServer();
            updateAssetState = UpdateAssetState.UpLoading;
        }
        else if(updateAssetState == UpdateAssetState.UpLoading)
        {
            valueSlider.value = sliderProgress / 100.0f;
            valueTxt.text = "正在更新资源包,请稍候....." + sliderProgress + "%";
            if (sliderProgress == 100)
                updateAssetState = UpdateAssetState.Complete;
        }
        else if(updateAssetState == UpdateAssetState.Complete)
        {
            OnUpdateAssetBundleComplete();
            updateAssetState = UpdateAssetState.None;
        }
    }

    /// <summary>
    /// 热更新资源完成
    /// </summary>
    void OnUpdateAssetBundleComplete()
    {
        string localVersionFile = Application.persistentDataPath + "/AssetBundle/" + platFormName + "/Version.txt";

        //覆盖本地的版本文件
        if (serverVersionData != null)
        {
            if (File.Exists(localVersionFile))
                File.Delete(localVersionFile);
            File.WriteAllBytes(localVersionFile, serverVersionData);
        }
        string loclConigAssetBundlePath = Application.persistentDataPath + "/AssetBundle/" + platFormName + "/" + platFormName;

        //覆盖本地的AssetBundle配置文件
        if (serverConfigAssetBundleData != null)
        {
            if (File.Exists(loclConigAssetBundlePath))
                File.Delete(loclConigAssetBundlePath);
            File.WriteAllBytes(loclConigAssetBundlePath, serverConfigAssetBundleData);
        }
        serverVersionData = null;
        serverConfigAssetBundleData = null;
    }

    private void Awake()
    {
#if UNITY_ANDROID
        platFormName = "Android";
#elif UNITY_IOS
        platFormName = "IOS";
#elif UNITY_STANDALONE_WIN
         platFormName = "Windows";
#elif UNITY_STANDALONE_OSX
        platFormName = "OSX";
#endif
        streamAssetBundleRoot = Application.streamingAssetsPath + "/AssetBundle/" + platFormName;
        sandBoxAssetBundleRoot = Application.persistentDataPath + "/AssetBundle/" + platFormName;

        serverAssetBundleRootPath = "http://smartkeygroup.cc/AssetBundle/" + platFormName;

        valueSlider = transform.FindChild("Slider").GetComponent<Slider>();

        valueTxt = transform.FindChild("Text").GetComponent<Text>();
    }

    void Start ()
    {
        //先将StreamAssets下的AssetBundle拷贝到Application.persistentDataPath下
        streamAssetbundles.Clear();

        updateAssetState = UpdateAssetState.Copy2SandBox;

        //获得需要拷贝的所有AssetBundle的文件名list
        GetStreamAssetBundles(streamAssetBundleRoot);

        //如果沙盒目录下没有版本文件,说明是第一次打开游戏,那么把StreamAssets下的AssetBundle拷贝到Application.persistentDataPath下
        if(!File.Exists(sandBoxAssetBundleRoot + "/" + "Version.txt"))
        {
            StartCoroutine(StartCopy2SandBoxAssetBundle());
        }
        else//如果有版本文件先检查StreamAssets下的Version的版本号是否大于沙盒目录下的,如果大就覆盖
        {
            StartCoroutine(CompareStreamAssetsWithSandboxVersion());
        }
    }

    /// <summary>
    /// 比较StreamAsset和本地沙盒中的Version.txt
    /// </summary>
    /// <returns></returns>
    IEnumerator CompareStreamAssetsWithSandboxVersion()
    {
        string url = streamAssetBundleRoot + "/" + "Version.txt";

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            url = "file:///" + url;
        }
        else
        {
            url = "file://" + url;
        }
        WWW www = new WWW(url);

        yield return www;

        if(!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield break;
        }

        string sandBoxVersion = File.ReadAllText(sandBoxAssetBundleRoot + "/" + "Version.txt");

        if(CheckVersion(www.text, sandBoxVersion))
        {
            StartCoroutine(StartCopy2SandBoxAssetBundle());
        }
        else
        {
            sliderProgress = 100;
            streamAssetbundles.Clear();
        }
    }

    /// <summary>
    /// 从服务器上下载版本文件进行比对,根据比对结果来更新资源
    /// </summary>
    void DownLoadAssetBundleFormServer()
    {
        //1先检查哪些AssetBundle需要更新
        //先从服务器上下载版本文件

        string versionFileServerUrl = serverAssetBundleRootPath + "/Version.txt";

        Action<WWW> loadServerVersionFileEndCallBack = (request) =>
        {
            string localVersionFile = sandBoxAssetBundleRoot + "/Version.txt";

            if (!File.Exists(localVersionFile))
            {
                Debug.LogError("Fatal Error Local Version File is Not Exsits");
                return;
            }

            string localVersion = File.ReadAllText(localVersionFile);

            //如果服务器上的版本文件版本号大于本地版本文件的版本号就要从服务器上更新AssetBundle
            if (CheckVersion(request.text, localVersion))
            {
                //用于覆盖本地的版本文件
                serverVersionData = request.bytes;
                //在更新AssetBundle之前需要先下载AssetBundle的总的Hash码校验文件
                string serverAssetBundleManifestFile = serverAssetBundleRootPath + "/" + platFormName;

                //开始下载
                StartCoroutine(DownLoadFile(serverAssetBundleManifestFile, OnDownLoadAssetBundleManifestFileCallBack));
            }
            else//不需要更新
            {
                sliderProgress = 100;
            }
        };

        StartCoroutine(DownLoadFile(versionFileServerUrl, loadServerVersionFileEndCallBack));
    }

    /// <summary>
    /// 下载总的Hash码校验文件完毕回调
    /// </summary>
    /// <param name="wwwRequest"></param>
    private void OnDownLoadAssetBundleManifestFileCallBack(WWW wwwRequest)
    {
        //获得服务器上的校验文件的AssetBundle
        AssetBundle serverManifestLoader = wwwRequest.assetBundle;

         //获得manifest文件
        AssetBundleManifest[] serverABmainfests = serverManifestLoader.LoadAllAssets<AssetBundleManifest>();

        //获得服务器上的所有AssetBundle文件名称
        string[] serverAssetBundleNames = serverABmainfests[0].GetAllAssetBundles();

        serverAssetBundleHashs.Clear();

        //获得服务器上所有AssetBundle文件对应的Hash码
        for (int i = 0; i < serverAssetBundleNames.Length; i++)
        {
            string bundleName = serverAssetBundleNames[i];
            serverAssetBundleHashs.Add(bundleName, serverABmainfests[0].GetAssetBundleHash(bundleName));
        }

        serverManifestLoader.Unload(true);

        string localAssetBundleManifestFile = sandBoxAssetBundleRoot + "/" + platFormName;

        //加载本地校验文件的AssetBundle
        AssetBundle localManifestLoader = AssetBundle.LoadFromFile(localAssetBundleManifestFile);

        //获得manifest文件
        AssetBundleManifest[] localABmainfests = localManifestLoader.LoadAllAssets<AssetBundleManifest>();

        //获得本地的所有AssetBundle文件名称
        string[] localAssetBundleNames = localABmainfests[0].GetAllAssetBundles();

        localAssetBundleHashs.Clear();

        //获得本地所有AssetBundle文件对应的Hash码
        for (int i = 0; i < localAssetBundleNames.Length; i++)
        {
            string bundleName = localAssetBundleNames[i];
            localAssetBundleHashs.Add(bundleName, localABmainfests[0].GetAssetBundleHash(bundleName));
        }

        localManifestLoader.Unload(true);

        //根据服务器上的AssetBundle的Hash码和本地AssetBundle的Hash码来检查需要更新多少个AssertBudle并存放在needDownLoadAssetNames里
        CheckNeedDownLoadAssetBundle(serverAssetBundleHashs, localAssetBundleHashs);

        maxDownLoadNum = needDownLoadAssetNames.Count;

        //需要更新
        if (maxDownLoadNum > 0)
        {
            //用于覆盖本地的ConfigAssetBundleData
            serverConfigAssetBundleData = wwwRequest.bytes;

            sliderProgress = 0;
            downLoadAssetBundleNum = 0;
            StartCoroutine(DownLoadFile(serverAssetBundleRootPath + "/" + needDownLoadAssetNames[0], DownLoadServerAssetBundle));
        }
        else//不需要更新
        {
            sliderProgress = 100;
        }
    }

    /// <summary>
    /// 下载需要更新的AssetBundle文件
    /// </summary>
    /// <param name="request"></param>
    private void DownLoadServerAssetBundle(WWW request)
    {
        downLoadAssetBundleNum++;

        float num = 100.0f / maxDownLoadNum;

        if (downLoadAssetBundleNum == maxDownLoadNum)
        {
            sliderProgress = 100;
        }
        else
        {
            //界面进度条显示
            sliderProgress = (int)(downLoadAssetBundleNum * num);
        }

        string curDownLoadAssetName = needDownLoadAssetNames[0];

        string destWriteFileFullName = sandBoxAssetBundleRoot + "/" + curDownLoadAssetName;

        string destWritePath = "";

        string[] bundleNameDetails = curDownLoadAssetName.Split('/');

        string bundleName = string.Empty;

        if(bundleNameDetails.Length >= 2)
        {
            bundleName = bundleNameDetails[bundleNameDetails.Length - 1];
            destWritePath = destWriteFileFullName.Substring(0, destWriteFileFullName.Length - bundleName.Length);

            if (!Directory.Exists(destWritePath))
                Directory.Exists(destWritePath);
        }

        if (File.Exists(destWriteFileFullName))
            File.Delete(destWriteFileFullName);

        File.WriteAllBytes(destWriteFileFullName, request.bytes);

        needDownLoadAssetNames.RemoveAt(0);

        if(needDownLoadAssetNames.Count > 0)
        {
            StartCoroutine(DownLoadFile(serverAssetBundleRootPath + "/" + needDownLoadAssetNames[0], DownLoadServerAssetBundle));
        }
    }

    /// <summary>
    /// 根据服务器上的AssetBundle的hash和本地的AssetBundle的hash来检查需要更新哪些AssetBundle
    /// </summary>
    /// <param name="serverHashs"></param>
    /// <param name="localHashs"></param>
    private void CheckNeedDownLoadAssetBundle(Dictionary<string,Hash128> serverHashs,Dictionary<string, Hash128> localHashs)
    {
        needDownLoadAssetNames.Clear();

        foreach (string k1 in serverHashs.Keys)
        {
            //首先检查服务端和客户端都有的AssetBundle
            if(localHashs.ContainsKey(k1))
            {
                foreach (string k2 in localHashs.Keys)
                {
                    if (k1 == k2)
                    {
                        //如果hash不一样就需要去下载服务器上的AssetBundle覆盖到本地
                        if (serverHashs[k1].ToString() != localHashs[k2].ToString())
                            needDownLoadAssetNames.Add(k1);
                    }
                }
            }
            else//如果本地没有这个AssetBundle,说明这是新增的AssetBundle,直接需要下载
            {
                needDownLoadAssetNames.Add(k1);
            }
        }
    }

    /// <summary>
    /// 从服务器上下载文件
    /// </summary>
    /// <param name="url"></param>
    /// <param name="completeCallback"></param>
    /// <returns></returns>
    IEnumerator DownLoadFile(string url,Action<WWW> completeCallback = null)
    {
        WWW www = new WWW(url);

        yield return www;

        if(!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            yield break;
        }

        if(www.isDone)
        {
            if (completeCallback != null)
                completeCallback(www);
        }

        www.Dispose();
        www = null;
    }

    /// <summary>
    /// 版本号对比,如StreamAssets文件夹下的Version.txt和本地沙盒路径下的Version.txt对比或者服务器上的Version和本地沙盒目录下的Version.txt对比
    /// </summary>
    /// <param name="serverVersion"></param>
    /// <param name="localVersion"></param>
    /// <returns></returns>
    bool CheckVersion(string serverVersion,string localVersion)
    {
        string[] serverVersionDetail = serverVersion.Split('.');

        string[] localVersionDetail = localVersion.Split('.');

        int serverBigVersion = int.Parse(serverVersionDetail[0]);

        int serverSmallVersion = int.Parse(serverVersionDetail[1]);

        int localBigVersion = int.Parse(localVersionDetail[0]);

        int localSmallVersion = int.Parse(localVersionDetail[1]);

        if (serverBigVersion > localBigVersion)
            return true;
        else
        {
            if(serverBigVersion == localBigVersion)
            {
                if (serverSmallVersion > localSmallVersion)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 开始把AssetBundle拷贝到沙盒目录中
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCopy2SandBoxAssetBundle()
    {
        sliderProgress = 0;

        foreach (string key in streamAssetbundles.Keys)
        {
            yield return Copy2SandBoxAssetBundlePath(key, streamAssetbundles[key], streamAssetbundles.Count);
        }
    }

    /// <summary>
    /// 拷贝AssetBundle到沙盒目录
    /// </summary>
    /// <param name="url">AssetBundle在StreamAssets文件夹下的完整路径</param>
    /// <param name="fileName">AssetBundle名称</param>
    /// <param name="AllResCount">所有需要拷贝的AssetBundle数量</param>
    /// <returns></returns>
    IEnumerator Copy2SandBoxAssetBundlePath(string url,string fileName,int AllResCount)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            url = "file:///" + url;
        }
        else
        {
            url = "file://" + url;
        }

        WWW www = new WWW(url);

        yield return www;

        if(!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            yield break;
        }

        copy2SandboxFileNum++;

        float num = 100.0f / AllResCount;

        if(copy2SandboxFileNum == AllResCount)
        {
            sliderProgress = 100;
        }
        else
        {
            //界面进度条显示
            sliderProgress = (int)(copy2SandboxFileNum * num);
        }

        byte[] data = www.bytes;

        string filePath = "";

        //如果该AB文件不是总的配置文件的AssetBundle
        if (fileName != platFormName && fileName != platFormName + ".manifest")
        {
            //获取该AB文件的url中对应平台(如Android,IOS)的后面的部分,以便检测是否还有文件夹路径
            string subUrl = url.Substring(url.LastIndexOf(platFormName) + platFormName.Length + 1);

            //获取要拷贝到沙盒路径下的完整路径,如果该AB文件的平台路径后还有文件夹那么subUrl.Substring(0, subUrl.Length - fileName.Length)就是获取该文件夹的写法
            filePath = sandBoxAssetBundleRoot + "/" + subUrl.Substring(0, subUrl.Length - fileName.Length);
            
            if(!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            //得到最终的要写入的文件路径
            filePath = filePath + fileName;
        }
        else//如果是该AB文件是总配置文件只需要将该AB文件写在平台路径后即可
        {
            if (!Directory.Exists(sandBoxAssetBundleRoot))
                Directory.CreateDirectory(sandBoxAssetBundleRoot);

            filePath = sandBoxAssetBundleRoot + "/" + fileName;
        }

        File.WriteAllBytes(filePath, data);
    }

    /// <summary>
    /// 获得一个文件夹下的所有文件数量
    /// </summary>
    /// <param name="dirInfo"></param>
    /// <returns></returns>
    static int GetFilesCount(DirectoryInfo dirInfo)
    {
        int totalFile = 0;
        totalFile += dirInfo.GetFiles().Length;
        foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
        {
            totalFile += GetFilesCount(subdir);
        }
        return totalFile;
    }

    /// <summary>
    /// 获得StreamAsset目录下所有AssetBundle
    /// </summary>
    /// <param name="srcPath"></param>
    void GetStreamAssetBundles(string srcPath)
    {
        try
        {
            string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);

            foreach (string fileFullNam in fileList)
            {
                if (System.IO.Directory.Exists(fileFullNam))
                    GetStreamAssetBundles(fileFullNam);
                else
                {
                    if (!fileFullNam.EndsWith(".meta"))
                    {
                        string fileName = Path.GetFileName(fileFullNam);
                        streamAssetbundles.Add(fileFullNam, fileName);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
