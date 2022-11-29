using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMisslle : PowerUp
{
    [SerializeField] private Missile missilePrefab;

    void Update()
    {
        ExecuteMissile();
    }

    float _missileTimer = 0;
    public void ExecuteMissile()
    {
        if (!Player.Instance.CurrentRoom.ActiveFight) { return; } //check if any enemy alive
        if (_missileTimer < GetStat("Interval").statValue) { _missileTimer += Time.deltaTime; return; }//makes the method run at a certain interval
        else { _missileTimer = 0; }//reset timer
        if (Random.Range(0f, 1f) > GetStat("ChancePerc").statValue) { return; }//roll dice

        Missile missile = Instantiate(missilePrefab, Player.Instance.transform.position, Player.Instance.SpriteRenderer.transform.rotation);//spawn missile

        missile.Target = MikeGameObject.GetClosestTargetWithTag(missile.transform.position, "Enemy").transform;//set missile target
    }
}
