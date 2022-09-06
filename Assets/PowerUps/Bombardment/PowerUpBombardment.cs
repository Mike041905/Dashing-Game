using UnityEngine;

public class PowerUpBombardment : PowerUp
{
    [Header("References")]
    [SerializeField] private GameObject bombPrefab;

    //----------------

    private void Update()
    {
        ExecuteBombardment();
    }

    //---------------

    private float bombardmentTimer = 0;
    public void ExecuteBombardment()
    {
        if (bombardmentTimer < GetStat("Interval").statValue) { bombardmentTimer += Time.deltaTime; return; } //makes the method run at a certain interval
        else { bombardmentTimer = 0; } //reset timer
        if (Random.Range(0f, 1f) > GetStat("ChancePerc").statValue) { return; } //roll dice
        if (GameObject.FindGameObjectWithTag("Enemy") == null) { return; } //check if any enemy alive

        Vector2 currentRoom = Mike.MikeGameObject.GetClosestTargetWithTag(transform.position, "Room").transform.position;

        Bomb bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity).GetComponent<Bomb>();//spawn bomb

        bomb.targetPosition = currentRoom + Mike.MikeRandom.RandomVector2(-20, 20, -20, 20);
    }
}
