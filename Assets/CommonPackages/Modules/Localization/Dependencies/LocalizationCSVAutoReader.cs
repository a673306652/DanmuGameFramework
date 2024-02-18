// namespace Data.AutoCSV
// {
//     using System.Collections.Generic;
//     using Modules.CSV;
//     public class LocalizationCSVAutoReader : BaseAutoCSVReader<LocalizationCSVAutoModel>
//     {
//         public LocalizationCSVAutoReader()
//         {
//             LoadData();
//         }
//         public List<LocalizationCSVAutoModel> LocalizationList;
//         public Dictionary<string, LocalizationCSVAutoModel> LocalizationDic;
//         // X-RawNative public Localization LocalizationNative;
//         public void LoadData()
//         {
//             LocalizationList = RealLoadConfig("Data.AutoCSV.LocalizationCSVAutoModel", "CSV/localization");
//             LocalizationDic = new Dictionary<string, LocalizationCSVAutoModel>();
//             // X-RawNative LocalizationNative = RealLoadNative("Data.AutoCSV.LocalizationCSVAutoModel", "CSV/localization", "X-RawNativeKey", "X-RawNativeVal");
//             foreach (var temp in LocalizationList)
//             {
//                 LocalizationDic[temp.key] = temp;
//             }
//         }
//     }
// }