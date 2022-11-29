using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggered) { return; }
        if (!collision.CompareTag("Player")) { return; }

        _triggered = true;
        GameManager.Insatnce.MoveToNextLevel();
    }
}
