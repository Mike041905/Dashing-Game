using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedExecute : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private UnityEvent actions;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);

        actions?.Invoke();
    }
}
