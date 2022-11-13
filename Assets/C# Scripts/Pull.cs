using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : MonoBehaviour
{
    float size;
    [SerializeField] float force;
    private void Start()
    {
        size = GetComponent<CircleCollider2D>().radius;
    }

    private void OnTriggerStay2D(Collider2D c)
    {
        if(!c.CompareTag("Player")) { return; }
        if(Player.Instance.PlayerDash.CurrentDash != null) { return; }

        float distancePerc = (1 - Vector2.Distance(transform.position, c.ClosestPoint(transform.position)) / size);
        EZCameraShake.CameraShaker.Instance.ShakeOnce(Mathf.Pow(distancePerc, 2) * 0.5f, 10, 0.1f, 0.1f);
        c.transform.position = Vector2.MoveTowards(c.transform.position, transform.position, distancePerc * force * Time.deltaTime);
    }
}
