using System;
using System.Collections.Generic;

namespace EntityStats.Data
{
    public interface IStatCalculator
    {
        float GetDerivedStat(CalculatedStatType name, Stats baseStats);
    }
    
    /// <summary>
    /// See this is jsut pure stats. No real growth mod. We can add a CharacterStatCalculator and it can be as a SO? potentialy asign it and the when choosing a class
    /// We make the player start with it and have new growth factors?? or we can jsut keep it default no growth and do itt like in eldenring?
    /// </summary>
    public class DefaultStatCalculator : IStatCalculator
    {
        private readonly Dictionary<CalculatedStatType, Func<Stats, float>> _formulas;

        public DefaultStatCalculator()
        {
            _formulas = new Dictionary<CalculatedStatType, Func<Stats, float>>
            {
                { CalculatedStatType.MaxHealth, s => s[StatType.VIT] * 10f },
                { CalculatedStatType.MaxMana, s => s[StatType.INT] * 10f },
                { CalculatedStatType.MaxStamina, s => s[StatType.DEX] * 10f + s[StatType.VIT] * 5f },
                { CalculatedStatType.BasePhysicalDamage, s => s[StatType.STR] * 2f },
                { CalculatedStatType.AttackSpeed, s => s[StatType.DEX] * 0.1f },
                { CalculatedStatType.MovementSpeed, s => s[StatType.DEX] * 0.1f + s[StatType.AGI] * 0.05f },
                { CalculatedStatType.Poop, s => s[StatType.DEX] * 0.1f + s[StatType.AGI] * 0.05f }

            };
        }

        public float GetDerivedStat(CalculatedStatType name, Stats stats)
        {
            return _formulas.TryGetValue(name, out var formula) ? formula(stats) : 0f;
        }
    }

    public enum CalculatedStatType
    {
        MaxHealth,
        MaxStamina,
        MaxMana,
        BasePhysicalDamage,
        AttackSpeed,
        MovementSpeed,    
        Poop
    }
}