using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarGradient : MonoBehaviour
{
    [SerializeField] Gradient _gradient;
    [SerializeField] Image _healthBarImage;

    float _playerHealthPercentage;

    void Start()
    {
        Player.Instance.PlayerHealth.OnHealthChanged += UpdateHealthBarColor;
    }

    void UpdateHealthBarColor(float health)
    {
        if(Player.Instance.PlayerHealth.Maxhealth == 0 || Player.Instance.PlayerHealth.Maxhealth == float.PositiveInfinity) { return; }

        _playerHealthPercentage = health / Player.Instance.PlayerHealth.Maxhealth;
        _healthBarImage.color = _gradient.Evaluate(_playerHealthPercentage);
    }
}
