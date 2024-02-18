// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// namespace Modules.Localization
// {
//     [RequireComponent(typeof(Text))]
//     public class UGUITextLocalizer : UGUIBaseTextLocalizer
//     {
//         protected override void Start()
//         {
//             base.Start();
//             if (AutoTranslate)
//                 Format();
//         }

//         protected override void SetText(string t)
//         {
//             GetComponent<Text>().text = t;
//         }
//     }
// }