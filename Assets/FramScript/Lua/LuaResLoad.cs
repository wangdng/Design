using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class LuaResCallbackNode
{
    public string AbName;

    public string bundleName;

    public string sceneName;

    public bool isSingle;

    public LuaResCallbackNode next;

    public LuaFunction luaFunc;

    public LuaResCallbackNode(string tmpAbName,string tmpBundleName,string tmpSceneName,bool single,LuaFunction Lfuc, LuaResCallbackNode tmpNode)
    {
        this.AbName = tmpAbName;
        this.bundleName = tmpBundleName;
        this.sceneName = tmpSceneName;
        this.isSingle = single;

        this.luaFunc = Lfuc;

        this.next = tmpNode;
    }

    public void Dispose()
    {
        this.AbName = null;
        this.bundleName = null;
        this.sceneName = null;

        this.luaFunc.Dispose();

        this.next = null;
    }
}

public class LuaResCallBackNodeMangaer
{
    Dictionary<string, LuaResCallbackNode> manager = null;

    public LuaResCallBackNodeMangaer()
    {
        manager = new Dictionary<string, LuaResCallbackNode>();
    }

    public void AddBundle(string bundleName, LuaResCallbackNode tmpNode)
    {
        if (manager.ContainsKey(bundleName))
        {
            LuaResCallbackNode curNode = manager[bundleName];

            while (curNode.next != null)
            {
                curNode = curNode.next;
            }

            curNode.next = tmpNode;
        }
        else
        {
            manager.Add(bundleName, tmpNode);
        }
    }

    public void Dispose(string bundleName)
    {
        if (manager.ContainsKey(bundleName))
        {
            LuaResCallbackNode node = manager[bundleName];

            while(node.next != null)
            {
                LuaResCallbackNode tmpNode = node;

                node = node.next;

                tmpNode.Dispose();
            }

            node.Dispose();
            manager.Remove(bundleName);
        }
    }

    public void LuaCallBackRes(string BundleName)
    {
        if(manager.ContainsKey(BundleName))
        {
            LuaResCallbackNode node = manager[BundleName];

            do
            {
                if (node.isSingle)
                {
                    //每次加载AB文件后都回调到这里后才进行真正的AB文件的实例化
                    object tmpObj = ILoadManager.instance.GetSingleABFile(node.sceneName, node.bundleName, node.AbName);

                    node.luaFunc.Call(node.sceneName, node.bundleName, node.AbName, tmpObj);
                }
                else
                {
                    object[] tmpObjs = ILoadManager.instance.GetABFiles(node.sceneName, node.bundleName, node.AbName);
                    node.luaFunc.Call(node.sceneName, node.bundleName, node.AbName, tmpObjs);
                }

                node = node.next;
            }
            while (node != null);
        }
    }
}

public class LuaResLoad
{
    private LuaResCallBackNodeMangaer LResManager = null;

    public LuaResCallBackNodeMangaer LResCallBackManager
    {
        get
        {
            if (LResManager == null)
            {
                LResManager = new LuaResCallBackNodeMangaer();
            }

            return LResManager;
        }
    }

	public static LuaResLoad mIns;

	public static LuaResLoad Instance
	{
		get
		{
			if (mIns == null)
				mIns = new LuaResLoad ();

			return mIns;
		}
	}

    private void LoadProgress(string bundleName, float progress)
    {
        
    }

    private void LoadCompleteCallBack(string bundleName)
    {
        LResCallBackManager.LuaCallBackRes(bundleName);
        LResCallBackManager.Dispose(bundleName);
    }

    //目前看来lua直接调用C#方法好像只能调用静态方法,否则会多一个检测有没有实例化对象的步骤,从而导致参数检测的时候会多一个
    //直接调用静态方法就不会多一个参数检测
    //调用这个静态方法lua里只需要传4个参数
	public static void GetStaticRes(string sceneName, string bundleName, string ABName, bool isSingle)
	{
		Instance.GetResources (sceneName, bundleName, ABName, isSingle);
	}

    //调用这个非静态方法就要传5个参数,否则会报错
	private void GetResources(string sceneName, string bundleName, string ABName, bool isSingle)
    {
        //lua好像没办法直接将LuaFunction类型的方法直接用参数传过来,所以这里只能通过C#来调用
        LuaFunction luaFunc = LuaClient.GetMainState().GetFunction("LAssetBundleLoader.AssetBundleLoadFinishCallBack");
        //该AssetBundle还没有进行加载
        if (!ILoadManager.instance.IsLoadAssetBundle(sceneName, bundleName))
        {
            //scene01/test.unity3d
            string ReateBundleName = sceneName + "/" + bundleName + ".unity3d";

            if (ReateBundleName != null)
            {
                LuaResCallbackNode node = new LuaResCallbackNode(ABName, ReateBundleName, sceneName, isSingle, luaFunc, null);
                LResCallBackManager.AddBundle(ReateBundleName, node);
            }
            else
            {
                Debug.LogError("dont have bundleName:" + bundleName);
            }

            //进行加载
            ILoadManager.instance.LoadAsset(sceneName, bundleName, LoadProgress, LoadCompleteCallBack);
        }
        else
        {
            //获取带场景名的包名
            string ReateBundleName = ILoadManager.instance.GetBundleReateName(sceneName, bundleName);

            if (isSingle)
            {
                Object obj = ILoadManager.instance.GetSingleABFile(sceneName, ReateBundleName, ABName);

                luaFunc.Call(sceneName, ReateBundleName, ABName, obj);
            }
            else
            {
                Object[] objs = ILoadManager.instance.GetABFiles(sceneName, ReateBundleName, ABName);
                luaFunc.Call(sceneName, ReateBundleName, ABName, objs);
            }
        }
    }

    public void UnLoadSingleABFile(string sceneName, string bundleName, string ABName)
    {
        ILoadManager.instance.UnLoadSingleABFile(sceneName, bundleName, ABName);
    }


    public void UnLoadABFiles(string sceneName, string bundleName)
    {
        ILoadManager.instance.UnLoadABFiles(sceneName, bundleName);
    }

    public void UnLoadAllABFiles(string sceneName)
    {
        ILoadManager.instance.UnLoadAllABFiles(sceneName);
    }

    public void UnLoadSingleBundle(string sceneName, string bundleName)
    {
        ILoadManager.instance.UnLoadSingleBundle(sceneName, bundleName);
    }

    public void UnLoadAllBundle(string sceneName)
    {
        ILoadManager.instance.UnLoadAllBundle(sceneName);
    }

     public void UnLoadAllBundleAndABFils(string sceneName)
    {
        ILoadManager.instance.UnLoadAllBundleAndABFils(sceneName);
    }
}
