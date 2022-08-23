using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBombardment : PowerUp
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float interval = 1;
    [SerializeField] private float chancePerc = 0.75f;

    //----------------

    void Update()
    {
        ExecuteBombardment();
    }

    //---------------

    float bombardmentTimer = 0;
    public void ExecuteBombardment()
    {
        if (bombardmentTimer < interval) { bombardmentTimer += Time.deltaTime; return; } //makes the method run at a certain interval
        else { bombardmentTimer = 0; } //reset timer
        if (Random.Range(0f, 1f) < chancePerc) { return; } //roll dice
        if (GameObject.FindGameObjectWithTag("Enemy") == null) { return; } //check if any enemy alive

        Vector2 currentRoom = Mike.MikeGameObject.GetClosestTargetWithTag(transform.position, "Room").transform.position;

        for (int i = 0; i < 25; i++)
        {
            Bomb bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity).GetComponent<Bomb>();//spawn bomb

            bomb.targetPosition = currentRoom + Mike.MikeRandom.RandomVector2(-20, 20, -20, 20);
        }
    }
}
