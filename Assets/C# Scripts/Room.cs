using UnityEngine;
using Mike;
using EZCameraShake;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public class Room : MonoBehaviour
{
    [Header("Essential")]
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

    //TODO: Put this in a singleton to save on performance
    EnemyManager.Enemy[] avaliableEnemies = new EnemyManager.Enemy[0];
    public EnemyManager.Enemy[] AvaliableEnemies
    {
        get
        {
            if(avaliableEnemies.Length == 0)
            {
                foreach (EnemyManager.Enemy enemy in EnemyManager.Instance.EnemyRoster)
                {
                    if (GameManager.Insatnce.Level >= enemy.minimumLevel) { avaliableEnemies = avaliableEnemies.Append(enemy); };
                }
            }

            return avaliableEnemies;
        }
    }


    //---------------------


    private void Start()
    {
        SetEnemyTickets();
    }


    //---------------------

    public EnemyAI GetRandomEnemy()
    {
        while(true)
        {
            bool alive = false;

            for (int i = 0; i < spawnedEnemies.Length; i++)
            {
                if (spawnedEnemies[i] != null) 
                { 
                    alive = true; 
                    if(UnityEngine.Random.Range(0, spawnedEnemies.Length) <= 1) { return spawnedEnemies[i].GetComponent<EnemyAI>(); }
                }
            }

            if (!alive) { return null; }
        }
    }

    public void ResetDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].Type = Door.DoorType.Barrier;
        }
    }

    protected virtual void SetDoorType(GenerateRoom.Side side, Door.DoorType doorType, bool open = true)
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].side == side) 
            { 
                if(doorType == Door.DoorType.Door) { doors[i].OnEnteredThroughDoor += OnDoorTrigger; }
                doors[i].Type = doorType; 
                doors[i].IsOpen = open; 
            }
        }
    }

    public bool IsNeighbourTo(Room other)
    {
        return (PositionInGrid - other.PositionInGrid).magnitude == 1;
    }

    protected virtual void SetEnemyTickets()
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

    protected virtual void OpenDoors()
    {
        foreach (Door door in doors)
        {
            door.IsOpen = true;
        }
    }

    protected virtual void CloseDoors()
    {
        foreach (Door door in doors)
        {
            door.IsOpen = false;
        }
    }

    public bool CheckIfAllEnemiesDead()
    {
        if(spawnedEnemies.Length == 0) { return false; }

        int enemiesLeft = 0;
        foreach (var item in spawnedEnemies)
        {
            if (item != null && !item.GetComponent<Health>().Dead) { enemiesLeft++; }
        }

        EnemyCounter.Instance.ChangeAmmount(enemiesLeft);
        return enemiesLeft <= 0;
    }

    protected virtual void OnDoorTrigger(Collider2D collider)
    {
        if(!enabled) { return; }
        if(!collider.CompareTag("Player") || Vector2.Distance(collider.transform.position, transform.position) >= 26) { return; }

        CameraShaker.Instance.ShakeOnce(2, 10, .25f, .75f);

        Player.Instance.CurrentRoom = this;

        CloseDoors();
        StartFight();
    }

    protected virtual void StartFight()
    {
        // WTF IS THIS! WHO THOUGHT THIS WAS A GOOD IDEA! oh wait.
        GameObject.FindGameObjectWithTag("RoomText").GetComponent<FightStartFinish>().StartFight();
        SpawnEnemies();
        SpawnCrates();
    }

    protected virtual void SpawnEnemies()
    {
        while (enemySpawnTickets > 0)//run until tickets are depleated
        {
            //initilaze variables
            EnemyManager.Enemy enemy = AvaliableEnemies[0];
            Vector2 spawnPosition = Mike.MikeRandom.RandomVector2(-20, 20, -20, 20) + (Vector2)transform.position;
            float[] weights = new float[0];

            //asign random enemy baised on their spawn chance
            foreach (EnemyManager.Enemy item in AvaliableEnemies)
            {
                weights = MikeArray.Append(weights, item.spwanChance);
            }

            enemy = AvaliableEnemies[MikeRandom.RandomIntByWeights(weights)];

            //set random position until the distance between the player and the spawn position is more than 10
            while (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, spawnPosition) <= 15)
            {
                spawnPosition = Mike.MikeRandom.RandomVector2(-20, 20, -20, 20) + (Vector2)transform.position;
            }

            //check if enemy span tickets are suficien to spawn currenly selected enemy
            if(enemySpawnTickets - enemy.ticketCost < 0) { return; }

            //spawn Enemy
            EnemyAI newEnemy = Instantiate(enemy.prefab, spawnPosition, Quaternion.identity).GetComponent<EnemyAI>();
            newEnemy.difficultyMultiplier = GameManager.Insatnce.Difficulty;
            newEnemy.room = this;

            //add spawned enemy to array
            spawnedEnemies = MikeArray.Append(spawnedEnemies, newEnemy.gameObject);
            
            //remove tickets
            enemySpawnTickets -= enemy.ticketCost;
        }

        EnemyCounter.Instance.ChangeAmmount(spawnedEnemies.Length);
    }

    protected virtual void SpawnCrates()
    {
        for (int i = 0; i < maxCrates; i++)
        {
            if (UnityEngine.Random.Range(0, 100) > crateSpawnChance) { break; }

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

    public GenerateRoom.Side GetRelativeSideTo(Room other)
    {
        Vector2Int relativePos = other.PositionInGrid - PositionInGrid;

        if(relativePos.y > 0) { return GenerateRoom.Side.Top; }
        else if (relativePos.x > 0) { return GenerateRoom.Side.right; }
        else if (relativePos.y < 0) { return GenerateRoom.Side.bottom; }
        else { return GenerateRoom.Side.left; }
    }

    public BossRoom ChangeToBossRoom()
    {
        Destroy(this);
        return gameObject.AddComponent<BossRoom>().Initialize(doors);
    }
}
