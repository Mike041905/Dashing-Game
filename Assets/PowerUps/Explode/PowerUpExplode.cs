using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExplode : PowerUp
{
    [SerializeField] private GameObject explosionPrefab;

    void Start()
    {
        Player.Instance.PlayerDash.OnEnemyKilled += Explode;
    }

    void Explode(EnemyAI enemy)
    {
        GameObject go = Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);
        go.transform.localScale = Vector2.one * GetStat("Size").statValue;
        go.GetComponent<Explosion>().radius = GetStat("Size").statValue;
    }
}
