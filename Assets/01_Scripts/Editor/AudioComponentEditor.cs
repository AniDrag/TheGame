using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using AniDrag.Audio;

[CustomEditor(typeof(AudioComponent))]
public class AudioComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (shows the fields you defined)
        DrawDefaultInspector();

        AudioComponent audioComponent = (AudioComponent)target;

        // If there's no AudioMixerExposer assigned, show a warning
        if (audioComponent.audioMixer == null)
        {
            EditorGUILayout.HelpBox("Assign an AudioMixerExposer in the inspector first.", MessageType.Warning);
            return;
        }

        // If the mixer is not set, show a warning
        if (audioComponent.audioMixer.ParentAudioMixer == null)
        {
            EditorGUILayout.HelpBox("ParentAudioMixer in AudioMixerExposer is not assigned. Cannot populate parameters.", MessageType.Warning);
            return;
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Populate Parameters from Mixer"))
        {
            // Get exposed parameters from the mixer
            string[] exposedParams = GetExposedParametersFromMixer(audioComponent.audioMixer.ParentAudioMixer);
            if (exposedParams != null && exposedParams.Length > 0)
            {
                // Replace the list with the fetched parameters
                audioComponent.audioMixer.mixerParams.Clear();
                audioComponent.audioMixer.mixerParams.AddRange(exposedParams);
                EditorUtility.SetDirty(audioComponent); // Save changes to the component
                Debug.Log($"Populated {exposedParams.Length} parameters from mixer.");
            }
            else
            {
                Debug.LogWarning("No exposed parameters found in the mixer. Did you expose any?");
            }
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Instantiate Sliders in Scene (Editor)"))
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Cannot instantiate sliders while in Play Mode.", MessageType.Error);
                return;
            }

            if (audioComponent.prefabSlider == null)
            {
                EditorGUILayout.HelpBox("No slider prefab assigned. Drag a prefab with a Slider component into the 'prefabSlider' field.", MessageType.Error);
                return;
            }

            if (audioComponent.audioMixer.mixerParams == null || audioComponent.audioMixer.mixerParams.Count == 0)
            {
                EditorGUILayout.HelpBox("No parameters populated. Click 'Populate Parameters from Mixer' first.", MessageType.Warning);
                return;
            }

            // Record state for undo
            Undo.RecordObject(audioComponent, "Clear Existing Sliders");

            // Destroy old sliders
            audioComponent.ClearCreatedSliders();

            // Create new sliders (this will add to the createdSliders list)
            audioComponent.CreateSlidersForParameters(audioComponent.transform, audioComponent.prefabSlider);

            // Register undo for each newly created object
            foreach (GameObject go in audioComponent.CreatedSliders)
            {
                if (go != null)
                    Undo.RegisterCreatedObjectUndo(go, "Create Audio Slider");
            }

            // Mark the component as dirty so changes are saved
            EditorUtility.SetDirty(audioComponent);
        }
    }

    private string[] GetExposedParametersFromMixer(UnityEngine.Audio.AudioMixer mixer)
    {
        // Use SerializedObject to safely read exposed parameters (Editor only)
        SerializedObject serializedMixer = new SerializedObject(mixer);
        SerializedProperty exposedParamsProp = serializedMixer.FindProperty("m_ExposedParameters");
        if (exposedParamsProp == null || !exposedParamsProp.isArray)
            return null;

        List<string> paramNames = new List<string>();
        for (int i = 0; i < exposedParamsProp.arraySize; i++)
        {
            SerializedProperty paramProp = exposedParamsProp.GetArrayElementAtIndex(i);
            SerializedProperty nameProp = paramProp.FindPropertyRelative("name");
            if (nameProp != null)
                paramNames.Add(nameProp.stringValue);
        }
        return paramNames.ToArray();
    }
}