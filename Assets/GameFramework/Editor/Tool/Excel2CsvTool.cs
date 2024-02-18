using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;
using Excel;
using System.Data;
using System;
using ICSharpCode.SharpZipLib.Zip;
public class Excel2CsvTool : EditorWindow
{
    private static string excelPath = Application.dataPath + "/Config";
    private static string csvPath = Application.dataPath + "/Resources/Config";
    [MenuItem("Build Tools/Excel To Csv")]
    private static void Excel2Csv_All()
    {
        DirectoryInfo dInfo = new DirectoryInfo(excelPath);

        if (Directory.Exists(csvPath))
        {
            Directory.Delete(csvPath, true);
        }
        FileInfo[] files = dInfo.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta")) continue;
            string outputPath = csvPath + files[i].Directory.FullName.Replace(dInfo.FullName, "");
            string inputPath = files[i].Directory.FullName;
            string fileName = Path.GetFileNameWithoutExtension(files[i].Name);

            DataSet set = GetDataSet(inputPath, files[i].Name);
            ConverToCsv(set, files[i].FullName, outputPath, fileName);
        }
        AssetDatabase.Refresh();
    }


    [MenuItem("Assets/Build Tools/Excel To Csv")]
    private static void Excel2Csv_Select()
    {
        DirectoryInfo dInfo = new DirectoryInfo(excelPath);
        UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        if (SelectedAsset != null && SelectedAsset.Length > 0)
        {
            for (int i = 0; i < SelectedAsset.Length; i++)
            {
                FileInfo info = new FileInfo(AssetDatabase.GetAssetPath(SelectedAsset[i]));
                if (info.Name.EndsWith(".meta")) continue;
                if (!info.Name.EndsWith(".xls") && !info.Name.EndsWith(".xlsx")) continue;


                string outputPath = csvPath + info.Directory.FullName.Replace(dInfo.FullName, "");
                string inputPath = info.Directory.FullName;
                string fileName = Path.GetFileNameWithoutExtension(info.Name);

                DataSet set = GetDataSet(inputPath, info.Name);
                ConverToCsv(set, info.FullName, outputPath, fileName);
            }
        }
        AssetDatabase.Refresh();
    }





    private static DataSet GetDataSet(string inputPath, string fileName)
    {
        string file = inputPath + "/" + fileName;
        DataSet result = null;
        FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        string extension = Path.GetExtension(file);

        if (string.CompareOrdinal(extension, ".xls") == 0)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            result = excelReader.AsDataSet();

            excelReader.IsFirstRowAsColumnNames = true;
            excelReader.Close();
        }
        else if (string.CompareOrdinal(extension, ".xlsx") == 0)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            result = excelReader.AsDataSet();

            excelReader.Close();
        }
        else
        {
            Debug.Log("extension is not xls or xlsx");
        }
        return result;
    }
    private static void ConverToCsv(DataSet set, string inputPath, string outputPath, string fileName)
    {
        string outputFile = outputPath + "/" + fileName + ".csv";
        if (Directory.Exists(outputPath))
        {
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }
        else
        {
            Directory.CreateDirectory(outputPath);
        }

        var content = GetExcelFile(set);
        using (var sw = new StreamWriter(outputFile, false,new System.Text.UTF8Encoding(false)))
        {
            sw.Write(content);
        }
    }

    private static StringBuilder GetExcelFile(DataSet dataTabale, int ind = 0)
    {
        var content = new StringBuilder();
        var rowNumber = 0;

        while (rowNumber < dataTabale.Tables[ind].Rows.Count)
        {
            for (int i = 0; i < dataTabale.Tables[ind].Columns.Count; i++)
            {
                if (dataTabale.Tables[ind].Rows[rowNumber][i].ToString() == "//") break;
                content.Append(dataTabale.Tables[ind].Rows[rowNumber][i]);

                if (i != dataTabale.Tables[ind].Columns.Count - 1)
                {
                    content.Append(FrameworkConst.ConfigSplitChar);
                }
                else
                {
                    content.Append(Environment.NewLine);
                }
            }
            rowNumber++;
        }
        return content;
    }
}
