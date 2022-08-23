using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject collectionEffect;
    public int coinsPerPickup = 0;
    public int gemsPerPickup = 0;
    public float homingRange = 0;
    public float homingSpeed = 5;
    [SerializeField] private float activationDelay = 0;

    [Header("PowerUp")]
    [SerializeField] private bool _isPowerUp = false;
    [SerializeField] private PowerUp[] _powerUpLootTable;
    [SerializeField] private int _choices = 3;

    float timer = 0;
    Transform player;
    private void Start()
    {
        if(player == null) { return; }
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(player == null) { return; }

        timer += activationDelay > timer ? Time.deltaTime : 0;

        if(activationDelay > timer) { return; }

        if(Vector2.Distance(transform.position, player.position) <= homingRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, homingSpeed * (1 - (Vector2.Distance(transform.position, player.position) / homingRange)) * Time.deltaTime);
        }
        
        if(Vector2.Distance(transform.position, player.position) <= 0.1f) { CollectItem(player.GetComponent<Collider2D>()); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectItem(collision);
    }

    void CollectItem(Collider2D playerCollider)
    {
        if (activationDelay > timer) { return; }
        if (!playerCollider.CompareTag("Player")) { return; }

        if (_isPowerUp)
        {


            /*playerCollider.GetComponent<PowerUpManager>().AddUpgrade();*/
        }

        GameManager.Insatnce.AddCoins(coinsPerPickup);

        if (collectionEffect != null) Instantiate(collectionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
