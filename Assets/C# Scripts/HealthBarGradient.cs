using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarGradient : MonoBehaviour
{
    [SerializeField] Health _health;
    [SerializeField] Gradient _gradient;
    [SerializeField] Graphic[] _graphics = new Graphic[0];
    [SerializeField] SpriteRenderer[] _spriteRenderers = new SpriteRenderer[0];

    float _playerHealthPercentage;

    void Start()
    {
        _health.OnHealthChanged += UpdateGraphicColor;
    }

    void UpdateGraphicColor(float health)
    {
        if(_health.Maxhealth == 0 || _health.Maxhealth == float.PositiveInfinity) { return; }

        _playerHealthPercentage = health / _health.Maxhealth;

        for (int i = 0; i < math.max(_graphics.Length, _spriteRenderers.Length); i++)
        {
            if(i < _graphics.Length) _graphics[i].color = _gradient.Evaluate(_playerHealthPercentage);
            if(i < _spriteRenderers.Length) _spriteRenderers[i].color = _gradient.Evaluate(_playerHealthPercentage);
        }
    }
}
