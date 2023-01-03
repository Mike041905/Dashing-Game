using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicePowerUp : PowerUp
{
    private float GetDamage()
    {
        return GetStat("Damage").statValue;
    }
    private float GetWidth()
    {
        return GetStat("Width").statValue;
    }
    private float GetLength()
    {
        return GetStat("Length").statValue;
    }

    [SerializeField] Slice _slicePrefab;

    float? _damage = null;
    float Damage { get => (_damage ??= GetDamage()) * Upgrade.GetUpgrade("Damage", UpgradeData.VariableType.Float); }

    float? _width = null;
    float Width { get => _width ??= GetWidth(); }

    float? _length = null;
    float Length { get => _length ??= GetLength(); }


    private void Start()
    {
        Player.Instance.PlayerDash.OnHitEnemy += OnHit;
    }

    void OnHit(EnemyAI enemy)
    {
        Slice.SpawnSlice(_slicePrefab, Player.Instance.transform.position, (enemy.transform.position - Player.Instance.transform.position).normalized, Damage, Length, Width, new string[] { "Player" });
    }

    public override void UpgradePowerUp(int times = 1)
    {
        base.UpgradePowerUp(times);

        _damage = GetDamage();
        _width = GetWidth();
        _length = GetLength();
    }
}
