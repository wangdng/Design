using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class IPathTools  
{
    /// <summary>
    /// 获取Record文件的存放路径,电脑上是在 Application.streamingAssetsPath,手机平台是在Application.persistentDataPath
    /// </summary>
    /// <returns></returns>
    public static string GetAssetBundlePath()
    {
        string folderName = GetPlatformFolderName();

        string AllPath = GetAppFilePath() + "/AssetBundle/" + folderName;

        return AllPath;
    }

    public static string GetPlatformFolderName()
    {
#if UNITY_ANDROID
        return "Android";
#elif UNITY_IOS
        return "IOS";
#elif UNITY_STANDALONE_WIN
        return "Windows";
#elif  UNITY_STANDALONE_OSX
        return "OSX";
#endif
    }

    public static string GetAppFilePath()
    {
           return Application.persistentDataPath;
    }
}
