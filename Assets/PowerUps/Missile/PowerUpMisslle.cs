using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMisslle : PowerUp
{
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float interval = 1;
    [SerializeField] private float chancePerc = 0.75f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteMissile();
    }

    float missileTimer = 0;
    public void ExecuteMissile()
    {
        if (missileTimer < interval) { missileTimer += Time.deltaTime; return; }//makes the method run at a certain interval
        else { missileTimer = 0; }//reset timer
        if (Random.Range(0f, 1f) < chancePerc) { return; }//roll dice
        if (GameObject.FindGameObjectWithTag("Enemy") == null) { return; }//check if any enemy alive

        Missile missile = Instantiate(missilePrefab, transform.position, Quaternion.identity).GetComponent<Missile>();//spawn missile

        missile.target = MikeGameObject.GetClosestTargetWithTag(missile.transform.position, "Enemy").transform;//set missile target
    }
}
