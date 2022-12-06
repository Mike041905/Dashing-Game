using UnityEngine;

public abstract class LootMonoBehaviour : MonoBehaviour
{
    [Header("Loot Rarity")]
    public float MinimumRarity;
    public float MaximumRarity;
    public float Rarity;
    [Tooltip("Calculated As Weight")] public float DropChance;


    public virtual float CalculateRelativeWeight(float rarity)
    {
        // I'm too stupid for math imma just hardcode the shit out of this

        if (rarity == 0) { return Rarity * DropChance; }
        if (MaximumRarity > 0 && MaximumRarity < rarity) { return 0; }
        if (MinimumRarity > rarity) { return 0; }

        float prc = Mathf.Min(Rarity, rarity) / Mathf.Max(Rarity, rarity);

        return Mathf.Pow(prc, 4) * DropChance;
    }
}