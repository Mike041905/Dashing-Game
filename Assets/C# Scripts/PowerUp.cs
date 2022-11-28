using Mike;
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
        public int statLevel;
    }

    [Header("ID")]
    public string powerUpName;
    public string LongDescription;
    public string description;
    public Sprite icon;
    public Sprite[] Images;

    [Header("Stats")]
    public Stat[] stats;
    public int PowerUpLevel = 1;
    [Tooltip("Values 0 and below mean infinity")] public int MaxLevel = 0;
    public bool HasReachedMaxLevel { get => MaxLevel > 0 && MaxLevel <= PowerUpLevel; }

    public virtual void UpgradePowerUp(int times = 1)
    {
        PowerUpLevel += times;

        for (int i1 = 0; i1 < stats.Length; i1++)
        {
            for (int i = 0; i < times; i++)
            {
                stats[i1].statValue += stats[i1].statUpgradeOffset + stats[i1].statValue * stats[i1].statUpgradeMultiplier;
                stats[i1].statLevel++;
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
            result += $"{stats[i].statName}: {MikeString.ConvertNumberToString(stats[i].statValue)} -> {MikeString.ConvertNumberToString(stats[i].statValue + stats[i].statUpgradeOffset + stats[i].statValue * stats[i].statUpgradeMultiplier)}" + "\n";
        }

        return result;
    }
}
