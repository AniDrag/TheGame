using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
namespace AniDrag.Audio.Utility
{
    [System.Serializable]
    public class AudioMixerExposer
    {

        [field: SerializeField] public AudioMixer ParentAudioMixer { get; private set; }
        [field: SerializeField] public List<string> mixerParams { get; private set; } = new List<string>();

        // Optional: if you want to also store the group for other purposes
        [field: SerializeField] public AudioMixerGroup ParentAudioMixerGroup { get; private set; }

        // Default dB range for volume parameters (adjustable)
        [SerializeField, Range(-80f, 5f)] private float minDB = -80f;
        [SerializeField, Range(-80f, 5f)] private float maxDB = 5f;

        #region Constructors
        public AudioMixerExposer(AudioMixer pParentAudioMixer)
        {
            ParentAudioMixer = pParentAudioMixer;
            mixerParams.Clear();
        }

        public AudioMixerExposer(AudioMixer pParentAudioMixer, List<string> pMixerParams) : this(pParentAudioMixer)
        {
            mixerParams = pMixerParams;
        }

        public AudioMixerExposer(AudioMixer pParentAudioMixer, string pMixerParams) : this(pParentAudioMixer)
        {
            mixerParams.Add(pMixerParams);
        }
        #endregion
        #region Volume Helpers (0–100 ? dB) 
        /// <summary>Converts a linear 0–100 value to decibels using the current min/max range.</summary>
        public float NormalizedToDB(float normalized)
        {
            return Mathf.Lerp(minDB, maxDB, Mathf.Clamp01(normalized / 100f));
        }

        /// <summary>Converts decibels to a linear 0–100 value using the current min/max range.</summary>
        public float DBToNormalized(float dB)
        {
            return Mathf.InverseLerp(minDB, maxDB, dB) * 100f;
        }

        /// <summary>Sets a float parameter using a 0–100 linear value (converted to dB).</summary>
        public void SetVolumeParameter(string paramName, float normalizedValue)
        {
            if (ParentAudioMixer == null)
            {
                Debug.LogError("AudioMixerExposer: ParentAudioMixer is null!");
                return;
            }

            float dbValue = NormalizedToDB(normalizedValue);
            if (!ParentAudioMixer.SetFloat(paramName, dbValue))
            {
                Debug.LogWarning($"AudioMixerExposer: Failed to set parameter '{paramName}' (does it exist? Is it a float?)");
            }
        }

        /// <summary>Gets a float parameter's current value as a normalized 0–100 value.</summary>
        public float GetVolumeParameter(string paramName)
        {
            if (ParentAudioMixer == null)
            {
                Debug.LogError("AudioMixerExposer: ParentAudioMixer is null!");
                return 0f;
            }

            if (ParentAudioMixer.GetFloat(paramName, out float dbValue))
            {
                return DBToNormalized(dbValue);
            }
            else
            {
                Debug.LogWarning($"AudioMixerExposer: Failed to get parameter '{paramName}'");
                return 0f;
            }
        }
        #endregion
        #region Utility Methods

        void GetAllParams()
        {
            //ParentAudioMixerGroup.paramaters.add to mixerParams;
        }
        /// <summary>Sets a float parameter directly (no conversion).</summary>
        public void SetFloatParameter(string paramName, float value)
        {
            if (ParentAudioMixer == null) return;
            if (!ParentAudioMixer.SetFloat(paramName, value))
                Debug.LogWarning($"AudioMixerExposer: Failed to set float parameter '{paramName}'");
        }

        /// <summary>Gets a float parameter's value directly.</summary>
        public bool GetFloatParameter(string paramName, out float value)
        {
            if (ParentAudioMixer == null)
            {
                value = 0f;
                return false;
            }
            return ParentAudioMixer.GetFloat(paramName, out value);
        }

        /// <summary>Sets a bool ? parameter.</summary>
        /// <summary>Gets a bool? parameter.</summary>

        /// <summary>Returns the current normalized value for every parameter in the mixerParams list.</summary>
        public Dictionary<string, float> GetAllVolumeParameters()
        {
            var dict = new Dictionary<string, float>();
            foreach (string param in mixerParams)
            {
                dict[param] = GetVolumeParameter(param);
            }
            return dict;
        }

        /// <summary>Sets all volume parameters in the list to the same normalized value.</summary>
        public void SetAllVolumeParameters(float normalizedValue)
        {
            foreach (string param in mixerParams)
            {
                SetVolumeParameter(param, normalizedValue);
            }
        }
        #endregion

    }
}