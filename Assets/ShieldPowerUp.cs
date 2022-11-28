using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShieldPowerUp : PowerUp
{
    [SerializeField] Health _shieldHealth;
    [SerializeField] float _rechargeDelay;

    Task _recharge;

    private void Start()
    {
        UpdateMaxHealth();

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
        _shieldHealth.SetMaxHealth(Upgrade.GetUpgrade("Health", UpgradeData.VariableType.Float) * 0.01f * GetStat("Shield Health").statValue, false);
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
            if (!_recharge.IsCompleted) { _recharge.Dispose(); }
            _recharge = Recharge();
        }
    }

    async Task Recharge()
    {
        await Task.Delay((int)(_rechargeDelay * 1000));

        while (_shieldHealth.CurrentHealth < _shieldHealth.Maxhealth)
        {
            _shieldHealth.CurrentHealth += GetStat("Recharge Speed").statValue * Time.deltaTime;

            await Task.Yield();
        }
    }
}
