using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedExecute : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private UnityEvent actions;

    [SerializeField] private bool executeOnStart = true;

    void Start()
    {
        if (executeOnStart) { Execute(); }
    }

    public void Execute()
    {
        StartCoroutine(ExecuteCo());
    }

    IEnumerator ExecuteCo()
    {
        yield return new WaitForSeconds(delay);

        actions?.Invoke();
    }
}
