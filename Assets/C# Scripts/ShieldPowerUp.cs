using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ShieldPowerUp : PowerUp
{
    [SerializeField] Health _shieldHealth;
    [SerializeField] float _rechargeDelay;

    Task _recharge;
    CancellationTokenSource _cancellationToken = new();

    float RechargeSpeed { get => Upgrade.GetUpgrade("Health", UpgradeData.VariableType.Float) * 0.01f * GetStat("Recharge Speed").statValue; }
    float ShieldMaxHealth { get => Upgrade.GetUpgrade("Health", UpgradeData.VariableType.Float) * 0.01f * GetStat("Shield Health").statValue; }

    private void Start()
    {
        UpdateMaxHealth();
        _shieldHealth.HealToMax();

        _shieldHealth.OnDeath += OnShieldDown;
        _shieldHealth.OnTakeDamage += (h, g) => OnHit();
    }

    public override void UpgradePowerUp(int times = 1)
    {
        base.UpgradePowerUp(times);

        UpdateMaxHealth();
    }

    void UpdateMaxHealth()
    {
        _shieldHealth.SetMaxHealth(ShieldMaxHealth, false);
    }

    async void OnShieldDown()
    {
        await Task.Delay((int) (GetStat("Recovery Time").statValue * 1000));

        if(!isActiveAndEnabled) { return; }

        _shieldHealth.Revive();
    }

    void OnHit()
    {
        if(_recharge != null) 
        {
            if (!_recharge.IsCompleted) { _cancellationToken.Cancel(); _cancellationToken = new(); }
        }

        _recharge = Recharge(_cancellationToken.Token);
    }

    async Task Recharge(CancellationToken token)
    {
        await Task.Delay((int)(_rechargeDelay * 1000), token);

        while (_shieldHealth.CurrentHealth < _shieldHealth.Maxhealth && !token.IsCancellationRequested)
        {
            _shieldHealth.CurrentHealth += RechargeSpeed * Time.deltaTime;

            await Task.Yield();
        }
    }
}
