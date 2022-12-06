using UnityEngine;

public abstract class Loot
{
    [Header("Loot Rarity")]
    public float MinimumRarity;
    public float MaximumRarity;
    public float Rarity;
    [Tooltip("Calculated As Weight")] public float DropChance;

    // TODO: Add remaping to rarity calculation based on min and max rarity.
    public virtual float CalculateRelativeWeight(float rarity)
    {
        if (rarity == 0) { return Rarity * DropChance; }
        if (MaximumRarity < rarity) { return 0; }
        if (MinimumRarity > rarity) { return 0; }

        float actualRarity = (MaximumRarity != 0 ? rarity / MaximumRarity : rarity);

        return actualRarity / rarity * DropChance;
    }
}