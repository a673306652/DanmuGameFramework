using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(IndicatorAttribute))]
    public class IndicatorPropertyDrawer : PropertyDrawerBase
    {
        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            var attribute = PropertyUtility.GetAttribute<IndicatorAttribute>(property);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2f, rect.height), property.displayName);
            EditorGUI.DrawRect(new Rect(rect.x + rect.width / 2f, rect.y + rect.height / 16f, rect.height, rect.height), EColor.Black.GetColor());
            EditorGUI.DrawRect(new Rect(rect.x + rect.width / 2f + rect.height / 8f, rect.y + rect.height / 4f, rect.height / 4f * 3f, rect.height / 4f * 3f), property.boolValue ? attribute.TrueColor.GetColor() : attribute.FalseColor.GetColor());
        }
    }
}
