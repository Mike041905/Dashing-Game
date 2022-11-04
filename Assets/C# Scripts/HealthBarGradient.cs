using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarGradient : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] Health playerHealth;
    [SerializeField] float playerHealthPercentage;
    [SerializeField] Image healthBarImage;

    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerHealth.OnTakeDamage += UpdateHealthBarColor;
    }

    void UpdateHealthBarColor(float damage, GameObject damager)
    {
        if(playerHealth.maxhealth == 0 || playerHealth.maxhealth == float.PositiveInfinity) { return; }

        playerHealthPercentage = playerHealth.CurrentHealth / playerHealth.maxhealth;
        healthBarImage.color = gradient.Evaluate(playerHealthPercentage);
    }
}
