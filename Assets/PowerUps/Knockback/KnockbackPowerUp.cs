using UnityEngine;

public class KnockbackPowerUp : PowerUp
{
    float Force { get => GetStat("Force").statValue; }

    private void Start()
    {
        Player.Instance.PlayerDash.OnHitEnemy += OnHit;
    }

    void OnHit(EnemyAI enemy)
    {
        enemy.Rb.AddForce((enemy.transform.position - Player.Instance.transform.position).normalized * Force, ForceMode2D.Impulse);
    }
}
