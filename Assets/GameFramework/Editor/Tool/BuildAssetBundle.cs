/*
    Powered By:Jie Xiong,Daan

 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using System.Threading;
using Daan;

// 注意 因为 Windows 的路径分隔符为 '/', Mac OS 系统的路径分隔符号为 '\' 大坑
// 当时是在 Mac 上写的打包方法， 没注意到这个大坑， 后来在 Windows 上使用时发现此坑
// 如果发现打包不成功，可以打印一下代码中有关路径的地方，看看是不是有问题
// 大致就是使用如   pathReplace = path.Replace('\\', '/');   pathReplace = path.Replace('/', '\\');
// 转换下即可
public class BuildAssetBundle : EditorWindow
{
    enum WindowType
    {
        AssetBundle,
        ReplaceSuffix
    }
    private static BuildAssetBundle window;
    private static WindowType windowType;

    void OnGUI()
    {

    }
    private static string suffixName = ".unity3d";
    #region 替换后缀名
    private static string replacePath = "";
    private static string fileSuffix = ".bytes";

    //[MenuItem("插件/替换后缀名")]
    //private static void ReplaceSuffix()
    //{
    //    windowType = WindowType.ReplaceSuffix;

    //    Object[] SelectedAsset = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
    //    if (SelectedAsset != null && SelectedAsset.Length > 0)
    //    {
    //        assetBundleName = SelectedAsset[0].name;
    //        string sourcePath = AssetDatabase.GetAssetPath(SelectedAsset[0]);
    //        sourcePath = GetAssetPath(sourcePath, SelectedAsset[0].name);
    //        sourcePath = sourcePath.Remove(0, sourcePath.IndexOf("/") + 1);
    //        string filesPath = sourcePath.Contains(".") ? sourcePath.Remove(sourcePath.IndexOf("."), sourcePath.Length - sourcePath.IndexOf(".")) : sourcePath;
    //        replacePath = Application.dataPath + "/" + filesPath;

    //        Rect rect = new Rect(Screen.width / 2, Screen.height, 400, 120);
    //        window = (BuildAssetBundle)EditorWindow.GetWindowWithRect(typeof(BuildAssetBundle), rect, true, "Replace Suffix");
    //        window.Show();
    //        window.Repaint();
    //    }
    //    else
    //    {
    //        EditorUtility.DisplayDialog("提示", "至少选择个文件夹啊", "好哒");
    //    }
    //}
    //private void UpdateReplaceSuffixUI()
    //{
    //    replacePath = EditorGUILayout.TextField("替换文件夹路径：", replacePath);
    //    fileSuffix = EditorGUILayout.TextField("替换后缀名：", fileSuffix);

    //    if (GUI.Button(new Rect(160, 90, 80, 20), "替换"))
    //    {
    //        if (assetPath == "")
    //        {
    //            return;
    //        }
    //        if (suffixName == "")
    //        {
    //            return;
    //        }

    //        MyIO.ReplaceSuffix(replacePath);

    //        window.Close();
    //    }
    //}
    #endregion

    #region AssetBundle


    //导出包路径
    private static string AssetBundleOutPsth = "Assets/StreamingAssets";


    //需要打包的资源后缀
    private static Dictionary<string, string> asExtensionDic = new Dictionary<string, string>();

    private static BuildTarget buildTarget;

    ////[MenuItem("Assets/Build AssetBundle")]
    //private static void BuildAssetBundleSource()
    //{
    //    buildsList.Clear();
    //    assetPathList.Clear();

    //    GetDirs(Application.dataPath + "/" + assetPath);

    //    BuildAsset(outPath);
    //}
    #region Window
    [MenuItem("Build Tools/AssetBundle/Windows/Codes And Resources")]
    private static void Window_CodesAndResources()
    {
        buildTarget = BuildTarget.StandaloneWindows;
        BuildAB(true, true);
    }
    [MenuItem("Build Tools/AssetBundle/Windows/Only Codes")]
    private static void Window_OnlyCodes()
    {
        buildTarget = BuildTarget.StandaloneWindows;
        BuildAB(true, false);
    }
    [MenuItem("Build Tools/AssetBundle/Windows/Only Resources")]
    private static void Window_OnlyResources()
    {
        buildTarget = BuildTarget.StandaloneWindows;
        BuildAB(false, true);
    }

    #endregion

    #region Android
    [MenuItem("Build Tools/AssetBundle/Android/Codes And Resources")]
    private static void Android_CodesAndResources()
    {
        buildTarget = BuildTarget.Android;
        BuildAB(true, true);
    }
    [MenuItem("Build Tools/AssetBundle/Android/Only Resources")]
    private static void Android_OnlyResources()
    {
        buildTarget = BuildTarget.Android;
        BuildAB(false, true);
    }
    [MenuItem("Build Tools/AssetBundle/Android/Only Codes")]
    private static void Android_OnlyCodes()
    {
        buildTarget = BuildTarget.Android;
        BuildAB(true, false);
    }
    #endregion

    #region IOS
    [MenuItem("Build Tools/AssetBundle/IOS/Codes And Resources")]
    private static void IOS_CodesAndResources()
    {
        buildTarget = BuildTarget.iOS;
        BuildAB(true, true);
    }
    [MenuItem("Build Tools/AssetBundle/IOS/Only Resources")]
    private static void IOS_OnlyResources()
    {
        buildTarget = BuildTarget.iOS;
        BuildAB(false, true);
    }
    [MenuItem("Build Tools/AssetBundle/IOS/Only Codes")]
    private static void IOS_OnlyCodes()
    {
        buildTarget = BuildTarget.iOS;
        BuildAB(true, false);
    }
    #endregion

    #region WebGL
    [MenuItem("Build Tools/AssetBundle/WebGL/Codes And Resources")]
    private static void WebGL_CodesAndResources()
    {
        buildTarget = BuildTarget.WebGL;
        BuildAB(true, true);
    }
    [MenuItem("Build Tools/AssetBundle/WebGL/Only Resources")]
    private static void WebGL_OnlyResources()
    {
        buildTarget = BuildTarget.WebGL;
        BuildAB(false, true);
    }
    [MenuItem("Build Tools/AssetBundle/WebGL/Only Codes")]
    private static void WebGL_OnlyCodes()
    {
        buildTarget = BuildTarget.WebGL;
        BuildAB(true, false);
    }
    #endregion

    #region Build Selected Codes
    [MenuItem("Assets/Build Tools/AssetBundle/Build Selected Codes/Window")]
    private static void Window_BuildSelectedCodes()
    {
        buildTarget = BuildTarget.StandaloneWindows;
        BuildSelectedCodes();
    }
    [MenuItem("Assets/Build Tools/AssetBundle/Build Selected Codes/Android")]
    private static void Android_BuildSelectedCodes()
    {
        buildTarget = BuildTarget.Android;
        BuildSelectedCodes();
    }
    [MenuItem("Assets/Build Tools/AssetBundle/Build Selected Codes/IOS")]
    private static void IOS_BuildSelectedCodes()
    {
        buildTarget = BuildTarget.iOS;
        BuildSelectedCodes();
    }
    [MenuItem("Assets/Build Tools/AssetBundle/Build Selected Codes/WebGL")]
    private static void WebGL_BuildSelectedCodes()
    {
        buildTarget = BuildTarget.WebGL;
        BuildSelectedCodes();
    }
    #endregion
    private const string constResPath = "AssetBundle";

    private static void BuildSelectedCodes()
    {
        long start = System.DateTime.Now.Ticks;
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel);
        List<string> list = new List<string>();
        if (SelectedAsset != null && SelectedAsset.Length > 0)
        {
            for (int i = 0; i < SelectedAsset.Length; i++)
            {
                list.Add(AssetDatabase.GetAssetPath(SelectedAsset[i]));
            }
        }

        if (BuildCodes(list))
        {
            long end = System.DateTime.Now.Ticks;
            Debug.Log("打包Codes共耗时" + ((end - start) / 10000.0F / 1000.0F).ToString("f2") + "秒");
        }
    }

    private static bool BuildCodes(List<string> arr)
    {
        SetASExtensionDic(true, false);
        List<string> list = new List<string>();
        List<string> tempAssetPath = new List<string>();
        List<AssetBundleBuild> finalAB = new List<AssetBundleBuild>();
        string outPath = System.IO.Path.Combine(AssetBundleOutPsth, Plathform.GetPlatformFolder(buildTarget)) + "/" + constResPath + "/Lua";
        if (arr.Count > 0)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                string path = arr[i];

                //如果是文件，则找到上一层目录
                path = path.Remove(0, path.IndexOf("/") + 1);
                path = path.Remove(0, path.IndexOf("/") + 1);
                if (path.Contains("."))
                {
                    path = path.Substring(0, path.LastIndexOf('/'));
                }
                path = path == constResPath ? "" : path;
                List<string> abDirectories = GetABDirectories(path);
                for (int j = 0; j < abDirectories.Count; j++)
                {
                    if (list.Contains(abDirectories[j]))
                    {
                        continue;
                    }
                    list.Add(abDirectories[j]);
                    string assetBundleName = abDirectories[j].Remove(0, abDirectories[j].IndexOf("/") + 1);
                    List<string> abResources = GetDirs(Application.dataPath + "/" + constResPath + "/" + abDirectories[j], asExtensionDic);
                    tempAssetPath.AddRange(AssetType(abResources));
                    AssetBundleBuild abb = CreateAB(assetBundleName, abResources);
                    if (abb.assetNames.Length > 0)
                    {
                        finalAB.Add(abb);
                    }
                }
            }
            if (finalAB.Count == 0)
            {
                Debug.LogWarning("请选择Lua脚本/目录来打包");
                return false;
            }
            //代码的话，本来就不大，为了快点打包，就先这样，而且不存在重名问题，所以不需要给每个codes唯一的hash ID
            StartBuildAsset(outPath, finalAB,BuildAssetBundleOptions.None);

            for (int i = 0; i < tempAssetPath.Count; i++)
            {
                File.Delete(tempAssetPath[i]);
            }
            
            if (File.Exists(outPath+"/Lua")) File.Delete(outPath + "/Lua");
            if (File.Exists(outPath + "/Lua.manifest")) File.Delete(outPath + "/Lua.manifest");

            AssetDatabase.Refresh();
        }
        return true;
    }
    private static void BuildAB(bool codes, bool resources)
    {



        if (resources)
        {
            long startRes = System.DateTime.Now.Ticks;
            SetASExtensionDic(false, true);

            string outPath = "";
            string assetBundleName = "";
            //防止资源重复打包
            List<string> list = new List<string>();
            List<AssetBundleBuild> finalAB = new List<AssetBundleBuild>();

            outPath = System.IO.Path.Combine(AssetBundleOutPsth, Plathform.GetPlatformFolder(buildTarget)) + "/" + constResPath; ;

            List<string> abDirectories = GetABDirectories("");

            for (int j = 0; j < abDirectories.Count; j++)
            {
                if (list.Contains(abDirectories[j]))
                {
                    continue;
                }
                list.Add(abDirectories[j]);
                assetBundleName = abDirectories[j];// Plathform.GetAssetBundleName(sourcePath);
                List<string> abResources = GetDirs(Application.dataPath + "/" + constResPath + "/" + abDirectories[j], asExtensionDic);
                AssetBundleBuild abb = CreateAB(assetBundleName, abResources);
                if (abb.assetNames.Length > 0)
                {
                    finalAB.Add(abb);
                }
            }
            if (finalAB.Count == 0)
            {
                Debug.LogWarning("请选择正确文件/目录来打包");
                return;
            }
            //资源这种东西，都比较大，还是压缩一下。但是不确定每次修改以后打包，是否都要全部重新压缩，所以先试试。如果不行，就在发版本的时候，压缩打一次，平时测试，就不压缩
            //资源存在重名可能，所以需要给所有Object一个唯一的hash ID
            StartBuildAsset(outPath, finalAB, BuildAssetBundleOptions.DeterministicAssetBundle|BuildAssetBundleOptions.ChunkBasedCompression);
            long endRes = System.DateTime.Now.Ticks;
            Debug.Log("打包Resources共耗时" + ((endRes - startRes) / 10000.0F / 1000.0F).ToString("f2") + "秒");
        }

        if (codes)
        {
            long start = System.DateTime.Now.Ticks;
            BuildCodes(new List<string> { constResPath + "/Lua" });
            long end = System.DateTime.Now.Ticks;
            Debug.Log("打包Codes共耗时" + ((end - start) / 10000.0F / 1000.0F).ToString("f2") + "秒");
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Build Tools/Zip/压缩")]
    private static void Compression()
    {
        if (Selection.objects.Length > 1)
        {
            Debug.LogError("请选择存档最外层文件夹，每次只能打包一个");
            return;
        }

        string name = Selection.objects[0].name;
        string q = AssetDatabase.GetAssetPath(Selection.objects[0]).Replace(name, "");

        string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        path = path.Replace('/', '\\');

        Debug.Log(path);
        MyIO.Zip.CompressionDirectory(path);
    }

    [MenuItem("Assets/Build Tools/Zip/解压")]
    private static void Decompression()
    {
        if (Selection.objects.Length > 1)
        {
            Debug.LogError("请选择存档最外层文件夹，每次只能打包一个");
            return;
        }

        string name = Selection.objects[0].name;
        string q = AssetDatabase.GetAssetPath(Selection.objects[0]).Replace(name, "");

        string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        path = path.Replace('/', '\\');

        Debug.Log(path);
        //MyIO.Zip.Decompression(path);
    }

    //添加需要打包资源的后缀
    private static void SetASExtensionDic(bool codes, bool resources)
    {
        asExtensionDic.Clear();
        if (codes)
        {
            asExtensionDic.Add(".lua", suffixName);

        }
        if (resources)
        {
            asExtensionDic.Add(".prefab", suffixName);
            asExtensionDic.Add(".mat", suffixName);
            asExtensionDic.Add(".png", suffixName);
            asExtensionDic.Add(".FBX", suffixName);
            asExtensionDic.Add(".obj", suffixName);
            asExtensionDic.Add(".csv", suffixName);
            asExtensionDic.Add(".bytes", suffixName);
            asExtensionDic.Add(".txt", suffixName);
            asExtensionDic.Add(".jpg", suffixName);
        }
    }

    private static string GetAssetPath(string sourcePath, string sourceName)
    {
        string path = sourcePath.Replace(sourceName, "");
        foreach (KeyValuePair<string, string> node in asExtensionDic)
        {
            if (path.Contains(node.Key))
            {
                return path.Replace(node.Key, "");
            }
        }
        return path;
    }

    //遍历制定文件夹获取需要打包的资源路径
    private static List<string> GetDirs(string dirPath, Dictionary<string, string> config)
    {
        List<string> result = new List<string>();
        string[] files = Directory.GetFiles(dirPath);
        foreach (string path in files)
        {
            // 通过资源后缀判断资源是否为需要打包的资源
            if (config.ContainsKey(System.IO.Path.GetExtension(path)))
            {
                string pathReplace = "";

                // Windows 平台分隔符为 '/', OS 平台 路径分隔符为 '\'， 此处是一个大坑
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    pathReplace = path.Replace('\\', '/');
                }

                //将需要打包的资源路径添加到打包路劲中
                result.Add(pathReplace);
            }
        }
        return result;
    }


    //获取所有AB包路径
    private static List<string> GetABDirectories(string path, List<string> list = null)
    {
        string fullPath = path == "" ? Application.dataPath + "/" + constResPath : Application.dataPath + "/" + constResPath + "/" + path;
        if (list == null)
        {
            list = new List<string>();
        }
        string[] arr = Directory.GetDirectories(fullPath);
        if (arr.Length > 0)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                GetABDirectories(path == "" ? Directory.CreateDirectory(arr[i]).Name : path + "/" + Directory.CreateDirectory(arr[i]).Name, list);
            }
        }
        else
        {
            if (path != "") list.Add(path);
        }
        return list;
    }
    //清除已经打包的资源 AssetBundleNames
    private static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
    }
    private static AssetBundleBuild CreateAB(string assetBundleName, List<string> assetPathList)
    {

        AssetBundleBuild bulid = new AssetBundleBuild();
        bulid.assetBundleName = assetBundleName + suffixName;
        for (int i = 0; i < assetPathList.Count; i++)
        {
            // 在Windows 平台 要从 Assets/ 开始，和 Mac 上不同，此处也是一个大坑
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                assetPathList[i] = assetPathList[i].Substring(assetPathList[i].IndexOf("Assets/"));
            }

            //// 从此处(assetPath = "AllAssets")截取路径  
            //string assetName = asPath.Substring(asPath.IndexOf(assetPath));
            ////替换后缀名
            //assetName = assetName.Replace(System.IO.Path.GetExtension(assetName), suffixName);
            //通过资源路径来获取需要打包的资源
            //AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            //if (assetImporter == null)
            //{
            //    Debug.LogWarning("null  " + path);
            //    continue;
            //}
            //设置打包资源的名字包括后缀
            //assetImporter.assetBundleName = assetName;
        }
        bulid.assetNames = assetPathList.ToArray();
        return bulid;
    }
    private static void StartBuildAsset(string outPath, List<AssetBundleBuild> buildsList, BuildAssetBundleOptions options)
    {
        //如果不存在到处路径文件，创建一个
        if (!Directory.Exists(outPath))
        {
            Directory.CreateDirectory(outPath);
        }

        BuildPipeline.BuildAssetBundles(outPath, buildsList.ToArray(), options, buildTarget);

    }

    private static List<string> AssetType(List<string> assetPathList)
    {
        string assetType = "";
        if (assetPathList.Count > 0)
        {
            assetType = System.IO.Path.GetExtension(assetPathList[0]);
        }
        switch (assetType)
        {
            case ".lua":
                return BulidLuaAsset(assetPathList);
            default:
                break;
        }
        return new List<string>();
    }

    private static List<string> BulidLuaAsset(List<string> assetPathList)
    {
        List<string> tempAssetPath = new List<string>();
        for (int i = 0; i < assetPathList.Count; i++)
        {
            File.Copy(assetPathList[i], assetPathList[i] + ".txt");
            assetPathList[i] += ".txt";
        }

        tempAssetPath = assetPathList;

        AssetDatabase.Refresh();
        return tempAssetPath;
    }

    #endregion
}


//根据切换的平台返回相应的导出路径
public class Plathform
{
    public static string GetPlatformFolder(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:   //Android平台导出到 Android文件夹中
                return "Android";
            case BuildTarget.iOS:
                return "IOS";
            //case BuildTarget.WebPlayer:
            //    return "WebPlayer";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            //case BuildTarget.StandaloneOSXIntel:
            //case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSX:
                return "OSX";
            default:
                return null;
        }
    }

    public static string GetAssetBundleName(string fileName)
    {
        string[] sss = fileName.Split(new char[] { '/' });
        if (sss.Length < 3) return "";
        switch (sss[2])
        {
            case "Model":
                return "Model";
            case "Config":
                return "GameConfig";
            case "Effect":
                return "Effect";
            case "Lua":
                return "Lua";
            default:

                break;
        }
        if (sss[sss.Length - 1].Contains("."))
        {
            return sss[sss.Length - 2];
        }
        else
        {
            return sss[sss.Length - 1];
        }
    }
}