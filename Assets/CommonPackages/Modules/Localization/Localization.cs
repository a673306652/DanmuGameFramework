using System.Net.Mime;
using UnityEngine;

namespace Modules.Localization
{
    public static class Localization
    {
        private static int currentLocale = 0;
        public static int CurrentLocale
        {
            get
            {
                return currentLocale;
            }
        }

        public static LanguageLocale CurrentLocaleAlias
        {
            get
            {
                return (LanguageLocale)currentLocale;
            }
        }

        public static void SetLocale(int i = -1)
        {
            if (i < 0)
            {
                i = GetSystemLocale();
            }
            currentLocale = i;
        }

        public static int GetSystemLocale()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    return (int)LanguageLocale.en;
                case SystemLanguage.ChineseSimplified:
                    return (int)LanguageLocale.zhs;
                // TODO GUANGLING 根据需要开放多语言
                // case SystemLanguage.ChineseTraditional:
                //     return (int)LanguageLocale.zht;
                // case SystemLanguage.French:
                //     return (int)LanguageLocale.fr;
                // case SystemLanguage.German:
                //     return (int)LanguageLocale.de;
                // case SystemLanguage.Spanish:
                //     return (int)LanguageLocale.es;
                // case SystemLanguage.Japanese:
                //     return (int)LanguageLocale.jp;
                // case SystemLanguage.Portuguese:
                //     return (int)LanguageLocale.pt;
                // case SystemLanguage.Vietnamese:
                //     return (int)LanguageLocale.vn;
                // case SystemLanguage.Indonesian:
                //     return (int)LanguageLocale.id;
                default:
                    return (int)LanguageLocale.en;
            }
        }
    }

    public enum LanguageLocale
    {
        en = 0,
        zhs = 1,
        zht = 2,
        fr = 3,
        de = 4,
        es = 5,
        jp = 6,
        pt = 7,
        vn = 8,
        id = 9,
    }
}