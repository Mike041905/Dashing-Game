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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.transform.CompareTag("Player")) { return; }
        if (collision.GetComponent<Dash>().currentDash != null) { return; }

        float distancePerc = (1 - Vector2.Distance(transform.position, collision.ClosestPoint(transform.position)) / size);
        EZCameraShake.CameraShaker.Instance.ShakeOnce(Mathf.Pow(distancePerc, 2) * 0.5f, 10, 0.1f, 0.1f);
        collision.transform.position = Vector2.MoveTowards(collision.transform.position, transform.position, distancePerc * force * Time.deltaTime);
    }
}
