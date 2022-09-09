using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float damage = 100;
    [SerializeField] private float speed = 5;
    [SerializeField] private float rotSpeed = 180;
    [SerializeField] private GameObject hitEffect;

    private float currentSpeed = 0;

    void Update()
    {
        if(currentSpeed < speed)
        {
            currentSpeed += 10f * Time.deltaTime + currentSpeed * 1.1f * Time.deltaTime;

            if(currentSpeed > speed) { currentSpeed = speed; }
        }

        if(target == null) 
        { 
            GameObject go = MikeGameObject.GetClosestTargetWithTag(transform.position, "Enemy"); 
            if (go == null) 
            {
                Hit(); 
                return; 
            } 
            else 
            { 
                target = go.transform; 
            } 
        }

        if(target == null) 
        {
            if (hitEffect != null) { Instantiate(hitEffect, transform.position, Quaternion.identity); }
            Destroy(gameObject);
        }

        if(target != null) transform.position += currentSpeed * Time.deltaTime * transform.up;
        if(target != null) transform.rotation = Quaternion.RotateTowards(transform.rotation, Mike.MikeTransform.Rotation.LookTwards(transform.position, target.position), rotSpeed * Time.deltaTime);
    }

    public void OnMikeSphereTriggerEnter(RaycastHit2D hit)
    {
        if (hit.transform.CompareTag("Player")) { return; }
        if (hit.transform.CompareTag("Projectile")) { return; }
        if (hit.transform.CompareTag("Coin")) { return; }

        if(hit.transform.GetComponent<Health>() != null) { hit.transform.GetComponent<Health>().TakeDamage(damage * PlayerPrefs.GetFloat("Damage")); }

        Hit();
    }

    void Hit()
    {
        if (hitEffect != null) { Instantiate(hitEffect, transform.position, Quaternion.identity); }

        Destroy(gameObject);
    }
}