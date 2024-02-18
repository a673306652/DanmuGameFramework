// using TMPro;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// namespace Modules.Localization
// {
//     [RequireComponent(typeof(TextMeshProUGUI))]
//     public class UGUITMPLocalizer : UGUIBaseTextLocalizer
//     {
//         protected override void Start()
//         {
//             base.Start();
//             if (AutoTranslate)
//                 Format();
//         }

//         protected override void SetText(string t)
//         {
//             GetComponent<TextMeshProUGUI>().text = t;
//         }
//     }
// }