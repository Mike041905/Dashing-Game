using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private GameObject sendTo;
    [SerializeField] private string method;

    private void OnTriggerExit2D(Collider2D collision)
    {
        sendTo.SendMessage(method, collision, SendMessageOptions.DontRequireReceiver);
    }
}
