using UnityEngine;
using EntityStats.Data;
using System;
namespace EntityStats
{
    /// <summary>
    /// Pourpous:
    ///     A whole stat holder class that is on a Character Entity.
    ///     THis class can have functionality for leveling up,
    ///     Growth factors,
    ///     Starter Stats,
    ///     
    /// Acts as a Main used component for stats on a character or anything that needs multiple stat inputs.
    ///     
    /// </summary>

    [Serializable]
    public class StatsBlock
    {
        [SerializeField] private Stats _baseStats;
        [SerializeField] private Stats _growthFactors;
        [SerializeField] private Stats _equipmentBonuses;
        [SerializeField] private int _level = 1;

        [NonSerialized] private IStatCalculator _calculator;
        [NonSerialized] private Stats _cachedCurrentStats;

       // remove the null refrences we get
        private void EnsureInitialized()
        {
            if (_calculator == null)
                _calculator = new DefaultStatCalculator();

            if (_cachedCurrentStats == null)
            {
                _cachedCurrentStats = new Stats();
                Recalculate();
            }
        }

        public Stats CurrentStats
        {
            get
            {
                EnsureInitialized();
                return _cachedCurrentStats;
            }
        }

        public Stats BaseStats => _baseStats;
        public Stats GrowthFactors => _growthFactors;
        public Stats EquipmentBonuses => _equipmentBonuses;
        public int Level => _level;

        // Public constructor for code creation
        public StatsBlock(Stats baseStats, Stats growthFactors, IStatCalculator calculator = null)
        {
            _baseStats = new Stats(baseStats);
            _growthFactors = new Stats(growthFactors);
            _equipmentBonuses = new Stats();
            _level = 1;
            _calculator = calculator ?? new DefaultStatCalculator();
            Recalculate();
        }

        public void SetLevel(int level)
        {
            _level = level;
            Recalculate();
        }

        public void ApplyEquipmentBonuses(Stats bonuses)
        {
            _equipmentBonuses = new Stats(bonuses);
            Recalculate();
        }

        public void Recalculate()
        {
            EnsureInitialized(); 
            _cachedCurrentStats.Reset();

            int levelBonus = _level - 1;

            for (int i = 0; i < Stats.Count; i++)
            {
                StatType stat = (StatType)i;
                int baseVal = _baseStats?[stat] ?? 0;
                int growthVal = (_growthFactors?[stat] ?? 0) * levelBonus;
                int equipVal = _equipmentBonuses?[stat] ?? 0;
                _cachedCurrentStats[stat] = baseVal + growthVal + equipVal;
            }
        }

        public float GetDerivedStat(CalculatedStatType type)
        {
            EnsureInitialized();
            return _calculator.GetDerivedStat(type, CurrentStats);
        }

        public float MaxHealth => GetDerivedStat(CalculatedStatType.MaxHealth);
        public float MaxMana => GetDerivedStat(CalculatedStatType.MaxMana);
        public float MaxStamina => GetDerivedStat(CalculatedStatType.MaxStamina);
        public float PhysicalDamage => GetDerivedStat(CalculatedStatType.BasePhysicalDamage);
        public float AttackSpeed => GetDerivedStat(CalculatedStatType.AttackSpeed);
        public float MovementSpeed => GetDerivedStat(CalculatedStatType.MovementSpeed);

        public void SetCalculator(IStatCalculator calculator) => _calculator = calculator;
    }
}