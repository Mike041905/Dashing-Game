using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZigZagEffect : MonoBehaviour
{
    [SerializeField] private float minY = -1;
    [SerializeField] private float maxY = 1;
    [SerializeField] private float minSpeed = 3;
    [SerializeField] private float maxSpeed = 5;

    private float currentSpeed = 5;
    private float currentY = 5;
    bool up = true;

    private void Start()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        currentY = Random.Range(up == true ? 0 : minY, up == true ? maxY : 0);
    }

    private void FixedUpdate()
    {
        if (transform.localPosition != transform.up * currentY) transform.localPosition = Vector2.MoveTowards(transform.localPosition, transform.up * currentY, currentSpeed * Time.deltaTime);
        else
        {
            up = !up;
            currentSpeed = Random.Range(minSpeed, maxSpeed);
            currentY = Random.Range(up == true ? 0 : minY, up == true ? maxY : 0);
        }
    }
}
