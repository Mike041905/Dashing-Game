using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] private Transform _target;

    [Header("Options")]
    [SerializeField] private bool _smoothFollow = true;
    [SerializeField] private float _minSpeed = 1;
    [SerializeField] private float _speedDistanceExponent = 0;
    [SerializeField] private float _speedDistanceMultiplier = 1;


    private void Update()
    {
        if (_smoothFollow)
        {
            float dist = Vector2.Distance(transform.position, _target.position) * _speedDistanceMultiplier;
            float speed = _minSpeed + dist * Mathf.Pow(dist, _speedDistanceExponent) * Time.unscaledDeltaTime;

            transform.position = Vector2.MoveTowards(transform.position, _target.position, speed);
        }
        else
        {
            transform.position = _target.position;
        }
    }
}
