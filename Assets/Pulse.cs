using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    [SerializeField] float magnitude = 1;
    [SerializeField] float frequency = 1;

    [SerializeField] SpriteRenderer spriteRenderer;

    Color startColor;

    private void Start()
    {
        startColor = spriteRenderer.material.GetColor("_EmissionColor");
    }

    void Update()
    {
        float mul = Mathf.Sin(Time.time * 360 * frequency * Mathf.Deg2Rad); // -1, 1

        spriteRenderer.material.SetColor("_EmissionColor", startColor + startColor * mul * magnitude);
    }
}
