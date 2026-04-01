using System;
using UnityEngine;

namespace EntityStats.Data
{
    /// <summary>
    /// A basic stats class.
    /// Used to hold mass stats in one variable and to easily add them together with utility functions.
    /// Possible expancion: add more stats.
    /// 
    /// Utility outside:
    ///     We add a SubStats class that holds hiden class.
    ///     It can be dependant on this class externaly to calculate substats like:
    ///         max Health = VIT * 10
    ///         max mana = INT * 10
    ///         max stamina = DEX * 10 + VIT * 5
    ///         Physical damage = STR * 2
    ///         Attack speed = DEX * 0.1f
    ///         movementSpeed = DEX * 0.1f + AGI * 0.05f
    ///         and so on.
    /// </summary>
    [Serializable]
    public class Stats : ISerializationCallbackReceiver
    {
        [SerializeField] private int[] _values;

        // Indexer with null safety
        public int this[StatType type]
        {
            get
            {
                EnsureInitialized();
                return _values[(int)type];
            }
            set
            {
                EnsureInitialized();
                _values[(int)type] = value;
            }
        }

        public Stats()
        {
            EnsureInitialized();
        }

        public Stats(Stats other)
        {
            if (other == null) return;
            other.EnsureInitialized();
            _values = (int[])other._values.Clone();
        }

        private void EnsureInitialized()
        {
            if (_values == null || _values.Length != Enum.GetValues(typeof(StatType)).Length)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            int count = Enum.GetValues(typeof(StatType)).Length;
            _values = new int[count];
        }

        // ISerializationCallbackReceiver
        public void OnAfterDeserialize()
        {
            if (_values == null || _values.Length != Enum.GetValues(typeof(StatType)).Length)
                Initialize();
        }

        public void OnBeforeSerialize() { }

        public void Add(Stats other)
        {
            if (other == null) return;
            EnsureInitialized();
            other.EnsureInitialized();
            for (int i = 0; i < _values.Length; i++)
                _values[i] += other._values[i];
        }

        public void Reset()
        {
            EnsureInitialized();
            Array.Clear(_values, 0, _values.Length);
        }

        public static int Count => Enum.GetValues(typeof(StatType)).Length;
    }



    public enum StatType
    {
        VIT,
        STR,
        DEX,
        INT,
        AGI,
        LOL
        // Add more as we need them.
    }   
}