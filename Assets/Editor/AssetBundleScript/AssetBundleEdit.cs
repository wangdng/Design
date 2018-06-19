using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using System.Diagnostics;

public class AssetBundleEdit : EditorWindow
{
    static AssetBundleEdit s_window;

    static bool IsSureBuild = false;

    static bool IsUpLoadFtpServer = false;

    string FtpServerAddress = "";

    [MenuItem("Tool/BuildAssetBunlde")]
    static void StartWindow()
    {
        if (s_window != null)
            EditorWindow.DestroyImmediate(s_window);

        s_window = EditorWindow.GetWindow<AssetBundleEdit>(false, "一键打包工具", true);
    }

    public static void AssetBundleBuilder()
    {
        string outPath = null;

        outPath = IPathTools.GetAssetBundlePath();

        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        //BuildAssetBundleOptions常用选项说明:
        //BuildAssetBundleOptions.None 默认选项,打包的时候Unity会自动使用LZMA算法进行压缩AB包,这种压缩方式是压缩率最高的,适合从服务器进行下载,但是缺点是它解压比较慢,所以在使用的时候加载就比较慢了
        //还有一个缺点是它是全体解压(一个AB包里包含了ABC三个assetbundle,如果我仅仅只想使用B,那么也必须把ABC三者都解压了才能使用),一旦解压完成后系统还会自动使用LZ4的方式将其重新压缩放在缓存中(如果你没有调用unload的话),因为这样可以节省磁盘空间
        //BuildAssetBundleOptions.ChunkBasedCompression 使用LZ4算法进行压缩,这种压缩方式压缩率没有那么高,但是它解压快(基本上跟没压缩一样),而且它还支持单个assetbundle解压,不必全体解压
        //BuildAssetBundleOptions.UncompressedAssetBundle 不压缩,如果选择不压缩的方式,最好自己选用第三方压缩工具将其压缩然后再上传服务器进行更新

        //Unity5的AssetBunlde会自动将打好的AB包放进outPath里,只需要在要打包的资源的属性面板上设置好标记,如果标记里没有路径(没有"/"),那么打出包后就直接把AB包和配置放在outPath根目录
        //如果标记里含有路径(如"load/test"),那么Unity会在outPath里自动建一个新的文件夹load,然后将打好的AB以test命名放在load里,还有该AB包的配置文件(mainfest)也会放在这个文件夹下

        //打AB包之前先将AB包删除
        DelectDir(outPath);

#if UNITY_ANDROID
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
#elif UNITY_IOS
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, BuildTarget.iOS))
#elif UNITY_STANDALONE_WIN
          BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
#elif UNITY_STANDALONE_OSX
         BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXIntel);
#endif
        //打包策略:最好把UI引用到的资源(图集,mat,Shader)各打进一个AB包(图集,mat,Shader各放在一个文件夹下(文件夹名称用小写,因为Unity标记assetbundle时始终是小写)然后使用MarkAssetBundle进行标记),然后把各个UI(分别放在一个文件夹下)分别打包成一个不同的包(方便更新),这样资源只会打包一份,不会使得将资源重复打包

        CreateOrUpdateVersion();

        CopyConfigToStreaming();

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 创建或者更新版本文件
    /// </summary>
    static void CreateOrUpdateVersion()
    {
        string versionFilleFullName = Application.dataPath + "/Art/Scenes/Version.txt";

        if(!File.Exists(versionFilleFullName))
        {
            FileStream fs = new FileStream(versionFilleFullName, FileMode.Create);

            StreamWriter sw = new StreamWriter(fs);

            string version = "1.0";

            sw.Write(version);

            sw.Close();
            fs.Close();
        }
        else
        {
            string version = File.ReadAllText(versionFilleFullName);

            string[] versionDetail = version.Split('.');

            int bigVersionNO = int.Parse(versionDetail[0]);

            int smallVerisonNO = int.Parse(versionDetail[1]);

            if (smallVerisonNO < 9)
                smallVerisonNO++;
            else
                smallVerisonNO = 0;

            string newVersion = "";

            if (smallVerisonNO == 0)
            {
                bigVersionNO++;
            }

            newVersion = bigVersionNO + "." + smallVerisonNO;

            FileStream fs = new FileStream(versionFilleFullName, FileMode.Open);

            StreamWriter sw = new StreamWriter(fs);

            sw.Write(newVersion);

            sw.Close();
            fs.Close();
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("标记AssetBundle", GUILayout.Width(200)))
                    MarkAssetBundle();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            {
                IsSureBuild = EditorGUILayout.ToggleLeft("确认要进行AssetBundle打包吗?", IsSureBuild, GUILayout.Width(200));
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("开始打包", GUILayout.Width(200)))
            {
                if(IsSureBuild)
                {
                    AssetBundleBuilder();
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "必须先勾选确认打包复选框","确定");
                }
            }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            {
                IsUpLoadFtpServer = EditorGUILayout.ToggleLeft("确定上传Ftp资源服务器吗?", IsUpLoadFtpServer, GUILayout.Width(200));
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("开始上传", GUILayout.Width(200)))
            {
                if(IsUpLoadFtpServer)
                {
                    UpLoadFtpServer();
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "必须先勾选确认上传ftp服务器复选框", "确定");
                }
            }
        }
    }

    /// <summary>
    /// 上传AssetBundle到ftp服务器
    /// </summary>
    static void UpLoadFtpServer()
    {
        //第一步先将StreamAssets文件夹下的所有AssetBundle拷贝到工程中和Assets目录同级的目录
        string ProjectAssetBundlePath = Application.dataPath.Replace("/Assets", "/AssetBundle");

        if(Directory.Exists(ProjectAssetBundlePath))
        {
            //首先删除该文件夹下的所有文件
            DelectDir(ProjectAssetBundlePath);
            //然后再删除该文件夹
            Directory.Delete(ProjectAssetBundlePath);
        }

        //创建新的文件夹
        Directory.CreateDirectory(ProjectAssetBundlePath);

        string assetBundlePath = Application.streamingAssetsPath + "/AssetBundle/";

        if(!Directory.Exists(assetBundlePath))
        {
            UnityEngine.Debug.LogError("AssetBundle Directory Is Not Found");
            return;
        }

        //拷贝工程中的StreamAssets/AssetBundle文件夹下的所有AB文件到与Assets同级的AssetBundle文件夹下
        CopyDirectory(assetBundlePath, ProjectAssetBundlePath);

        AssetBundleFtpIno ftpInfo = new AssetBundleFtpIno("ftp://120.132.11.232/", "wangdong_508", "wangdong508", true);

        ftpInfo.SrcDirPath = ProjectAssetBundlePath;

        string arguments = string.Format("{0},{1},{2},{3},{4}", ftpInfo.SrcDirPath, ftpInfo.FtpUrl, ftpInfo.UserName, ftpInfo.Password, ftpInfo.Passive);

        UnityEngine.Debug.Log(arguments);

        string toolPath = string.Format("{0}/Editor/AssetBundleScript/FtpUploadTool.exe", Application.dataPath.TrimEnd('/'));
        Process.Start(toolPath, arguments);
    }

    /// <summary>
    /// 删除一个文件夹下的所有文件
    /// </summary>
    /// <param name="srcPath"></param>
    static void DelectDir(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 拷贝一个文件夹下的所有文件到另外一个文件夹下
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="destPath"></param>
    static void CopyDirectory(string srcPath, string destPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)     //判断是否文件夹
                {
                    if (!Directory.Exists(destPath + "\\" + i.Name))
                    {
                        Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                    }
                    CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                }
                else
                {
                    File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }

    //把标记assetbundle时生成的配置文件拷贝到IPathTools.GetAssetBundlePath()目录下，后面解析用
    static void CopyConfigToStreaming()
    {
        string AssetRootPath = Application.dataPath + "/Art/Scenes";

        DirectoryInfo dir = new DirectoryInfo(AssetRootPath);

        FileSystemInfo[] fileInfos = dir.GetFileSystemInfos();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            FileSystemInfo curFileInfo = fileInfos[i];

            if (curFileInfo.Extension == ".txt")
            {
                string targetPath = Path.Combine(IPathTools.GetAssetBundlePath(), curFileInfo.Name);

                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                File.Copy(curFileInfo.FullName, targetPath);
            }
        }
    }

    public static void MarkAssetBundle()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();

        string AssetRootPath = Application.dataPath + "/Art/Scenes";

        DirectoryInfo dir = new DirectoryInfo(AssetRootPath);

        FileSystemInfo[] fileInfos = dir.GetFileSystemInfos();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            FileSystemInfo curFileInfo = fileInfos[i];

            if (curFileInfo is DirectoryInfo)
            {
                string tmpPath = Path.Combine(AssetRootPath, curFileInfo.Name);

                SceneOverView(tmpPath);
            }
        }

        AssetDatabase.Refresh();
    }



    //找到Scenes下的某个Scene(如Scene01)和记录
    public static void SceneOverView(string ScenePath)
    {
        //这里创建一个record.txt来记录我们要打包资源的名字
        string textFileName = "Record.txt";

        string tmpPath = ScenePath + textFileName;

        FileStream fs = new FileStream(tmpPath, FileMode.OpenOrCreate);

        StreamWriter sw = new StreamWriter(fs);

        Dictionary<string, string> readDic = new Dictionary<string, string>();

        ChangeHead(ScenePath, readDic);

        sw.WriteLine(readDic.Count);

        foreach (string key in readDic.Keys)
        {
            sw.Write(key);
            sw.Write("|");
            sw.Write(readDic[key]);
            sw.Write("\n");
        }

        sw.Close();
        fs.Close();
    }


    //截取相对路径
    //ScenePath是绝对路径(如E://Design Patterns/Design Patterns/Assets/Art/Scenes/Scene01)
    //得到Assets/Art/Scenes/Scene01
    public static void ChangeHead(string fullPath, Dictionary<string, string> readDic)
    {
        int tmpCount = fullPath.LastIndexOf("Assets");

        int tmpLength = fullPath.Length;

        //得到Assets/Art/Scenes/Scene01
        string replacePath = fullPath.Substring(tmpCount, tmpLength - tmpCount);

        UnityEngine.Debug.Log("replacePath:" + replacePath);

        DirectoryInfo dir = new DirectoryInfo(fullPath);

        if (dir != null)
        {
            ListFiles(dir, replacePath, readDic);
        }
        else
        {
            UnityEngine.Debug.LogError("this fullPath is not exit");
        }
    }


    //遍历文件
    public static void ListFiles(FileSystemInfo info, string replacePath, Dictionary<string, string> readDic)
    {
        if(info == null || !info.Exists)
        {
            UnityEngine.Debug.LogError("该文件目录不存在");
            return;
        }

        DirectoryInfo dir = info as DirectoryInfo;

        FileSystemInfo[] files = dir.GetFileSystemInfos();

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo f = files[i] as FileInfo;

            if (f != null)//文件
            {
                ChangeMark(f, replacePath, readDic);
            }
            else//文件夹
            {
                ListFiles(files[i], replacePath, readDic);
            }
        }
    }

    public static string GetBuildPath(FileInfo file, string replacePath)
    {
        string tmpPath = file.FullName;

        //举例说明
        //tmpPath 就是E:\Design Patterns\Design Patterns\Assets\Art\Scenes\Scene01\Load\GlobalUIAtlas(rgba).png
        UnityEngine.Debug.Log("tmpPath:" + tmpPath);


        //从tmpPath中截取到Assets之前
        int assetCount = tmpPath.IndexOf("Assets");


        //replacePath 就是Assets/Art/Scenes\Scene01所以assetCount就到了Load之前
        assetCount += replacePath.Length + 1;

        //nameCount到了最后的文件名GlobalUIAtlas(rgba).png之前
        int nameCount = tmpPath.LastIndexOf(file.Name);

        //GlobalUIAtlas(rgba).png
        UnityEngine.Debug.Log("file.Name:" + file.Name);

        //replacePath 就是Assets/Art/Scenes\Scene01,所以这里到了Scene01之前,"\\"就是"\"
        int tmpCount = replacePath.LastIndexOf("\\");

        //这里就剩一个Scene01
        string scenehead = replacePath.Substring(tmpCount + 1, replacePath.Length - tmpCount - 1);

        //这里就剩一个Scene01
        UnityEngine.Debug.Log("scenehead:" + scenehead);

        //用GlobalUIAtlas(rgba).png之前的字符串数减去Load\GlobalUIAtlas(rgba).png之前的字符串数
        int tmpLength = nameCount - assetCount;

        //如果load文件夹下有文件tmpLength就会大于0,如果==0就表示GlobalUIAtlas(rgba).png是不存在的
        if (tmpLength > 0)
        {
            //说明load文件夹下有文件
            string SubString = tmpPath.Substring(assetCount, tmpPath.Length - assetCount);

            //Load\GlobalUIAtlas(rgba).png
            UnityEngine.Debug.Log("SubString:" + SubString);

            string[] reault = SubString.Split("\\".ToCharArray());

            UnityEngine.Debug.Log("scenehead + reault[0]:" + scenehead + "/" + reault[0]);

            return scenehead + "/" + reault[0];
        }
        else
        {
            UnityEngine.Debug.Log("场景文件:" + scenehead);
            return scenehead;
        }
    }


    public static void ChangeAssetMark(FileInfo file,string markName, Dictionary<string, string> readDic)
    {
        string filePath = file.FullName;

        int assetCount = filePath.IndexOf("Assets");

        string assetName = filePath.Substring(assetCount, filePath.Length - assetCount);

        AssetImporter import = AssetImporter.GetAtPath(assetName);

        import.assetBundleName = markName;

        import.assetBundleVariant =  "unity3d";

        string[] subMark = markName.Split("/".ToCharArray());

        string ModleName = null;

        string ModlePath = markName.ToLower() + "."+"unity3d";

        if (subMark.Length > 1)//非场景的资源
        {
            ModleName = subMark[1];
        }
        else//场景文件
        {
            ModleName = markName;
        }

        if (!readDic.ContainsKey(ModleName))
        {
            readDic.Add(ModleName, ModlePath);
        }
    }


    public static void ChangeMark(FileInfo file, string replacePath, Dictionary<string, string> readDic)
    {
        if (file.Extension == ".meta")
        {
            return;
        }

        string markStr = GetBuildPath(file, replacePath);

        ChangeAssetMark(file, markStr, readDic);
    }
}
