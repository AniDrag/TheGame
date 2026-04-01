#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using EntityStats.Data;

[CustomPropertyDrawer(typeof(Stats))]
public class StatsDrawer : PropertyDrawer
{
    private static readonly string[] StatNames = System.Enum.GetNames(typeof(StatType));
    private static readonly int StatCount = StatNames.Length;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Find the _values array property
        SerializedProperty valuesProp = property.FindPropertyRelative("_values");

        // Ensure the array size matches the number of stats
        if (valuesProp.arraySize != StatCount)
        {
            valuesProp.arraySize = StatCount;
            property.serializedObject.ApplyModifiedProperties();
        }

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rect, label);

        EditorGUI.indentLevel++;
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        for (int i = 0; i < StatCount; i++)
        {
            var element = valuesProp.GetArrayElementAtIndex(i);
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, element, new GUIContent(StatNames[i]));
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        }
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Height = one line for the label + one line per stat (including spacing)
        return (StatCount + 1) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }
}
#endif