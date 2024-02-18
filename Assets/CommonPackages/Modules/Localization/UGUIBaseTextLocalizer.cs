// using NaughtyAttributes;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// namespace Modules.Localization
// {
//     public class UGUIBaseTextLocalizer : MonoBehaviour
//     {
//         [SerializeField] protected string LocalizedKey;
//         [SerializeField] protected string TrailingContent;
//         [SerializeField] protected bool EscapeQuotes;
//         [SerializeField] protected bool AutoTranslate;
//         [ReadOnly]
//         [SerializeField] protected int CurrentLocale;
//         public delegate string CustomFormat(string localizedText, object[] args);

//         protected object[] formatArgs = new object[] { };
//         protected CustomFormat formatter;

//         public UGUIBaseTextLocalizer SetLocalizedKey(string key)
//         {
//             LocalizedKey = key;
//             return this;
//         }

//         public UGUIBaseTextLocalizer SetFormatter(CustomFormat fmt)
//         {
//             formatter = fmt;
//             return this;
//         }

//         public virtual void Format(params object[] args)
//         {
//             CurrentLocale = Localization.CurrentLocale;
//             formatArgs = args;
//             var rawStr = LocalizedKey.ToLocalizedString();
//             if (EscapeQuotes)
//             {
//                 rawStr = rawStr.Replace("\"", "");
//             }
//             if (null != formatter)
//             {
//                 SetText(formatter.Invoke(rawStr + TrailingContent, formatArgs));
//                 return;
//             }
//             if (formatArgs.Length > 0)
//                 SetText(string.Format(rawStr + TrailingContent, formatArgs));
//             else
//                 SetText(rawStr + TrailingContent);
//         }

//         protected virtual void SetText(string t)
//         {

//         }

//         protected virtual void Start()
//         {

//         }

//         protected virtual void OnEnable()
//         {
//             LocalizationManager.Instance?.Register(OnLocaleUpdate);
//         }

//         protected virtual void OnDisable()
//         {
//             LocalizationManager.Instance?.DeRegister(OnLocaleUpdate);
//         }

//         protected virtual void OnLocaleUpdate()
//         {
//             Format(formatArgs);
//         }
//     }
// }