using System.Collections.Generic;
namespace Modules.Localization
{
    using System;
    using Modules.Patterns;
    using NaughtyAttributes;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    [AddComponentMenu("Localization/LocalizationManager")]
    [ExecuteInEditMode]
    public class LocalizationManager : MonoSingleton<LocalizationManager>
    {
        [Header("Properties")]
        [SerializeField] private LanguageLocale _CurrentLang = (LanguageLocale)Localization.CurrentLocale;
        [Header("References")]
        [SerializeField] private FontAssetGroup[] _FontGroups;
        private UnityEvent txtUpdateEvents = new UnityEvent();

        public void Register(UnityAction onUpdate)
        {
            txtUpdateEvents.AddListener(onUpdate);
        }

        public void DeRegister(UnityAction onUpdate)
        {
            txtUpdateEvents.RemoveListener(onUpdate);
        }

        [Button]
        public void UpdateLocale()
        {
            Localization.SetLocale((int)_CurrentLang);
            txtUpdateEvents.Invoke();
        }

        public void SetLocale(int i = -1)
        {
            if (i < 0)
            {
                i = Localization.GetSystemLocale();
            }
            _CurrentLang = (LanguageLocale)i;
            Localization.SetLocale(i);
            txtUpdateEvents.Invoke();
        }
    }

    [Serializable]
    public struct FontAssetGroup
    {
        public LanguageLocale Locale;
        public Font[] FontAsset;
        public TMP_FontAsset[] FontAssetTMP;
    }
}