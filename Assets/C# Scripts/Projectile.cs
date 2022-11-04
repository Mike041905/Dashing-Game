using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] private GameObject HitEffect;
    [HideInInspector] public GameObject shooter;

    [HideInInspector] public float speed = 0;
    [HideInInspector] public float damage = 0;


    //---------------------------------------


    private void Update()
    {
        transform.position += speed * Time.deltaTime * transform.up;
    }


    //-----------------------------------------


    public void OnMikeSphereTriggerEnter(RaycastHit2D hit2D)
    {
        if(hit2D.transform.gameObject == shooter) { return; }

        DealDamageIfAble(hit2D.transform.gameObject);
    }

    void DealDamageIfAble(GameObject other)
    {
        if (other.CompareTag("Projectile")) { return; }
        if (other.CompareTag("Coin")) { return; }

        if(other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(damage, gameObject);
        }

        if (HitEffect != null) Instantiate(HitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
