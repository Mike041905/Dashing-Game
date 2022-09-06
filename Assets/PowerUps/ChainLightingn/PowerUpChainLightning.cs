using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpChainLightning : PowerUp
{
    [SerializeField] GameObject lightiningPrefab;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Dash>().OnHitEnemy += Electrify;
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
            chainLightning.bounces = (int) GetStat("Bounces").statValue;

            coolDown = .25f;
        }
    }
}
