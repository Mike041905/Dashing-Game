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
    [SerializeField] protected Door[] doors;
    [SerializeField] GameObject connectorPrefab;

    [Header("Options")]
    [SerializeField] private GameObject crate;
    [Range(0, 100)] [SerializeField] private float crateSpawnChance = 20;
    [SerializeField] private int maxCrates = 20;

    [Header("MiniMap")]
    [SerializeField] protected SpriteRenderer _minimapIcon;
    [SerializeField] protected Color _normalColor = new(.2f, .2f, .2f);
    [SerializeField] protected Color _completedColor = new(.1f, .1f, .1f);


    //---------------------

    [HideInInspector] public int descendant = 0;
    protected GameObject[] spawnedEnemies = new GameObject[0];

    public Vector2Int PositionInGrid 
    {
        get => new(Mathf.RoundToInt(transform.position.x / GenerateRoom.spawnDistance), Mathf.RoundToInt(transform.position.y / GenerateRoom.spawnDistance));
    }

    public const float ROOM_SIZE_X = 50f;
    public bool ActiveFight { get => enabled && spawnedEnemies.Length > 0; }


    //---------------------

    private void Start()
    {
        _minimapIcon.color = _normalColor;
    }

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

    public virtual void EndFightIfEnemiesDead() 
    {
        if(!CheckIfAllEnemiesDead()) { return; }

        _minimapIcon.color = _completedColor;

        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject item in coins)
        {
            item.GetComponent<Item>().homingRange = 200;
            item.GetComponent<Item>().homingSpeed = 60;
        }

        FightStartFinish.Instance.EndFight();
        OpenDoors();
        enabled = false;
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
        foreach (GameObject item in spawnedEnemies)
        {
            if (item != null && !item.GetComponent<Health>().Dead) { enemiesLeft++; }
        }

        EnemyCounter.Instance.ChangeAmmount(enemiesLeft);
        return enemiesLeft <= 0;
    }

    protected virtual void OnDoorTrigger(Collider2D collider)
    {
        if(this == null) { return; }
        if(!enabled) { _minimapIcon.color = _completedColor; return; }
        if(!collider.CompareTag("Player") || Vector2.Distance(collider.transform.position, transform.position) >= 26) { return; }
        if(spawnedEnemies.Length > 0) { return; }

        CameraShaker.Instance.ShakeOnce(2, 10, .25f, .75f);

        Player.Instance.CurrentRoom = this;

        CloseDoors();
        StartFight();
    }

    protected virtual void StartFight()
    {
        FightStartFinish.Instance.StartFight();

        SpawnEnemies(EnemyManager.Instance.AvaliableEnemies, EnemyManager.Instance.GetSpawnTickets());
        SpawnCrates();
    }

    protected virtual void SpawnEnemies(List<EnemyManager.Enemy> enemies, float tickets)
    {
        int InsufficentTicketsCounter = 0;
        while (tickets > 0 && InsufficentTicketsCounter < 5) //run until tickets are depleated or couldn't find cheapest enemy (5x)
        {
            //initilaze variables
            Vector2 spawnPosition = MikeRandom.RandomVector2(-20, 20) + (Vector2)transform.position;
            float[] weights = new float[0];

            //asign random enemy baised on their spawn chance
            foreach (EnemyManager.Enemy item in enemies)
            {
                weights = MikeArray.Append(weights, item.spwanChance);
            }

            EnemyManager.Enemy enemy = enemies[MikeRandom.RandomIntByWeights(weights)];

            //set random position until the distance between the player and the spawn position is more than 10
            while (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, spawnPosition) <= 15)
            {
                spawnPosition = Mike.MikeRandom.RandomVector2(-20, 20, -20, 20) + (Vector2)transform.position;
            }

            //check if enemy spawn tickets are suficien to spawn currenly selected enemy
            if(tickets - enemy.ticketCost < 0) { InsufficentTicketsCounter++; continue; }

            //spawn Enemy
            EnemyAI newEnemy = Instantiate(enemy.prefab, spawnPosition, Quaternion.identity).GetComponent<EnemyAI>();
            newEnemy.Initialize(this, GameManager.Insatnce.Difficulty);

            //add spawned enemy to array
            spawnedEnemies = MikeArray.Append(spawnedEnemies, newEnemy.gameObject);
            
            //remove tickets
            tickets -= enemy.ticketCost;
        }

        EnemyCounter.Instance.ChangeAmmount(spawnedEnemies.Length);
    }

    protected virtual void SpawnCrates()
    {
        if(crate == null) { return; }

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
        BossRoom bossRoom = gameObject.AddComponent<BossRoom>().Initialize(doors, _minimapIcon, _normalColor, _completedColor);
        Destroy(this);

        return bossRoom;
    }
}
