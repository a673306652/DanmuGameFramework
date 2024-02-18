using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FilePathPropertyDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            return GetPropertyHeight(property);
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            var attribute = PropertyUtility.GetAttribute<FolderPathAttribute>(property);
            EditorGUILayout.LabelField(property.displayName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(property.stringValue);
            if (GUILayout.Button(EditorGUIUtility.IconContent("Icon_Editor_Save"), GUILayout.Width(60f), GUILayout.Height(20f)))
            {
                property.stringValue = EditorPathUtil.ConvertPathRelativeToAssets(EditorUtility.OpenFolderPanel("Select your saving path...", attribute.DefaultPath, property.stringValue));
                property.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
