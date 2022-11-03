using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mike;
using EZCameraShake;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public struct Enemy
    {
        public GameObject prefab;
        public float spwanChance;
        public int ticketLossPerSpawnedUnit;
    }

    [Header("Essential")]
    [SerializeField] private Enemy[] enemies = new Enemy[0];
    [SerializeField] Door[] doors;
    [SerializeField] GameObject connectorPrefab;

    [Header("Options")]
    [SerializeField] private int enemySpawnTickets = 20;
    [SerializeField] private GameObject crate;
    [Range(0, 100)] [SerializeField] private float crateSpawnChance = 20;
    [SerializeField] private int maxCrates = 20;


    //---------------------

    [HideInInspector] public int descendant = 1;
    private GameObject[] spawnedEnemies = new GameObject[0];

    public Vector2Int PositionInGrid 
    {
        get => new(Mathf.RoundToInt(transform.position.x / GenerateRoom.spawnDistance), Mathf.RoundToInt(transform.position.y / GenerateRoom.spawnDistance));
    }

    public const float ROOM_SIZE_X = 50f;


    //---------------------


    private void Start()
    {
        SetEnemyTickets();
    }


    //---------------------


    public void ResetDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].Type = Door.DoorType.Barrier;
        }
    }

    void SetDoorType(GenerateRoom.Side side, Door.DoorType doorType, bool open = true)
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].side == side) 
            { 
                if(doorType == Door.DoorType.Door) { doors[i].OnEnteredThroughDoor += ReciveTrigger; }
                doors[i].Type = doorType; 
                doors[i].IsOpen = open; 
            }
        }
    }

    public bool IsNeighbourTo(Room other)
    {
        return (PositionInGrid - other.PositionInGrid).magnitude == 1;
    }

    void SetEnemyTickets()
    {
        enemySpawnTickets = Mathf.RoundToInt(enemySpawnTickets + GameManager.Insatnce.Level * 0.05f);
    }

    public void EndFightIfEnemiesDead() 
    {
        if (CheckIfAllEnemiesDead())
        {
            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
            foreach (GameObject item in coins)
            {
                item.GetComponent<Item>().homingRange = 200;
                item.GetComponent<Item>().homingSpeed = 60;
            }

            GameObject.FindGameObjectWithTag("RoomText").GetComponent<FightStartFinish>().EndFight();
            OpenDoors();
            transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(.25f, .25f, .25f);
            GameManager.Insatnce.TrySpawnPortal(this);
            enabled = false;
        }
    }

    void OpenDoors()
    {
        foreach (Door door in doors)
        {
            door.IsOpen = true;
        }
    }
    
    void CloseDoors()
    {
        foreach (Door door in doors)
        {
            door.IsOpen = false;
        }
    }

    bool CheckIfAllEnemiesDead()
    {
        if(spawnedEnemies.Length == 0) { return false; }

        foreach (var item in spawnedEnemies)
        {
            if (item != null && !item.GetComponent<Health>().Dead) { return false; }
        }

        return true;
    }

    public void ReciveTrigger(Collider2D collider)
    {
        if(!enabled) { return; }
        if(!collider.CompareTag("Player") || Vector2.Distance(collider.transform.position, transform.position) >= 26) { return; }

        CameraShaker.Instance.ShakeOnce(1, 15, .25f, .25f);
        
        CloseDoors();
        StartFight();
    }

    void StartFight()
    {
        GameObject.FindGameObjectWithTag("RoomText").GetComponent<FightStartFinish>().StartFight();
        SpawnEnemies();
        SpawnCrates();
    }

    void SpawnEnemies()
    {
        while (enemySpawnTickets > 0)//run until tickets are depleated
        {
            //initilaze variables
            Enemy enemy = enemies[0];
            Vector2 spawnPosition = Mike.MikeRandom.RandomVector2(-20, 20, -20, 20) + (Vector2)transform.position;
            float[] weights = new float[0];

            //asign random enemy baised on their spawn chance
            foreach (Enemy item in enemies)
            {
                weights = MikeArray.Append(weights, item.spwanChance);
            }

            enemy = enemies[MikeRandom.RandomIntByWeights(weights)];

            //set random position until the distance between the player and the spawn position is more than 10
            while (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, spawnPosition) <= 15)
            {
                spawnPosition = Mike.MikeRandom.RandomVector2(-20, 20, -20, 20) + (Vector2)transform.position;
            }

            //check if enemy span tickets are suficien to spawn currenly selected enemy
            if(enemySpawnTickets - enemy.ticketLossPerSpawnedUnit < 0) { return; }

            //spawn Enemy
            EnemyAI newEnemy = Instantiate(enemy.prefab, spawnPosition, Quaternion.identity).GetComponent<EnemyAI>();
            newEnemy.difficultyMultiplier = GameManager.Insatnce.Difficulty;
            newEnemy.room = this;

            //add spawned enemy to array
            spawnedEnemies = MikeArray.Append(spawnedEnemies, newEnemy.gameObject);
            
            //remove tickets
            enemySpawnTickets -= enemy.ticketLossPerSpawnedUnit;
        }
    }

    void SpawnCrates()
    {
        for (int i = 0; i < maxCrates; i++)
        {
            if (Random.Range(0, 100) > crateSpawnChance) { break; }

            Vector2 spawnPosition = Mike.MikeRandom.RandomVector2(-20, 20, -20, 20) + (Vector2)transform.position;

            //set random position until the distance between the player and the spawn position is more than 10
            while (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, spawnPosition) <= 10)
            {
                spawnPosition = Mike.MikeRandom.RandomVector2(-20, 20, -20, 20) + (Vector2)transform.position;
            }

            Instantiate(crate, spawnPosition, Mike.MikeRandom.RandomAngle(0, 360));
        }
    }

    public void ConnectRoom(Room other, bool spawnConnector = true)
    {
        if (!IsNeighbourTo(other)) { return; }

        GenerateRoom.Side sideTwardsOther = GetRelativeSideTo(other);
        if (spawnConnector)
        {
            // this is hard to read :/

            Vector2 spawnPosition = transform.position;

            // Add room size offset acounting for relative position to other room (this wont work if [scale.x ≠ scale.y])
            spawnPosition += (Vector2)(other.PositionInGrid - PositionInGrid) * (ROOM_SIZE_X / 2);

            // Add corridor size offset acounting for relative position to other room
            spawnPosition += (Vector2)(other.PositionInGrid - PositionInGrid) * (connectorPrefab.transform.lossyScale.y / 2);

            Instantiate(connectorPrefab, spawnPosition, Quaternion.Euler(0, 0, (int)sideTwardsOther % 2 == 0 ? 0 : 90));
            other.ConnectRoom(this, false);
        }

        SetDoorType(sideTwardsOther, Door.DoorType.Door);
    }

    GenerateRoom.Side GetRelativeSideTo(Room other)
    {
        Vector2Int relativePos = other.PositionInGrid - PositionInGrid;

        if(relativePos.y > 0) { return GenerateRoom.Side.Top; }
        else if (relativePos.x > 0) { return GenerateRoom.Side.right; }
        else if (relativePos.y < 0) { return GenerateRoom.Side.bottom; }
        else { return GenerateRoom.Side.left; }
    }
}
