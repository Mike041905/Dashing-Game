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
        if (!Player.Instance.CurrentRoom.ActiveFight) { return; } //check if any enemy alive

        Bomb bomb = Instantiate(bombPrefab).GetComponent<Bomb>();//spawn bomb

        bomb.targetPosition = (Vector2)Player.Instance.CurrentRoom.GetRandomEnemy().transform.position + Mike.MikeRandom.RandomVector2(GetStat("Spread").statValue, GetStat("Spread").statValue);
    }
}
