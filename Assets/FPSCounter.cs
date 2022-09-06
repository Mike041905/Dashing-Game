using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    float timer = 0f;
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            text.text = $"FPS: {1 / Time.deltaTime} ({Time.deltaTime * 1000}ms)";
            timer = .2f;
        }
    }
}
