using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipePowerUp : PowerUp
{
    [SerializeField] Slice _slicePrefab;

    private float GetDamage()
    {
        return GetStat("Damage").statValue;
    }
    private float GetWidth()
    {
        return GetStat("Width").statValue;
    }

    float? _damage = null;
    float Damage { get => (_damage ??= GetDamage()) * Upgrade.GetUpgrade("Damage", UpgradeData.VariableType.Float); }

    float? _width = null;
    float Width { get => _width ??= GetWidth(); }

    Vector2 FirstTouch { get => Camera.main.ScreenToWorldPoint(Player.Instance.PlayerDash.FirstTouchPosition); }
    Vector2 SecondTouch { get => Camera.main.ScreenToWorldPoint(Player.Instance.PlayerDash.SecondTouchPosition); }

    public override void UpgradePowerUp(int times = 1)
    {
        base.UpgradePowerUp(times);

        _damage = GetDamage();
        _width = GetWidth();
    }

    private void Start()
    {
        Player.Instance.PlayerDash.OnStartDash += DoSwipe;
    }

    void DoSwipe()
    {
        Slice.SpawnSlice(_slicePrefab, FirstTouch + ((SecondTouch - FirstTouch) / 2), (SecondTouch - FirstTouch).normalized, Damage, Vector2.Distance(FirstTouch, SecondTouch), Width, new string[] { "Player" });
    }
}
