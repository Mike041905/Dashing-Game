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
        GameManager.Insatnce.OnLevelChanged += (int _) => { GetAvaliableEnemies(); };
    }

    [System.Serializable]
    public struct Enemy
    {
        public GameObject prefab;
        public float spwanChance;
        public int ticketCost;
        public int minimumLevel;
    }

    [field: SerializeField] public Enemy[] EnemyRoster { get; private set; } = new Enemy[0];

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
}
