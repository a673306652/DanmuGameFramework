// using System.Collections.Generic;
// using Data.AutoCSV;
// using Modules.Patterns;

// public partial class AutoCsvReaderManager : Singleton<AutoCsvReaderManager>
// {
// // X-RawList
//     #region
//     private Dictionary<string, LocalizationCSVAutoModel> localizationCSVAutoModelDict;
//     public Dictionary<string, LocalizationCSVAutoModel> LocalizationCSVAutoModelDic
//     {
//         get
//         {
//             if (localizationCSVAutoModelDict == null)
//             {
//                 var reader = new LocalizationCSVAutoReader();
//                 localizationCSVAutoModelDict = reader.LocalizationDic;
//             }
//             return localizationCSVAutoModelDict;
//         }
//     }
//     #endregion
// // X-RawNative
// }