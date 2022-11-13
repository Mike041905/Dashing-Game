using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] private GameObject _hitEffect;
    [HideInInspector] public GameObject shooter;

    [HideInInspector] public float speed = 0;
    [HideInInspector] public float damage = 0;


    //---------------------------------------


    private void Update()
    {
        transform.position += speed * Time.deltaTime * transform.up;
    }


    //-----------------------------------------


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform.gameObject == shooter) { return; }

        DealDamageIfAble(collider.transform.gameObject);
    }

    void DealDamageIfAble(GameObject other)
    {
        if (other.CompareTag("Projectile")) { return; }
        if (other.CompareTag("Coin")) { return; }

        if(other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(damage, gameObject);
        }

        if (_hitEffect != null) Instantiate(_hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
