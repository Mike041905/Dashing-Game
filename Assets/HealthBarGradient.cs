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
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthPercentage = playerHealth.health / playerHealth.maxhealth;
        healthBarImage.color = gradient.Evaluate(playerHealthPercentage);
    }
}
