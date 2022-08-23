using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] private Transform target;

    [Header("Options")]
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float baseSpeed = 1;
    [SerializeField] private float speedOverDistance = 1;

    private void Update()
    {
        if (smoothFollow) transform.position = Vector2.MoveTowards(transform.position, target.position, (baseSpeed + Vector2.Distance(transform.position, target.position) * speedOverDistance) * Time.unscaledDeltaTime);
        else transform.position = target.position;
    }
}
