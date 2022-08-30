using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    [System.Serializable]
    public struct Stat
    {
        public string statName;
        public float statValue;
        public float statUpgradeOffset;
        public float statUpgradeMultiplier;
    }

    [Header("ID")]
    public string powerUpName;
    public string description;
    public Sprite icon;

    [Header("Stats")]
    public Stat[] stats;

    public virtual void Upgrade(float times = 1)
    {
        for (int i1 = 0; i1 < stats.Length; i1++)
        {
            for (int i = 0; i < times; i++)
            {
                stats[i1].statValue += stats[i1].statUpgradeOffset + stats[i1].statValue * stats[i1].statUpgradeMultiplier;
            }
        }
    }

    public Stat GetStat(string statName)
    {
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i].statName == statName) { return stats[i]; }
        }

        Debug.LogError($"There is no stat such as {statName}"); return new Stat();
    }

    public string GetUpgradeDifference()
    {
        string result = "";

        for (int i = 0; i < stats.Length; i++)
        {
            result += $"{stats[i].statName}: {stats[i].statValue} -> {stats[i].statValue + stats[i].statUpgradeOffset + stats[i].statValue * stats[i].statUpgradeMultiplier}" + "\n";
        }

        return result;
    }
}
