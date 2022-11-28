using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PiercePowerUp : PowerUp
{
    private void Start()
    {
        Player.Instance.PlayerDash.OnHitEnemy += PierceEnemy;
        Player.Instance.PlayerDash.UseBounce = false;
    }

    async void PierceEnemy(EnemyAI enemy)
    {
        Collider2D[] result = new Collider2D[enemy.Rb.attachedColliderCount];
        enemy.Rb.GetAttachedColliders(result);

        foreach (Collider2D collider in result)
        {
            collider.enabled = false;
        }

        await Task.Delay(200);

        foreach (Collider2D collider in result)
        {
            collider.enabled = true;
        }
    }

    private void OnDestroy()
    {
        Player.Instance.PlayerDash.OnHitEnemy -= PierceEnemy;
        Player.Instance.PlayerDash.UseBounce = true;
    }
}
