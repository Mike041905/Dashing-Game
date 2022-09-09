using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikeDetach : MonoBehaviour
{
    [SerializeField] float _delay = 0;
    float _timer = 0;

    private void Awake()
    {
        if(_delay == 0)
        {
            transform.parent = null;
        }
    }

    private void Update()
    {
        if(_timer <= 0)
        {
            transform.parent = null;
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }
}
