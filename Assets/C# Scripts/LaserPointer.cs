using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    [SerializeField] private Transform _origin;
    [SerializeField] private LineRenderer _lineRenderer;

    private void Update()
    {
        _lineRenderer.SetPosition(0, (Vector2)_origin.position);

        RaycastHit2D ray = Physics2D.Raycast(_origin.position, _origin.up);

        if(ray) _lineRenderer.SetPosition(1, ray.point);
    }
}
