using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;

    [Header("Options")]
    public float health = 100f;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private int minCoinsOnDeath = 0;
    [SerializeField] private int maxCoinsOnDeath = 3;
    [SerializeField] float immuneTime = .2f;


    float maxhealth;
    bool dead = false;

    bool immune = false;

    public UnityEvent OnDeath;

    //--------------------------


    private void Start()
    {
        if (CompareTag("Player")) health = PlayerPrefs.GetFloat("Health");
        maxhealth = health;
        if(healthSlider != null) healthSlider.maxValue = maxhealth;
        if (healthSlider != null) healthSlider.value = health;
    }


    //------------------------


    IEnumerator Immunity(float time)
    {
        immune = true;
        yield return new WaitForSeconds(time);
        immune = false;
    }

    void HealthOnKill()
    {
        Health player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();

        player.health += PlayerPrefs.GetFloat("Health On Kill");
        if (player.health > player.maxhealth) player.health = player.maxhealth;
        if (player.healthSlider != null) player.healthSlider.value = player.health;
    }

    public void TakeDamage(float damage)
    {
        if(immune) { return; }

        health -= damage;
        StartCoroutine(Immunity(immuneTime));

        if (healthSlider != null) healthSlider.value = health;
        if (health <= 0) Die();
    }

    public void Die()
    {
        if (dead) { return; } // a precaution (it likes to trigger multiple times when hit twice at the same time)
        dead = true;

        for (int i = 0; i < Random.Range(minCoinsOnDeath, maxCoinsOnDeath); i++)
        {
            Instantiate(GameManager.Insatnce.coin, transform.position + (Vector3) Mike.MikeRandom.RandomVector2(-.5f, .5f, -.5f, .5f), Quaternion.identity).GetComponent<Item>().coinsPerPickup = 1 + Mathf.RoundToInt(GameManager.Insatnce.Level * .1f);
        }

        HealthOnKill();
        OnDeath?.Invoke();
        if(deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (destroyOnDeath) Destroy(gameObject);
    }
}
