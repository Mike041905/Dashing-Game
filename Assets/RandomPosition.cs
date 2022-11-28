using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    [SerializeField] private float _max = 2;
    [SerializeField] private bool _unparent = true;

    void Start()
    {
        transform.localPosition = Random.insideUnitCircle * Random.Range(0, _max);
        if(_unparent) { transform.parent = null; }
    }
}
