using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExplode : PowerUp
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float size;
    [SerializeField] private float sizeUpgradeValue = .25f;

    void Start()
    {
        PowerUpExplode[] explosions = transform.parent.GetComponentsInChildren<PowerUpExplode>();
        if (explosions.Length > 0)
        {
            foreach (PowerUpExplode explosion in explosions)
            {
                explosion.size *= 1 + sizeUpgradeValue;
            }

            Destroy(gameObject);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Dash>().OnHitEnemy += Explode;
        }
    }

    void Explode(GameObject hit)
    {
        GameObject go = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = Vector2.one * size;
        go.GetComponent<Explosion>().radius = size;
    }
}
