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


    public float maxhealth;

    public bool Dead { get; private set; }
    public float CurrentHealth { get => health; set { SetHealth(value); } }


    List<Immunity> immunities = new();

    public event UnityAction OnDeath;
    public event UnityAction OnRevive;

    public event UnityAction<float> OnHealthChanged;
    /// <summary>
    /// First Parameter: damage dealt | 
    /// Second Parameter: damager (Warning: may be null)
    /// </summary>
    public event UnityAction<float, GameObject> OnTakeDamage;

    //--------------------------


    private void Start()
    {
        if (CompareTag("Player")) CurrentHealth = PlayerPrefs.GetFloat("Health");
        maxhealth = CurrentHealth;
        if(healthSlider != null) healthSlider.maxValue = maxhealth;
        if (healthSlider != null) healthSlider.value = CurrentHealth;
    }


    //------------------------


    void AddImmunity(float time, GameObject damager)
    {
        if(damager == null) { return; }

        immunities.Add(new Immunity(time, damager, this));
    }

    void RemoveImmunity(Immunity immunity)
    {
        immunities.Remove(immunity);
    }

    void HealthOnKill()
    {
        Health player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        if(player == null || player == this) { return; }

        player.CurrentHealth += PlayerPrefs.GetFloat("Health On Kill");
        if (player.CurrentHealth > player.maxhealth) player.CurrentHealth = player.maxhealth;
        if (player.healthSlider != null) player.healthSlider.value = player.CurrentHealth;
    }

    public void TakeDamage(float damage, GameObject damager = null)
    {
        if(CheckIfImmune(damager)) { return; }

        AddImmunity(immuneTime, damager);
        CurrentHealth -= damage;
        OnTakeDamage?.Invoke(damage, damager);

        if (healthSlider != null) healthSlider.value = CurrentHealth;
        if (CurrentHealth <= 0) Die();
    }

    void SetHealth(float health, bool allowRevive = false)
    {
        if(allowRevive && Dead) { Revive(health); return; }

        if (Dead) { return; }

        this.health = Mathf.Clamp(health, 0, maxhealth);
        OnHealthChanged?.Invoke(health);
    }

    public void Revive(float? health = null)
    {
        if(!Dead) { return; }

        if(health == null) { health = maxhealth; }

        gameObject.SetActive(true);
        Dead = false;
        CurrentHealth = health.Value;
        OnRevive?.Invoke();
    }

    private bool CheckIfImmune(GameObject damager)
    {
        if(damager == null) { return false; }

        foreach (Immunity immunity in immunities)
        {
            if(immunity.damager == damager) { return true; }
        }

        return false;
    }

    public void Die()
    {
        if (Dead) { return; } // a precaution (it likes to trigger multiple times when hit twice at the same time)
        Dead = true;

        OnDeath?.Invoke();

        int coinAmmount = Random.Range(minCoinsOnDeath, maxCoinsOnDeath);
        for (int i = 0; i < coinAmmount; i++) // idk if coinAmmount is a copy in a for loop or not and im to lazy to look it up
        {
            Instantiate(GameManager.Insatnce.coin, transform.position + (Vector3) Mike.MikeRandom.RandomVector2(-.5f, .5f, -.5f, .5f), Quaternion.identity).GetComponent<Item>().coinsPerPickup = 1 + Mathf.RoundToInt(GameManager.Insatnce.Level * .1f);
        }

        HealthOnKill();
        if(deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (destroyOnDeath) Destroy(gameObject);
    }
}
