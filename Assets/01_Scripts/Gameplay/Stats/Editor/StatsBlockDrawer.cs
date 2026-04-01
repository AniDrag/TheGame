#if UNITY_EDITOR
using EntityStats;
using EntityStats.Data;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(StatsBlock))]
public class StatsBlockDrawer : PropertyDrawer
{
    // Foldout state storage
    private static Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    // Property references
    private SerializedProperty _baseStatsProp;
    private SerializedProperty _growthFactorsProp;
    private SerializedProperty _equipmentBonusesProp;
    private SerializedProperty _levelProp;

    // Group names (used as keys in the dictionary)
    private const string BaseStatsGroup = "Base Stats";
    private const string GrowthStatsGroup = "Growth Stats";
    private const string EquipmentStatsGroup = "Equipment Stats";
    private const string CurrentBaseStatsGroup = "Current Base Stats";
    private const string DerivedStatsGroup = "Derived Stats";

    private StatsBlock _target;

    // Helper to get or set foldout state
    private bool GetFoldoutState(string key)
    {
        if (!foldoutStates.ContainsKey(key))
            foldoutStates[key] = true; // default to expanded
        return foldoutStates[key];
    }

    private void SetFoldoutState(string key, bool value)
    {
        foldoutStates[key] = value;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Cache properties
        _baseStatsProp = property.FindPropertyRelative("_baseStats");
        _growthFactorsProp = property.FindPropertyRelative("_growthFactors");
        _equipmentBonusesProp = property.FindPropertyRelative("_equipmentBonuses");
        _levelProp = property.FindPropertyRelative("_level");

        _target = property.GetValue<StatsBlock>();

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // Draw level (always visible)
        EditorGUI.PropertyField(rect, _levelProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw foldout groups
        DrawGroup(rect, ref rect, BaseStatsGroup, _baseStatsProp);
        DrawGroup(rect, ref rect, GrowthStatsGroup, _growthFactorsProp);
        DrawGroup(rect, ref rect, EquipmentStatsGroup, _equipmentBonusesProp);

        // Draw current base stats (read-only)
        if (_target != null)
        {
            DrawCurrentBaseStats(rect, ref rect);
            DrawDerivedStats(rect, ref rect);
        }

        EditorGUI.EndProperty();
    }

    private void DrawGroup(Rect currentRect, ref Rect nextRect, string groupName, SerializedProperty property)
    {
        // Draw foldout header
        bool expanded = GetFoldoutState(groupName);
        Rect headerRect = new Rect(currentRect.x, currentRect.y, currentRect.width, EditorGUIUtility.singleLineHeight);
        expanded = EditorGUI.Foldout(headerRect, expanded, groupName, true);
        SetFoldoutState(groupName, expanded);

        nextRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw content if expanded
        if (expanded)
        {
            EditorGUI.indentLevel++;
            float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
            Rect propertyRect = new Rect(currentRect.x, nextRect.y, currentRect.width, propertyHeight);
            EditorGUI.PropertyField(propertyRect, property, GUIContent.none, true);
            nextRect.y += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel--;
        }
    }

    private void DrawCurrentBaseStats(Rect currentRect, ref Rect nextRect)
    {
        bool expanded = GetFoldoutState(CurrentBaseStatsGroup);
        Rect headerRect = new Rect(currentRect.x, nextRect.y, currentRect.width, EditorGUIUtility.singleLineHeight);
        expanded = EditorGUI.Foldout(headerRect, expanded, CurrentBaseStatsGroup, true);
        SetFoldoutState(CurrentBaseStatsGroup, expanded);
        nextRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (expanded)
        {
            EditorGUI.indentLevel++;
            foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
            {
                float value = _target.CurrentStats[stat];
                Rect statRect = new Rect(currentRect.x, nextRect.y, currentRect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(statRect, stat.ToString(), value.ToString());
                nextRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.indentLevel--;
        }
    }

    private void DrawDerivedStats(Rect currentRect, ref Rect nextRect)
    {
        bool expanded = GetFoldoutState(DerivedStatsGroup);
        Rect headerRect = new Rect(currentRect.x, nextRect.y, currentRect.width, EditorGUIUtility.singleLineHeight);
        expanded = EditorGUI.Foldout(headerRect, expanded, DerivedStatsGroup, true);
        SetFoldoutState(DerivedStatsGroup, expanded);
        nextRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (expanded)
        {
            EditorGUI.indentLevel++;
            foreach (CalculatedStatType type in System.Enum.GetValues(typeof(CalculatedStatType)))
            {
                float value = _target.GetDerivedStat(type);
                Rect statRect = new Rect(currentRect.x, nextRect.y, currentRect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(statRect, type.ToString(), value.ToString("F2"));
                nextRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Cache properties for height calculations
        _baseStatsProp = property.FindPropertyRelative("_baseStats");
        _growthFactorsProp = property.FindPropertyRelative("_growthFactors");
        _equipmentBonusesProp = property.FindPropertyRelative("_equipmentBonuses");
        _levelProp = property.FindPropertyRelative("_level");
        _target = property.GetValue<StatsBlock>();

        float height = 0;

        // Level line
        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Groups: each contributes a header line + content if expanded
        height += GetGroupHeight(BaseStatsGroup, _baseStatsProp);
        height += GetGroupHeight(GrowthStatsGroup, _growthFactorsProp);
        height += GetGroupHeight(EquipmentStatsGroup, _equipmentBonusesProp);

        // Current stats and derived stats (if target exists)
        if (_target != null)
        {
            height += GetStatsGroupHeight(CurrentBaseStatsGroup, System.Enum.GetValues(typeof(StatType)).Length);
            height += GetStatsGroupHeight(DerivedStatsGroup, System.Enum.GetValues(typeof(CalculatedStatType)).Length);
        }

        return height;
    }

    private float GetGroupHeight(string groupName, SerializedProperty property)
    {
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // header
        if (GetFoldoutState(groupName))
        {
            height += EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.standardVerticalSpacing;
        }
        return height;
    }

    private float GetStatsGroupHeight(string groupName, int itemCount)
    {
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // header
        if (GetFoldoutState(groupName))
        {
            // Each stat line + one extra spacing for indent level (optional)
            height += itemCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
        return height;
    }
}
#endif