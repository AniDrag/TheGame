using EntityStats;
using EntityStats.Data;
using UnityEngine;

public class TestEntity : MonoBehaviour
{
    public StatsBlock stats;
    public DefaultStatCalculator calc;
    void Start()
    {
        // Optionally assign in code
        if (stats == null)
        {
            Stats baseStats = new Stats();
            baseStats[StatType.VIT] = 10;
            baseStats[StatType.STR] = 5;
            // ... etc.
            Stats growth = new Stats();
            growth[StatType.VIT] = 2;
            // ... etc.
            stats = new StatsBlock(baseStats, growth, calc);
        }

        stats.SetLevel(5);
        Debug.Log($"Health: {stats.MaxHealth}");
    }
}
