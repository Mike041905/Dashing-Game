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


    [System.Serializable]
    public struct Enemy
    {
        public GameObject prefab;
        public float spwanChance;
        public int ticketCost;
        public int minimumLevel;
    }

    public Enemy[] EnemyRoster { get; private set; } = new Enemy[0];
}
