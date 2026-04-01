using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Gameplay.FiniteStateMachine
{
    public class Blackboard : MonoBehaviour
    {
        [TextArea(5, 20)]
        public string DebugNotes = string.Empty;
        // Runtime, non-serializable store for ad-hoc values
        // Last damager, combo, last attack time, minions, etc.
        private Dictionary<string, object> _runtime = new Dictionary<string, object>();

        // API for runtime data
        public void Set<T>(string key, T value)
        {
            _runtime[key] = value;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_runtime.TryGetValue(key, out var o) && o is T t)
            {
                value = t;
                return true;
            }
            value = default;
            return false;
        }

        public T GetOrDefault<T>(string key, T defaultValue = default)
        {
            if (TryGet<T>(key, out var val)) return val;
            return defaultValue;
        }

        public bool HasKey(string key) => _runtime.ContainsKey(key);
    }
}
