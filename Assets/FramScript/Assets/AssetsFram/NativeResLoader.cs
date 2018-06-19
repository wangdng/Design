using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void NativeResCallback(NativeResCallbackNode node);

public class NativeResCallbackNode
{
    public ushort backMsgid;

    public string sceneName;

    public string bundleName;

    public string ABName;

    public bool isSingle;

    public NativeResCallbackNode nextValue;

    public NativeResCallback callback;


    public NativeResCallbackNode(string sceneName, string bundleName, string ABName, bool isSingle, ushort backMsgid, NativeResCallback tmpCallBack, NativeResCallbackNode tmpNode)
    {
        this.backMsgid = backMsgid;
        this.bundleName = bundleName;
        this.ABName = ABName;
        this.isSingle = isSingle;
        this.sceneName = sceneName;
        this.callback = tmpCallBack;
        this.nextValue = tmpNode;
    }


    public void Dispose()
    {
        nextValue = null;

        callback = null;

        bundleName = null;
        ABName = null;

        sceneName = null;
        callback = null;
        nextValue = null;
    }
}


public class NativeResCallBackNodeManager
{
    Dictionary<string, NativeResCallbackNode> manager = null;

    public NativeResCallBackNodeManager()
    {
        manager = new Dictionary<string, NativeResCallbackNode>();
    }

    //加入
    public void AddBundle(string BundleName, NativeResCallbackNode curNode)
    {
        if(manager.ContainsKey(BundleName))
        {
            NativeResCallbackNode node = manager[BundleName];

            while(node.nextValue != null)
            {
                node = node.nextValue;
            }

            node.nextValue = curNode;
        }
        else
        {
            manager.Add(BundleName, curNode);
        }
    }

    //释放
    public void Dispose(string BundleName)
    {
        if (manager.ContainsKey(BundleName))
        {
            NativeResCallbackNode tmpNode = manager[BundleName];

            while(tmpNode.nextValue != null)
            {
                NativeResCallbackNode curNode = tmpNode;
                tmpNode = tmpNode.nextValue;
                curNode.Dispose();
            }

            tmpNode.Dispose();

            manager.Remove(BundleName);
        }
    }

    public void CallBackRes(string BundleName)
    {
        if(manager.ContainsKey(BundleName))
        {
            NativeResCallbackNode curNode = manager[BundleName];

            do
            {
                curNode.callback(curNode);

                curNode = curNode.nextValue;
            }
            while (curNode != null);
        }
    }
}

public class NativeResLoader : AssetBase 
{
    //接收消息
    public override void ProcessEvent(MsgBase RecMsg)
    {
        switch (RecMsg.MsgId)
        {
            case (ushort)AssetEvent.ReleaseSingleABFile:
                {
                    HunkAssetMsg tmpMsg = (HunkAssetMsg)RecMsg;

                    ILoadManager.instance.UnLoadSingleABFile(tmpMsg.sceneName, tmpMsg.bundleName, tmpMsg.ABName);
                }
                break;

            case (ushort)AssetEvent.ReleaseABFiles:
                {
                    HunkAssetMsg tmpMsg = (HunkAssetMsg)RecMsg;

                    ILoadManager.instance.UnLoadABFiles(tmpMsg.sceneName, tmpMsg.bundleName);
                }

                break;

            case (ushort)AssetEvent.ReleaseAllABFiles:
                {
                    HunkAssetMsg tmpMsg = (HunkAssetMsg)RecMsg;

                    ILoadManager.instance.UnLoadAllABFiles(tmpMsg.sceneName);
                }

                break;

            case (ushort)AssetEvent.ReleaseBundle:
                {
                    HunkAssetMsg tmpMsg = (HunkAssetMsg)RecMsg;

                    ILoadManager.instance.UnLoadSingleBundle(tmpMsg.sceneName, tmpMsg.bundleName);
                }

                break;

            case (ushort)AssetEvent.ReleaseAllBundle:
                {
                    HunkAssetMsg tmpMsg = (HunkAssetMsg)RecMsg;

                    ILoadManager.instance.UnLoadAllBundle(tmpMsg.sceneName);
                }

                break;

            case (ushort)AssetEvent.ReleaseAllBundleAndABFiles:
                {
                    HunkAssetMsg tmpMsg = (HunkAssetMsg)RecMsg;

                    ILoadManager.instance.UnLoadAllBundleAndABFils(tmpMsg.sceneName);
                }

                break;

                //请求加载
            case (ushort)AssetEvent.HunkRes:
                {
                    HunkAssetMsg tmpMsg = (HunkAssetMsg)RecMsg;

                    GetResources(tmpMsg.sceneName, tmpMsg.bundleName, tmpMsg.ABName, tmpMsg.isSingle, tmpMsg.backMsgid);
                }

                break;
                //加载一个Ab文件完成后要干些什么
            case (ushort)AssetEvent.HunkResBackMsg:
                {
                    HunkAssetBackMsg tmpMsg = (HunkAssetBackMsg)RecMsg;

                    Debug.Log("加载完成的AB文件名为:" + ((Texture2D)tmpMsg.value[0]).name);
                }

                break;
        }
    }


    HunkAssetBackMsg backMsg = null;

    HunkAssetBackMsg releaseMsg
    {
        get
        {
            if(backMsg == null)
            {
                backMsg = new HunkAssetBackMsg();
            }

            return backMsg;
        }
    }

    NativeResCallBackNodeManager callBackManager = null;

    NativeResCallBackNodeManager CbManager
    {
        get
        {
            if (callBackManager == null)
            {
                callBackManager = new NativeResCallBackNodeManager();
            }

            return callBackManager;
        }
    }


    void Awake()
    {
        //注册消息
        msgIds = new ushort[]
        {
            (ushort)AssetEvent.ReleaseSingleABFile,
            (ushort)AssetEvent.ReleaseABFiles,
            (ushort)AssetEvent.ReleaseAllABFiles,
            (ushort)AssetEvent.ReleaseBundle,
            (ushort)AssetEvent.ReleaseAllBundle,
            (ushort)AssetEvent.ReleaseAllBundleAndABFiles,
            (ushort)AssetEvent.HunkRes,
            (ushort)AssetEvent.HunkResBackMsg,
        };

        RegisterSelf(this, msgIds);

        //AssetEvent.HunkResBackMsg即是加载完该AB文件后得到该AB文件要干些什么的消息
        HunkAssetMsg tmpMsg = new HunkAssetMsg("scene01", "Load", "GlobalUIAtlas(rgba).png", true, (ushort)AssetEvent.HunkResBackMsg, (ushort)AssetEvent.HunkRes);

        SendMessage(tmpMsg);
    }


    public void SendToBackMsg(NativeResCallbackNode tmpNode)
    {
        if(tmpNode.isSingle)
        {
            Object obj = ILoadManager.instance.GetSingleABFile(tmpNode.sceneName, tmpNode.bundleName, tmpNode.ABName);
            this.releaseMsg.Change(tmpNode.backMsgid, obj);
        }
        else
        {
            Object[] objs = ILoadManager.instance.GetABFiles(tmpNode.sceneName, tmpNode.bundleName, tmpNode.ABName);
            this.releaseMsg.Change(tmpNode.backMsgid, objs);
        }

        SendMessage(releaseMsg);
    }

    public void LoadProgress(string bundleName,float progress)
    {
       
    }

    public void LoadABFinish(string bundleName)
    {
        CbManager.CallBackRes(bundleName);
        CbManager.Dispose(bundleName);
    }

    //加载一个AssetBudle到CbManager里来管理
    //bundleName是Record配置文件中的第一部分,如load,在LoadAsset会转换成配置文件中的第二部分如scene01/load.ld
    //ABName是这个AssetBudle中具体哪一个AB文件名,一个AssetBudle可能含有一个或多个AB文件,因为都是放在相同的文件夹下进行打包的,如Assets\Art\Scenes\Scene01文件夹打包出来的所有文件都是包含在scene01/load.ld这AssetBundle种
    //isSingle表示该AssetBudle中是否只含有一个AB文件
    //backMsgid表示加载完成一个AB文件后要干些什么事直接发消息到ProcessEvent
    public void GetResources(string sceneName,string bundleName,string ABName, bool isSingle, ushort backMsgid)
    {
        //该AssetBundle还没有进行加载
        if (!ILoadManager.instance.IsLoadAssetBundle(sceneName, bundleName))
        {
            //进行加载
            ILoadManager.instance.LoadAsset(sceneName, bundleName, LoadProgress, LoadABFinish);

            //scene01/load.ld
            string ReateBundleName = ILoadManager.instance.GetBundleReateName(sceneName, bundleName);

            if (ReateBundleName != null)
            {
                //加载完成后再回掉函数SendToBackMsg发送要干嘛的消息到ProcessEvent
                NativeResCallbackNode node = new NativeResCallbackNode(sceneName, ReateBundleName, ABName, isSingle, backMsgid,SendToBackMsg, null);

                CbManager.AddBundle(ReateBundleName, node);
            }
            else
            {
                Debug.LogError("dont have bundleName:" + bundleName);
            }
        }
        else
        {
            string ReateBundleName = ILoadManager.instance.GetBundleReateName(sceneName, bundleName);

            if (isSingle)
            {
                Object obj = ILoadManager.instance.GetSingleABFile(sceneName, ReateBundleName, ABName);

                this.releaseMsg.Change(backMsgid, obj);
            }
            else
            {
                Object[] objs = ILoadManager.instance.GetABFiles(sceneName, ReateBundleName, ABName);

                this.releaseMsg.Change(backMsgid, objs);
            }
            //因为缓存中已经存在该AB文件,所以这里直接发送加载该AB文件后要干些什么事的消息到ProcessEvent
            SendMessage(releaseMsg);
        }
    }
}
