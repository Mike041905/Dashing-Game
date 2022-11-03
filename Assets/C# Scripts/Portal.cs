using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == null) { return; }
        if (!collision.CompareTag("Player")) { return; }

        GameManager.Insatnce.MoveToNextLevel();
    }
}
