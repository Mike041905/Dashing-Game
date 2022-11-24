using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    struct Immunity
    {
        public GameObject damager;
        Health health;

        public Immunity(float immuneTime, GameObject damager, Health health)
        {
            this.health = health;
            this.damager = damager;
            GameManager.Insatnce.StartCoroutine(Delay(immuneTime));
        }
        IEnumerator Delay(float time)
        {
            yield return new WaitForSeconds(time);
            health.RemoveImmunity(this);
        }

        public static bool operator ==(Immunity lhs, Immunity rhs)
        {
            return lhs.damager == rhs.damager;
        }
        public static bool operator !=(Immunity lhs, Immunity rhs)
        {
            return lhs.damager != rhs.damager;
        }
    }

    [Header("References")]
    [SerializeField] private Slider healthSlider;

    [Header("Options")]
    [SerializeField] float health = 100f;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private int minCoinsOnDeath = 0;
    [SerializeField] private int maxCoinsOnDeath = 3;
    [SerializeField] float immuneTime = .2f;


    float _maxhealth;
    public float Maxhealth { get { if (_maxhealth == 0) { _maxhealth = CurrentHealth; } return _maxhealth; } }

    public bool Dead { get; private set; }
    public float CurrentHealth { get => health; set { SetHealth(value); } }

    public bool IsInvincible { get; set; } = false;


    List<Immunity> _immunities = new();

    public event UnityAction OnDeath;
    [SerializeField] UnityEvent OnDeathEvent;

    public event UnityAction OnRevive;
    [SerializeField] UnityEvent OnReviveEvent;

    public event UnityAction<float> OnHealthChanged;
    /// <summary>
    /// First Parameter: damage dealt | 
    /// Second Parameter: damager (Warning: may be null)
    /// </summary>
    public event UnityAction<float, GameObject> OnTakeDamage;

    //--------------------------


    private void Start()
    {
        Dead = false;
        if (CompareTag("Player")) _maxhealth = Upgrade.GetUpgrade("Health", UpgradeData.VariableType.Float);
        if (CompareTag("Player")) CurrentHealth = Upgrade.GetUpgrade("Health", UpgradeData.VariableType.Float);

        if(healthSlider != null) healthSlider.maxValue = Maxhealth;
        if (healthSlider != null) healthSlider.value = CurrentHealth;
    }


    //------------------------


    IEnumerator LerpHealthBarValue(float value)
    {
        while (healthSlider.value > value + .1f || healthSlider.value < value - .1f)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, value, Time.deltaTime * 4);

            yield return null;
        }

        healthSlider.value = value;
    }

    Coroutine _healthBarLearp;
    void UpdateHealthBarValue()
    {
        if (_healthBarLearp != null) { StopCoroutine(_healthBarLearp); }

        _healthBarLearp = StartCoroutine(LerpHealthBarValue(CurrentHealth));
    }

    void AddImmunity(float time, GameObject damager)
    {
        if(damager == null) { return; }

        _immunities.Add(new Immunity(time, damager, this));
    }

    void RemoveImmunity(Immunity immunity)
    {
        _immunities.Remove(immunity);
    }

    void HealthOnKill()
    {
        Health player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        if(player == null || player == this) { return; }

        player.CurrentHealth += Upgrade.GetUpgrade("Health On Kill", UpgradeData.VariableType.Float);
        if (player.CurrentHealth > player.Maxhealth) player.CurrentHealth = player.Maxhealth;
    }

    public void TakeDamage(float damage, GameObject damager = null)
    {
        if(CheckIfImmune(damager)) { return; }

        AddImmunity(immuneTime, damager);
        CurrentHealth -= damage;

        OnTakeDamage?.Invoke(damage, damager);
    }

    void SetHealth(float health, bool allowRevive = false)
    {
        if(IsInvincible && health < CurrentHealth) { return; }
        if(allowRevive && Dead) { Revive(health); return; }

        if (Dead) { return; }

        this.health = Mathf.Clamp(health, 0, Maxhealth);

        if (healthSlider != null) UpdateHealthBarValue();
        if (CurrentHealth <= 0) Die();

        OnHealthChanged?.Invoke(health);
    }

    public void Revive(float health)
    {
        if(!Dead) { return; }

        gameObject.SetActive(true);
        Dead = false;
        CurrentHealth = health;

        OnRevive?.Invoke();
        OnReviveEvent?.Invoke();
    }
    public void Revive() => Revive(Maxhealth);
    public void Revive(bool temporarlyInvincible, UnityAction<UnityAction> cancelActionAdd, UnityAction<UnityAction> cancelActionRemove)
    {
        IsInvincible = temporarlyInvincible;
        Revive();

        cancelActionAdd?.Invoke(Stop);
        void Stop()
        { 
            IsInvincible = false; cancelActionRemove?.Invoke(Stop); 
        }
    }

    private bool CheckIfImmune(GameObject damager)
    {
        if(damager == null) { return false; }

        foreach (Immunity immunity in _immunities)
        {
            if(immunity.damager == damager) { return true; }
        }

        return false;
    }

    public void Die()
    {
        if(IsInvincible) { return; }
        if (Dead) { return; } // a precaution (it likes to trigger multiple times when hit twice at the same time)
        Dead = true;

        OnDeath?.Invoke();
        OnDeathEvent?.Invoke();

        int coinAmmount = Random.Range(minCoinsOnDeath, maxCoinsOnDeath + 1);
        for (int i = 0; i < coinAmmount; i++) // idk if coinAmmount is a copy in a for loop or not and im to lazy to look it up
        {
            Instantiate(GameManager.Insatnce.CoinPrefab, transform.position + (Vector3) Mike.MikeRandom.RandomVector2(-.5f, .5f, -.5f, .5f), Quaternion.identity).GetComponent<Item>().coinsPerPickup = 1 + Mathf.RoundToInt(GameManager.Insatnce.Level * .1f);
        }

        HealthOnKill();
        if(deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (destroyOnDeath) Destroy(gameObject);
    }

    public void SetMaxHealth(float maxHealth, bool healToMax = true)
    {
        _maxhealth = maxHealth;
        if(healToMax) { CurrentHealth = maxHealth; }
    }
}
