using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [System.Serializable]
    public struct Loot
    {
        public int weight;
        public GameObject item;
    }

    [Header("General")]
    [SerializeField] private GameObject collectionEffect;
    public int coinsPerPickup = 0;
    public int gemsPerPickup = 0;
    public float homingRange = 0;
    public float homingSpeed = 5;
    [SerializeField] private float activationDelay = 0;

    [Header("PowerUp")]
    [SerializeField] private bool _isPowerUp = false;
    [SerializeField] private Loot[] _powerUpLootTable;
    [SerializeField] private int _choices = 3;

    float timer = 0;
    Transform player;
    private void Start()
    {
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
            transform.position = new(transform.position.x, transform.position.y, -1);
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

        if (_isPowerUp) { ChoosePowerUp(); }

        GameManager.Insatnce.AddCoins(coinsPerPickup);
        GameManager.Insatnce.AddGems(gemsPerPickup);

        if (collectionEffect != null) Instantiate(collectionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // required for further use
    int[] choiceIndexes;
    void ChoosePowerUp()
    {
        // percaucion
        if (_choices > _powerUpLootTable.Length) { Debug.LogError("Loot Table too small"); return; }

        // Get Loot Weights
        float[] weights = new float[_powerUpLootTable.Length];
        for (int i = 0; i < _powerUpLootTable.Length; i++)
        {
            weights[i] = _powerUpLootTable[i].weight;
        }

        choiceIndexes = new int[_choices];
        for (int i = 0; i < choiceIndexes.Length; i++)
        {
            choiceIndexes[i] = -1;
        }

        // Generates Choices
        for (int i = 0; i < _choices; i++)
        {
            // generates and Checks for duplicates
            int potential = -1;
            while (potential == -1 || Mike.MikeArray.Contains(choiceIndexes, potential))
            {
                potential = Mike.MikeRandom.RandomIntByWeights(weights);
            }

            // adds to array
            choiceIndexes[i] = potential;
        }

        ChoiceSelector.ChoiceData[] choices = new ChoiceSelector.ChoiceData[_choices];

        // converts to ChoiceData
        for (int i = 0; i < _choices; i++)
        {
            PowerUp powerUp = _powerUpLootTable[choiceIndexes[i]].item.GetComponent<PowerUp>();
            PowerUp oldPowerUp = PowerUpAdder.Instance.GetPowerUp(powerUp.powerUpName);

            choices[i] = new ChoiceSelector.ChoiceData
            {
                name = powerUp.powerUpName,
                description = oldPowerUp != null ? oldPowerUp.GetUpgradeDifference() + "\n" + powerUp.description : powerUp.description,
                iconSpr = powerUp.icon
            };
        }

        // locates ChoiceSelector and executes method
        GameObject.FindGameObjectWithTag("ChoiceSelector").GetComponent<ChoiceSelector>().DisplayChoice(choices, OnChoose);
    }

    void OnChoose(int index)
    {
        if(index >= 0)
        {
            PowerUpAdder.Instance.AddOrUpgradePowerUp(_powerUpLootTable[choiceIndexes[index]].item);
        }
    }
}
