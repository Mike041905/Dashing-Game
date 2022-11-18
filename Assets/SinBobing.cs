using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinBobing : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float magintude;

    float timer = 0;

    private void Update()
    {
        timer += Time.deltaTime * speed;
        timer = timer > 360 ? timer -= 360 : timer;

        transform.localPosition = new Vector3 (transform.localPosition.x, Mathf.Sin(timer) * magintude, transform.localPosition.z);
    }
}
