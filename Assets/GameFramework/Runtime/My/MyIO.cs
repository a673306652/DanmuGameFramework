using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System;
using System.Text;

namespace Daan
{
    public static class MyIO
    {
        public static class Csv
        {

            public static void ReadLocalCsv(string filepath, Action<Dictionary<string, string>> action = null)
            {
                List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
                //string path = Application.dataPath + "/" + filepath + ".csv";
                string path = filepath;
                byte[] bytes = null;
                //if (!File.Exists(path))
                //{
                //    FileStream fs = File.Open(path, FileMode.Open);
                //    int len = fs.Read(bytes,0, (int)fs.Length);
                //    fs.Close();
                //}
                //else
                //{

                //}

                var text = Resources.Load<TextAsset>(filepath);
                bytes = text.bytes;

                string txt = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);


                table = GetDataFromCsv(filepath, txt);
                foreach (Dictionary<string, string> kv in table)
                {
                    action(kv);
                }
            }

            static List<Dictionary<string, string>> GetDataFromCsv(string filepath, string txt)
            {
                List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
                string[] lines = null;
                string[] head = null;
                int cnt = 0;
                try
                {
                    lines = txt.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    head = lines[0].Split(FrameworkConst.ConfigSplitChar);
                    cnt = head.Length;
                }
                catch
                {
                    UnityEngine.Debug.LogError(filepath + "编码格式有问题");
                }



                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[i]) || lines[i][0] == ',')
                        continue;

                    try
                    {
                        string[] row = lines[i].Split(FrameworkConst.ConfigSplitChar);
                        if (string.IsNullOrEmpty(row[0]))
                            continue;
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        for (int j = 0; j < cnt; j++)
                        {
                            dict.Add(head[j], row[j]);
                        }
                        table.Add(dict);
                    }
                    catch { }
                }

                return table;
            }

            static private string[] GetRowFromCsv(string line, int cnt)
            {
                line.Replace("\"\"", "\"");
                string[] strs = line.Split(FrameworkConst.ConfigSplitChar);
                if (strs.Length == cnt)
                    return strs;

                List<string> lst = new List<string>();
                int n = 0, begin = 0;
                bool flag = false;

                for (int i = 0; i < strs.Length; i++)
                {
                    if (strs[i].IndexOf("\"") == -1
                        || (!flag && strs[i][0] != '\"'))
                    {
                        lst.Add(strs[i]);
                        continue;
                    }

                    n = 0;
                    foreach (char ch in strs[i])
                    {
                        if (ch == '\"')
                            n++;
                    }
                    if (n % 2 == 0)
                    {
                        lst.Add(strs[i]);
                        continue;
                    }

                    flag = true;
                    begin = i;
                    i++;
                    for (i = begin + 1; i < strs.Length; i++)
                    {
                        foreach (char ch in strs[i])
                        {
                            if (ch == '\"')
                                n++;
                        }

                        if (strs[i][strs[i].Length - 1] == '\"' && n % 2 == 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (; begin <= i; begin++)
                            {
                                sb.Append(strs[begin]);
                                if (begin != i)
                                    sb.Append(",");
                            }
                            lst.Add(sb.ToString());
                            break;
                        }
                    }
                }
                return lst.ToArray();
            }
        }
        public static class Zip
        {
            /// 
            /// 压缩文件,默认目录为当前目录
            /// 
            /// 压缩后的文件
            /// 压缩的级别
            /// 要压缩的文件或文件夹
            public static void Compression(string zipedFileName, int zipLevel, params string[] source)
            {
                if (source == null)
                    return;
                else if (source.Length < 1)
                    return;
                else
                {
                    string str = source[0];
                    if (str.EndsWith("\\"))
                        str = str.Substring(0, str.Length - 1);
                    str = str.Substring(0, str.LastIndexOf("\\"));

                    Compression(zipedFileName, str, zipLevel, source);
                }
            }
            /// <summary>
            /// 压缩文件
            /// </summary>
            /// <param name="zipedFileName">压缩后的文件</param>
            /// <param name="currentDirectory">当前所处目录</param>
            /// <param name="zipLevel">压缩的级别</param>
            /// <param name="source">要压缩的文件或文件夹</param>
            private static void Compression(string zipedFileName, string currentDirectory, int zipLevel, params string[] source)
            {
                ArrayList AllFiles = new ArrayList();
                ZipConstants.DefaultCodePage = System.Text.Encoding.GetEncoding("gb2312").CodePage;
                //获取所有文件
                if (source != null)
                {
                    for (int i = 0; i < source.Length; i++)
                    {
                        if (File.Exists(source[i]))
                        {
                            string h = System.IO.Path.GetExtension(source[i]);
                            if (h != ".meta")
                            {
                                AllFiles.Add(source[i]);
                            }
                        }
                        else if (Directory.Exists(source[i]))
                        {
                            AllFiles.Add(source[i]);
                            GetDirectoryFile(source[i], AllFiles);
                        }
                    }
                }

                if (AllFiles.Count < 1)
                    return;

                ZipOutputStream zipedStream = new ZipOutputStream(File.Create(zipedFileName));
                zipedStream.SetLevel(zipLevel);

                for (int i = 0; i < AllFiles.Count; i++)
                {
                    string file = AllFiles[i].ToString();
                    FileStream fs;

                    //打开要压缩的文件
                    try
                    {
                        fs = File.OpenRead(file);
                    }
                    catch
                    {

                        continue;
                    }

                    try
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        //新建一个entry
                        string fileName = file.Replace(currentDirectory, "");
                        if (fileName.StartsWith("\\"))
                            fileName = fileName.Substring(1);
                        ZipEntry entry = new ZipEntry(fileName);
                        bool a = entry.IsUnicodeText;
                        entry.DateTime = DateTime.Now;

                        //保存到zip流
                        fs.Close();
                        zipedStream.PutNextEntry(entry);
                        zipedStream.Write(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }

                zipedStream.Finish();
                zipedStream.Close();
            }
            //递归获取一个目录下的所有文件
            private static void GetDirectoryFile(string parentDirectory, ArrayList toStore)
            {
                string[] files = Directory.GetFiles(parentDirectory);
                for (int i = 0; i < files.Length; i++)
                    toStore.Add(files[i]);
                string[] directorys = Directory.GetDirectories(parentDirectory);
                for (int i = 0; i < directorys.Length; i++)
                {
                    toStore.Add(directorys[i]);
                    GetDirectoryFile(directorys[i], toStore);
                }
            }

            /// <summary>
            /// 在源文件夹下压缩文件夹（需要打包的文件夹，压缩等级）
            /// </summary>
            public static void CompressionDirectory(string sourceDirectory, int zipLevel = 9)
            {
                if (sourceDirectory == null)
                    return;

                string dir = sourceDirectory;
                //去掉后缀
                if (dir.EndsWith("\\"))
                    dir = dir.Substring(0, dir.Length - 1);
                dir += ".zip";

                Compression(dir, zipLevel, sourceDirectory);
            }

            public static void DecompressionDirectory(string str)
            {
                if (str.EndsWith("\\"))
                    str = str.Substring(0, str.Length - 1);
                string parent = str.Substring(0, str.LastIndexOf("\\"));



            }
            /// <summary>
            /// 解压
            /// </summary>
            public static void Decompression(string source, string target)
            {

                //ZipEntry：文件条目 就是该目录下所有的文件列表(也就是所有文件的路径)  
                ZipEntry zip = null;
                //输入的所有的文件流都是存储在这里面的  
                ZipInputStream zipInStream = null;
                FileStream fs = File.OpenRead(source);
                //读取文件流到zipInputStream  
                zipInStream = new ZipInputStream(fs);



                //循环读取Zip目录下的所有文件  
                while ((zip = zipInStream.GetNextEntry()) != null)
                {
                    UnzipFile(zip, zipInStream, target);
                }
                try
                {
                    zipInStream.Close();


                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static void CopyFolder(string sourcePath, string destPath)
            {
                if (Directory.Exists(sourcePath))
                {
                    if (!Directory.Exists(destPath))
                    {
                        //目标目录不存在则创建
                        try
                        {
                            Directory.CreateDirectory(destPath);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("创建目标目录失败：" + ex.Message);
                        }
                    }
                    //获得源文件下所有文件
                    List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                    files.ForEach(c =>
                    {
                        string destFile = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(c));
                        File.Copy(c, destFile, true);//覆盖模式
                });
                    //获得源文件下所有目录文件
                    List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
                    folders.ForEach(c =>
                    {
                        string destDir = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(c));
                    //采用递归的方法实现
                    CopyFolder(c, destDir);
                    });
                }
                else
                {
                    throw new DirectoryNotFoundException("源目录不存在！");
                }
            }
            static void UnzipFile(ZipEntry zip, ZipInputStream zipInStream, string dirPath)
            {
                try
                {
                    //文件名不为空  
                    if (!string.IsNullOrEmpty(zip.Name))
                    {
                        string filePath = dirPath;
                        filePath += ("/" + zip.Name);


                        string path_tmp = System.IO.Path.GetDirectoryName(filePath);

                        //如果是一个新的文件路径　这里需要创建这个文件路径  
                        if (!Directory.Exists(path_tmp))
                        {
                            Directory.CreateDirectory(path_tmp);

                        }

                        {
                            FileStream fs = null;
                            //当前文件夹下有该文件  删掉  重新创建  
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            fs = File.Create(filePath);
                            int size = 2048;
                            byte[] data = new byte[2048];
                            //每次读取2MB  直到把这个内容读完  
                            while (true)
                            {
                                size = zipInStream.Read(data, 0, data.Length);
                                //小于0， 也就读完了当前的流  
                                if (size > 0)
                                {
                                    fs.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            fs.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sourceFolderName">源文件夹目录</param>
        /// <param name="destFolderName">目标文件夹目录</param>
        public static void Copy(string sourceFolderName, string destFolderName)
        {
            Copy(sourceFolderName, destFolderName, false);
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sourceFolderName">源文件夹目录</param>
        /// <param name="destFolderName">目标文件夹目录</param>
        /// <param name="overwrite">允许覆盖文件</param>
        public static void Copy(string sourceFolderName, string destFolderName, bool overwrite)
        {
            var sourceFilesPath = Directory.GetFileSystemEntries(sourceFolderName);

            for (int i = 0; i < sourceFilesPath.Length; i++)
            {
                var sourceFilePath = sourceFilesPath[i];
                var directoryName = System.IO.Path.GetDirectoryName(sourceFilePath);
                var forlders = directoryName.Split('\\');
                var lastDirectory = forlders[forlders.Length - 1];
                var dest = System.IO.Path.Combine(destFolderName, lastDirectory);

                if (File.Exists(sourceFilePath))
                {
                    var sourceFileName = System.IO.Path.GetFileName(sourceFilePath);
                    if (!Directory.Exists(dest))
                    {
                        Directory.CreateDirectory(dest);
                    }
                    File.Copy(sourceFilePath, System.IO.Path.Combine(dest, sourceFileName), overwrite);
                }
                else
                {
                    Copy(sourceFilePath, dest, overwrite);
                }
            }
        }

        /// <summary>
        /// 替换后缀名
        /// </summary>
        public static void ReplaceSuffix(string path, string suffixName = ".bytes")
        {
            GetDirs(path, suffixName);
        }

        //遍历制定文件夹获取需要打包的资源路径
        private static void GetDirs(string dirPath, string suffixName)
        {
            string[] files = Directory.GetFiles(dirPath);
            foreach (string path in files)
            {
                string suffix = System.IO.Path.GetExtension(path);
                if (suffix == ".meta")
                {
                    continue;
                }
                string[] sss = path.Split(new char[] { '/' });
                string fileName = sss[sss.Length - 1];
                if (suffix != suffixName)
                {
                    if (File.Exists(path))
                    {
                        File.Move(path, path.Replace(suffix, "") + suffixName);
                    }
                }
            }

            if (Directory.GetDirectories(dirPath).Length > 0)  //遍历所有文件夹
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    //使用递归方法遍历所有文件夹
                    GetDirs(path, suffixName);
                }
            }
        }

        /// <summary>
        /// Json序列化为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        public static string JsonSerialize<T>(this T jsonObj) where T : class
        {
            if (jsonObj == null)
            {
                return null;
            }
            return JsonUtility.ToJson(jsonObj);
        }

        /// <summary>
        /// Json反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(this string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return default(T);
            }
            return JsonUtility.FromJson<T>(jsonStr);
        }
    }
}
