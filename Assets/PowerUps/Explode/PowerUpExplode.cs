using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExplode : PowerUp
{
    [SerializeField] private GameObject explosionPrefab;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Dash>().OnHitEnemy += Explode;
    }

    private void Update()
    {
        if (coolDown > 0) coolDown -= Time.deltaTime;
    }

    float coolDown = 0;
    void Explode(GameObject hit)
    {
        if(coolDown <= 0)
        {
            GameObject go = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            go.transform.localScale = Vector2.one * GetStat("Size").statValue;
            go.GetComponent<Explosion>().radius = GetStat("Size").statValue;

            coolDown = .25f;
        }
    }
}
