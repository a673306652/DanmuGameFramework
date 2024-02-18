using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextMesh))]
public class TextMeshFontSizeSupport : Editor {


    public override void OnInspectorGUI()
    {
        TextMesh value = target as TextMesh;
        if (value)
        {
            EditorGUILayout.LabelField("实装Font Size支持，支持设置0值，出问题找Daan");
            value.text = EditorGUILayout.TextField("Text", value.text,GUILayout.Height(40));
            value.offsetZ = EditorGUILayout.FloatField("OffsetZ", value.offsetZ);
            value.characterSize = EditorGUILayout.FloatField("Character Size", value.characterSize);
            value.lineSpacing = EditorGUILayout.FloatField("Line Spacing", value.lineSpacing);
            value.anchor= (TextAnchor)EditorGUILayout.EnumPopup("Anchor", value.anchor);
            value.alignment = (TextAlignment)EditorGUILayout.EnumPopup("Alignment", value.alignment);
            value.tabSize = EditorGUILayout.FloatField("Tab Size", value.tabSize);
            value.fontSize = EditorGUILayout.IntField("Font Size", value.fontSize);
            value.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", value.fontStyle);
            value.richText= EditorGUILayout.Toggle("Rich Text", value.richText);
            value.font = (Font)EditorGUILayout.ObjectField("Font", value.font,typeof(Font),true);
            value.color = EditorGUILayout.ColorField("Color", value.color);
        }
    }
}
