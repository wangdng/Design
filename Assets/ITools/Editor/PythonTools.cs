using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Diagnostics;

public class PythonTools
{
    [MenuItem("ITools/PythonTest")]
    public static void PythonTest()
    {
        string rootPath = Application.dataPath;

        int tmp = rootPath.IndexOf("Assets");

        //我们的工作目录是Assets外一层的路径
        string workRoot = rootPath.Substring(0, tmp);

        string PythonToolsPath = workRoot + "ITools/Python/";

        Process pro = Process.Start(PythonToolsPath + "GenLuaJIT.bat");

        pro.WaitForExit();
    }

}
