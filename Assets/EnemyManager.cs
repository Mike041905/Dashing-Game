using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	static EnemyManager _instance;
	public static EnemyManager Instance { get => _instance; }

	private void Awake()
	{
		_instance = this;
	}

    private void Start()
    {
        GameManager.Insatnce.OnLevelChanged += (int _) => { _avaliableEnemies = GetAvaliableEnemies(); };
        GameManager.Insatnce.OnLevelChanged += (int _) => { _avaliableBosses = GetAvaliableBosses(); };
    }

    [System.Serializable]
    public struct Enemy
    {
        public GameObject prefab;
        public float spwanChance;
        public int ticketCost;
        public int minimumLevel;
    }

    [System.Serializable]
    public struct Boss
    {
        public BossAI BossPrefab;
        public float RandomWeight;
        public int MinimumLevel;
        public float EnemySpawnTickets;

        public Enemy[] Enemies;
    }

    [field: SerializeField] public float EnemySpwnTcktDiffMul { get; private set; } = 1.05f;
    public float EnemySpwnTcktMul { get => Mathf.Floor(EnemySpwnTcktDiffMul * GameManager.Insatnce.Difficulty); }
    [field: SerializeField] public float DefaultRoomSpawnTickets { get; private set; } = 20;
    [field: SerializeField] public Enemy[] EnemyRoster { get; private set; } = new Enemy[0];
    [field: SerializeField] public Boss[] BossRoster { get; private set; } = new Boss[0];

    
    List<Enemy> _avaliableEnemies = new();
    public List<Enemy> AvaliableEnemies
    {
        get
        {
            if (_avaliableEnemies.Count == 0)
            {
                _avaliableEnemies = GetAvaliableEnemies();
            }

            return _avaliableEnemies;
        }
    }

    List<Enemy> GetAvaliableEnemies()
    {
        List<Enemy> result = new();

        foreach (Enemy enemy in EnemyRoster)
        {
            if (GameManager.Insatnce.Level >= enemy.minimumLevel) { result.Add(enemy); };
        }

        return result;
    }


    List<Boss> _avaliableBosses = new();
    public List<Boss> AvaliableBosses
    {
        get
        {
            if (_avaliableBosses.Count == 0 || _avaliableBosses == null)
            {
                _avaliableBosses = GetAvaliableBosses();
            }

            return _avaliableBosses;
        }
    }

    List<Boss> GetAvaliableBosses()
    {
        List<Boss> result = new();

        foreach (Boss boss in BossRoster)
        {
            if (GameManager.Insatnce.Level >= boss.MinimumLevel) { result.Add(boss); };
        }

        return result;
    }

    public Boss GetBossForCurrentLevel()
    {
        if(AvaliableBosses.Count == 0) { throw new System.Exception("Boss not Found"); }

        Boss best = AvaliableBosses[0];

        foreach (Boss boss in AvaliableBosses)
        {
            if(best.MinimumLevel < boss.MinimumLevel) { best = boss; }
        }

        // chooses random boss if not found any with explicid level
        // if(best.MinimumLevel < GameManager.Insatnce.Level) { best = AvaliableBosses[Random.Range(0, AvaliableBosses.Count)]; }
        if(best.MinimumLevel < GameManager.Insatnce.Level) { best = AvaliableBosses[MikeRandom.RandomIntByWeights(AvaliableBosses.ToArray(), (Boss b) => b.RandomWeight)]; }

        return best;
    }
}
