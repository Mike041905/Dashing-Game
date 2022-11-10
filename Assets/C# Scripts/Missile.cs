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

    [SerializeField] float boostTime = 0.3f;
    [SerializeField] float boostDeceleration = 1.5f;

    private float currentSpeed = 0;
    private bool initialBoost = true;
    private float boostTimer = 0;


    private void Start()
    {
        currentSpeed = speed;
        trail.Stop();
    }

    void FixedUpdate()
    {
        if(currentSpeed < speed)
        {
            currentSpeed += 10f * Time.fixedDeltaTime + currentSpeed * 1.1f * Time.fixedDeltaTime;

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

        transform.position += currentSpeed * Time.fixedDeltaTime * transform.up;

        if(initialBoost && boostTimer < boostTime) { boostTimer += Time.fixedDeltaTime; currentSpeed -= currentSpeed * Time.deltaTime; return; }
        else if(initialBoost) { initialBoost = false; trail.Play(); currentSpeed = 0; }

        if(target != null) transform.rotation = Quaternion.RotateTowards(transform.rotation, Mike.MikeTransform.Rotation.LookTwards(transform.position, (Vector2)target.position * (1 + 0.1f * Random.Range(-maxWiggle, maxWiggle))), rotSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.transform.CompareTag("Player")) { return; }
        if (hit.transform.CompareTag("Projectile")) { return; }
        if (hit.transform.CompareTag("Coin")) { return; }

        Hit();
    }

    void Hit()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity).GetComponent<Explosion>().radius = damageRadius;

        trail.transform.parent = null;
        trail.Stop();

        Destroy(trail.gameObject, 2);
        Destroy(gameObject);
    }
}
