#if UNITY_EDITOR
using EntityStats;
using EntityStats.Data;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StatsBlock))]
public class StatsBlockDrawer : PropertyDrawer
{
    private SerializedProperty _baseStatsProp;
    private SerializedProperty _growthFactorsProp;
    private SerializedProperty _equipmentBonusesProp;
    private SerializedProperty _levelProp;

    private StatsBlock _target;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Cache serialized properties
        _baseStatsProp = property.FindPropertyRelative("_baseStats");
        _growthFactorsProp = property.FindPropertyRelative("_growthFactors");
        _equipmentBonusesProp = property.FindPropertyRelative("_equipmentBonuses");
        _levelProp = property.FindPropertyRelative("_level");

        // Get the actual StatsBlock object (to read derived stats)
        _target = property.GetValue<StatsBlock>();

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // Draw level
        EditorGUI.PropertyField(rect, _levelProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw base stats
        EditorGUI.PropertyField(rect, _baseStatsProp);
        rect.y += EditorGUI.GetPropertyHeight(_baseStatsProp, true) + EditorGUIUtility.standardVerticalSpacing;

        // Draw growth factors
        EditorGUI.PropertyField(rect, _growthFactorsProp);
        rect.y += EditorGUI.GetPropertyHeight(_growthFactorsProp, true) + EditorGUIUtility.standardVerticalSpacing;

        // Draw equipment bonuses
        EditorGUI.PropertyField(rect, _equipmentBonusesProp);
        rect.y += EditorGUI.GetPropertyHeight(_equipmentBonusesProp, true) + EditorGUIUtility.standardVerticalSpacing;

        // Draw current base stats (after level & equipment)
        if (_target != null)
        {
            EditorGUI.LabelField(rect, "Current Base Stats", EditorStyles.boldLabel);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Show each stat
            EditorGUI.indentLevel++;
            foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
            {
                float value = _target.CurrentStats[stat];
                EditorGUI.LabelField(rect, stat.ToString(), value.ToString());
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.indentLevel--;

            // Draw derived stats
            EditorGUI.LabelField(rect, "Derived Stats", EditorStyles.boldLabel);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.indentLevel++;
            foreach (CalculatedStatType type in System.Enum.GetValues(typeof(CalculatedStatType)))
            {
                float value = _target.GetDerivedStat(type);
                EditorGUI.LabelField(rect, type.ToString(), value.ToString("F2"));
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0;
        _baseStatsProp = property.FindPropertyRelative("_baseStats");
        _growthFactorsProp = property.FindPropertyRelative("_growthFactors");
        _equipmentBonusesProp = property.FindPropertyRelative("_equipmentBonuses");
        _levelProp = property.FindPropertyRelative("_level");

        // Level height
        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Base stats height
        height += EditorGUI.GetPropertyHeight(_baseStatsProp, true) + EditorGUIUtility.standardVerticalSpacing;

        // Growth factors height
        height += EditorGUI.GetPropertyHeight(_growthFactorsProp, true) + EditorGUIUtility.standardVerticalSpacing;

        // Equipment bonuses height
        height += EditorGUI.GetPropertyHeight(_equipmentBonusesProp, true) + EditorGUIUtility.standardVerticalSpacing;

        // Add height for current stats and derived stats
        if (property.serializedObject.targetObject != null) // check if target exists
        {
            int statCount = System.Enum.GetValues(typeof(StatType)).Length;
            int derivedCount = System.Enum.GetValues(typeof(CalculatedStatType)).Length;

            // "Current Base Stats" label + each stat
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // label
            height += statCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            // "Derived Stats" label + each derived stat
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // label
            height += derivedCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }

        return height;
    }
}
#endif