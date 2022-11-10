using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float damage = 100;
    [SerializeField] private float damageRadius = .5f;
    [SerializeField] private float speed = 5;
    [SerializeField] private float maxWiggle = 1;
    [SerializeField] private float rotSpeed = 180;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private ParticleSystem trail;
    [SerializeField] GameObject explosionPrefab;

    private float currentSpeed = 0;
    private bool initialBoost = true;
    private float boostTimer = 0;

    const float boostTime = 0.15f;

    private void Start()
    {
        currentSpeed = speed;
        trail.Stop();
    }

    void FixedUpdate()
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

        if(initialBoost && boostTimer < boostTime) { boostTimer += Time.deltaTime; currentSpeed -= currentSpeed * Time.deltaTime; return; }
        else if(initialBoost) { initialBoost = false; trail.Play(); currentSpeed = 0; }

        if(target != null) transform.rotation = Quaternion.RotateTowards(transform.rotation, Mike.MikeTransform.Rotation.LookTwards(transform.position, target.position), rotSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.transform.CompareTag("Player")) { return; }
        if (hit.transform.CompareTag("Projectile")) { return; }
        if (hit.transform.CompareTag("Coin")) { return; }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity).GetComponent<Explosion>().radius = damageRadius;
        Hit();
    }

    void Hit()
    {
        if (hitEffect != null) { Instantiate(hitEffect, transform.position, Quaternion.identity); }

        trail.transform.parent = null;
        trail.Stop();

        Destroy(trail.gameObject, 2);
        Destroy(gameObject);
    }
}
