using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeByHealth : MonoBehaviour
{
    [SerializeField] Health _target;
    [SerializeField] Vector2 _sizeMultiplier = Vector2.one;
    [SerializeField] Vector2 _sizeOffset = Vector2.one;

    private void Start()
    {
        _target.OnHealthChanged += SetSize;
    }

    void SetSize(float health)
    {
        float perc = health / _target.Maxhealth;

        transform.localScale = _sizeMultiplier * perc + _sizeOffset;
    }
}
