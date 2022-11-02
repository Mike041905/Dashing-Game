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

    [Header("Options")]
    [SerializeField] private int enemySpawnTickets = 20;
    [SerializeField] private GameObject crate;
    [Range(0, 100)] [SerializeField] private float crateSpawnChance = 20;
    [SerializeField] private int maxCrates = 20;


    //---------------------

    [HideInInspector] public int descendant = 1;
    private bool[] doors = new bool[4];
    private GameObject[] spawnedEnemies = new GameObject[0];


    //---------------------


    private void Start()
    {
        SetInitialDoorStates();
        SetEnemyTickets();
    }


    //---------------------


    void SetEnemyTickets()
    {
        enemySpawnTickets = Mathf.RoundToInt(enemySpawnTickets + GameManager.Insatnce.Level * 0.05f);
    }

    public void SetInitialDoorStates()
    {
        transform.GetChild(0).rotation = Quaternion.Euler(Vector3.zero);

        for (int i = 0; i < 4; i++)
        {
            doors[i] = transform.GetChild(0).GetChild(i).GetComponent<BoxCollider2D>().isTrigger;
            if (transform.GetChild(0).GetChild(i).GetComponent<BoxCollider2D>().isTrigger) { transform.GetChild(0).GetChild(i).tag = "Door"; }
        }
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
            OpenOpenableDoors();
            transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(.25f, .25f, .25f);
            GameManager.Insatnce.SpawnPortal(this);
            enabled = false;
        }
    }

    void OpenOpenableDoors()
    {
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<BoxCollider2D>().isTrigger = doors[i];
            if (doors[i]) transform.GetChild(0).GetChild(i).tag = "Door";
            if (doors[i]) transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(false);
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

        foreach (Transform item in transform.GetChild(0))
        {
            if(item.CompareTag("Door")) item.GetChild(0).gameObject.SetActive(true);

            item.GetComponent<BoxCollider2D>().isTrigger = false;
            item.tag = "Barrier";
        }
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
}
