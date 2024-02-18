// using System;
// using UnityEngine;

// namespace Modules.Localization
// {
//     public static class LocalizedExt
//     {
//         public static string ToLocalizedString(this string key, string returnOnDefault = "")
//         {
//             if (
//                 !string.IsNullOrEmpty(key) &&
//                 AutoCsvReaderManager
//                     .Instance
//                     .LocalizationCSVAutoModelDic
//                     .ContainsKey(key)
//             )
//             {
//                 var strings =
//                     AutoCsvReaderManager
//                         .Instance
//                         .LocalizationCSVAutoModelDic[key];
//                 switch (Localization.CurrentLocale)
//                 {
//                     case (int)LanguageLocale.en:
//                         return strings.en;
//                     case (int)LanguageLocale.zhs:
//                         return strings.zhs;
//                     case (int)LanguageLocale.zht:
//                         return strings.zht;
//                     case (int)LanguageLocale.fr:
//                         return strings.fr;
//                     case (int)LanguageLocale.de:
//                         return strings.de;
//                     case (int)LanguageLocale.es:
//                         return strings.es;
//                     case (int)LanguageLocale.jp:
//                         return strings.jp;
//                     case (int)LanguageLocale.pt:
//                         return strings.pt;
//                     case (int)LanguageLocale.vn:
//                         return strings.vn;
//                     case (int)LanguageLocale.id:
//                         return strings.id;
//                 }
//             }
//             return returnOnDefault;
//         }

//         public static string ToRomeNumber(int number)
//         {
//             switch (number)
//             {
//                 case 1:
//                     return "I";
//                 case 2:
//                     return "II";
//                 case 3:
//                     return "III";
//                 case 4:
//                     return "IV";
//                 case 5:
//                     return "V";
//                 case 6:
//                     return "VI";
//                 case 7:
//                     return "VII";
//                 case 8:
//                     return "VIII";
//                 case 9:
//                     return "IX";
//                 case 10:
//                     return "X";
//             }
//             return "";
//         }
//     }
// }
