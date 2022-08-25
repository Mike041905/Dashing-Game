using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpChainLightning : PowerUp
{
    [SerializeField] GameObject lightiningPrefab;
    public float damage = 3;
    public int bounces = 2;

    [SerializeField] private float damageUpgrade = 0.1f;
    [SerializeField] private int bouncesUpgrade = 1;

    void Start()
    {
        PowerUpChainLightning[] powerUps = transform.parent.GetComponentsInChildren<PowerUpChainLightning>();
        if (powerUps.Length > 1)
        {
            foreach (PowerUpChainLightning powerUp in powerUps)
            {
                powerUp.bounces += bouncesUpgrade;
                powerUp.damage += damageUpgrade;
            }

            Destroy(gameObject);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Dash>().OnHitEnemy += Electrify;
        }
    }

    private void Update()
    {
        if (coolDown > 0) coolDown -= Time.deltaTime;
    }

    float coolDown = 0;
    void Electrify(GameObject hit)
    {
        if(coolDown <= 0)
        {
            ChainLightning chainLightning = Instantiate(lightiningPrefab, transform.position, Quaternion.identity).GetComponent<ChainLightning>();
            chainLightning.damage = damage;
            chainLightning.bounces = bounces;

            coolDown = .25f;
        }
    }
}
