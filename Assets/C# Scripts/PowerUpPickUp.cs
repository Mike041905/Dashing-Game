using UnityEngine;

public class PowerUpPickUp : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject _collectionEffect;
    public float HomingRange = 0;
    public float HomingSpeed = 5;
    [SerializeField] private float _activationDelay = 0;
    [SerializeField] private float _pickUpRarity = 1;

    [Header("PowerUp")]
    [SerializeField] private PowerUpLootTable _lootTable;
    [SerializeField] private int _choices = 3;

    float _timer = 0;

    Transform PlayerTransfrom { get => Player.Instance.transform; }
    PowerUp[] PowerUpLootTable { get => _lootTable.LootTable; }


    private void Update()
    {
        if (Player.Instance.PlayerHealth.Dead) { return; }

        _timer += _activationDelay > _timer ? Time.deltaTime : 0;

        if (_activationDelay > _timer) { return; }

        if (Vector2.Distance(transform.position, PlayerTransfrom.position) <= HomingRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, PlayerTransfrom.position, HomingSpeed * (1 - (Vector2.Distance(transform.position, PlayerTransfrom.position) / HomingRange)) * Time.deltaTime);
            transform.position = new(transform.position.x, transform.position.y, -1);
        }

        if (Vector2.Distance(transform.position, PlayerTransfrom.position) <= 0.1f) { CollectItem(PlayerTransfrom.GetComponent<Collider2D>()); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Player.Instance.PlayerHealth.Dead) { return; }

        CollectItem(collision);
    }

    void CollectItem(Collider2D playerCollider)
    {
        if (_activationDelay > _timer) { return; }
        if (!playerCollider.CompareTag("Player")) { return; }

        ChoosePowerUp();

        if (_collectionEffect != null) Instantiate(_collectionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // required for further use
    int[] _choiceIndexes;
    void ChoosePowerUp()
    {
        // percaucion
        if (_choices > PowerUpLootTable.Length) { Debug.LogError("Loot Table too small"); return; }

        // Get Loot Weights
        float[] weights = new float[PowerUpLootTable.Length];
        for (int i = 0; i < PowerUpLootTable.Length; i++)
        {
            // List only if hasn't reached max level
            if (PowerUpAdder.Instance.TryGetPowerUp(PowerUpLootTable[i].powerUpName, out PowerUp p))
            {
                weights[i] = p.HasReachedMaxLevel ? 0 : PowerUpLootTable[i].CalculateRelativeWeight(_pickUpRarity);
            }
            else
            {
                weights[i] = PowerUpLootTable[i].CalculateRelativeWeight(_pickUpRarity);
            }
        }

        _choiceIndexes = new int[_choices];
        for (int i = 0; i < _choiceIndexes.Length; i++)
        {
            _choiceIndexes[i] = -1;
        }

        // Generates Choices
        for (int i = 0; i < _choices; i++)
        {
            // generates and Checks for duplicates
            int potential = -1;
            while (potential == -1 || Mike.MikeArray.Contains(_choiceIndexes, potential))
            {
                potential = Mike.MikeRandom.RandomIntByWeights(weights);
            }

            // adds to array
            _choiceIndexes[i] = potential;
        }

        ChoiceSelector.ChoiceData[] choices = new ChoiceSelector.ChoiceData[_choices];

        // converts to ChoiceData
        for (int i = 0; i < _choices; i++)
        {
            PowerUp powerUp = PowerUpLootTable[_choiceIndexes[i]];
            PowerUp oldPowerUp = PowerUpAdder.Instance.GetPowerUp(powerUp.powerUpName);

            choices[i] = new ChoiceSelector.ChoiceData
            {
                name = powerUp.powerUpName,
                description = oldPowerUp != null ? oldPowerUp.GetUpgradeDifference() : powerUp.description,
                iconSpr = powerUp.icon
            };
        }

        // locates ChoiceSelector and executes method
        GameObject.FindGameObjectWithTag("ChoiceSelector").GetComponent<ChoiceSelector>().DisplayChoice(choices, OnChoose);
    }

    void OnChoose(int index)
    {
        if (index >= 0)
        {
            PowerUpAdder.Instance.AddOrUpgradePowerUp(PowerUpLootTable[_choiceIndexes[index]].gameObject);
            Debug.Log(PowerUpLootTable[_choiceIndexes[index]].powerUpName);
        }
    }
}
