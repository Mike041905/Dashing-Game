using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMisslle : PowerUp
{
    [SerializeField] private GameObject missilePrefab;

    void Update()
    {
        ExecuteMissile();
    }

    float missileTimer = 0;
    public void ExecuteMissile()
    {
        if (missileTimer < GetStat("Interval").statValue) { missileTimer += Time.deltaTime; return; }//makes the method run at a certain interval
        else { missileTimer = 0; }//reset timer
        if (Random.Range(0f, 1f) > GetStat("ChancePerc").statValue) { return; }//roll dice
        if (GameObject.FindGameObjectWithTag("Enemy") == null) { return; }//check if any enemy alive

        Missile missile = Instantiate(missilePrefab, transform.position, transform.rotation).GetComponent<Missile>();//spawn missile

        missile.target = MikeGameObject.GetClosestTargetWithTag(missile.transform.position, "Enemy").transform;//set missile target
    }
}
