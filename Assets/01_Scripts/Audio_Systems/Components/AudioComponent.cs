using AniDrag.Audio.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace AniDrag.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioComponent : MonoBehaviour
    {
        [Header(" Settings volume")]
        public AudioMixerTypes type = AudioMixerTypes.Master;
        [field: SerializeField] public AudioMixerExposer audioMixer;
        public AudioSource audioSource;
        [field: SerializeField] public GameObject prefabSlider { get; private set; }

        List<GameObject> createdSliders = new List<GameObject>();
        public IReadOnlyList<GameObject> CreatedSliders => createdSliders;
        void Start()
        {
            if (audioMixer == null)
            {
                Debug.LogError($"{gameObject.name}: AudioMixerExposer is not assigned!");
                return;
            }
            if (audioMixer.ParentAudioMixer == null)
            {
                Debug.LogError($"{gameObject.name}: ParentAudioMixer is not set in AudioMixerExposer!");
                return;
            }
            if (transform.Find("Slider_" + audioMixer.mixerParams[0]) != null)
            {
                Debug.Log("Sliders already exist in scene, skipping runtime creation.");
                return;
            }
            // Setup audio source
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = audioMixer.ParentAudioMixerGroup;


            // Set initial master volume to 50% (optional)
            audioMixer.SetVolumeParameter("MasterVolume", 50f);

            

            // Create sliders only if all necessary references exist
            if (prefabSlider != null && audioMixer.mixerParams != null)
                CreateSlidersForParameters(this.transform, prefabSlider);
            else
                Debug.LogWarning($"{gameObject.name}: Cannot create sliders - missing panel, prefab, or parameter list.");
        }

        private void OnDestroy()
        {
            foreach (GameObject go in createdSliders)
            {
                if (go != null)
                {
                    Slider slider = go.GetComponent<Slider>();
                    if (slider != null) slider.onValueChanged.RemoveAllListeners();
                }
            }
        }
        public void CreateSlidersForParameters(Transform parentTransform, GameObject sliderPrefab)
        {
            if (parentTransform == null)
            {
                Debug.LogError("CreateSlidersForParameters: parentTransform is null!");
                return;
            }
            if (sliderPrefab == null)
            {
                Debug.LogError("CreateSlidersForParameters: sliderPrefab is null!");
                return;
            }
            if (audioMixer == null)
            {
                Debug.LogError("CreateSlidersForParameters: audioMixer is null!");
                return;
            }
            if (audioMixer.mixerParams == null || audioMixer.mixerParams.Count == 0)
            {
                Debug.LogWarning($"CreateSlidersForParameters: No parameters to create sliders for. Did you populate the list?");
                return;
            }

            foreach (string param in audioMixer.mixerParams)
            {
                // Instantiate the slider
                GameObject sliderObject = Instantiate(sliderPrefab, parentTransform);
                if (sliderObject == null)
                {
                    Debug.LogError($"Failed to instantiate slider for parameter: {param}");
                    continue;
                }

                Slider slider = sliderObject.GetComponentInChildren<Slider>();
                if (slider == null)
                {
                    Debug.LogError($"Prefab for parameter '{param}' does not have a Slider component!");
                    Destroy(sliderObject);
                    continue;
                }

                slider.gameObject.name = $"Slider_{param}";

                // Try to find a TMP_Text label in children
                TMP_Text label = sliderObject.GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = param;
                else
                    Debug.LogWarning($"No TMP_Text found on slider for param: {param}");

                // Set initial value (converted to 0-1)
                slider.value = audioMixer.GetVolumeParameter(param) / 100f;

                // Add listener with captured param
                string capturedParam = param;
                slider.onValueChanged.AddListener(val =>
                {
                    audioMixer.SetVolumeParameter(capturedParam, val * 100f);
                });

                createdSliders.Add(sliderObject);
            }

            // Optionally store the created sliders for later cleanup (e.g., in OnDestroy)
            // this.createdSliders = createdSliders; // you would need a class-level list for that
        }
        public void ClearCreatedSliders()
        {
            foreach (GameObject go in createdSliders)
            {
                if (go != null)
                {
                    // Use DestroyImmediate for edit?mode cleanup
                    DestroyImmediate(go);
                }
            }
            createdSliders.Clear();
        }
    }
}