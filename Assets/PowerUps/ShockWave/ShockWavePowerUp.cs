using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWavePowerUp : PowerUp
{
    [SerializeField] ParticleSystem _shockWave;
    [SerializeField] float _cooldown = .5f;

    float? _damageMultiplier = null;
    float DamageMultiplier { get => _damageMultiplier ??= Upgrade.GetUpgrade("Damage", UpgradeData.VariableType.Float); }
    float Damage { get => GetStat("Damage").statValue * DamageMultiplier; }

    float _cooldownTimer = 0;


    void Start()
    {
        Player.Instance.PlayerHealth.OnTakeDamage += (h,g) => SendShockWave();

        UpdateShockWaveSize();
    }

    private void Update()
    {
        if(_cooldownTimer < _cooldown) { _cooldownTimer += Time.deltaTime; }
    }

    public override void UpgradePowerUp(int times = 1)
    {
        base.UpgradePowerUp(times);

        UpdateShockWaveSize();
    }

    void SendShockWave()
    {
        if(_cooldownTimer < _cooldown) { return; }
        _cooldownTimer = 0;

        if(_shockWave != null) { _shockWave.Play(); }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, GetStat("Radius").statValue);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out EnemyAI e))
            {
                e.EnemyHealth.TakeDamage(Damage);
                e.Rb.AddForce((e.Rb.position - (Vector2)transform.position).normalized * GetStat("Force").statValue, ForceMode2D.Impulse);
            }
        }
    }

    void UpdateShockWaveSize()
    {
        _shockWave.transform.localScale = Vector3.one * GetStat("Radius").statValue;
    }
}
