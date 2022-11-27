using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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

    private float _currentSpeed = 0;
    private bool _initialBoost = true;
    private float _boostTimer = 0;

    private bool _hasTarget = true;

    Rigidbody2D _rb;
    Rigidbody2D Rb { get { if (_rb == null) { _rb = GetComponent<Rigidbody2D>(); } return _rb; } }

    private void Start()
    {
        _currentSpeed = speed;
        trail.Stop();
    }

    void FixedUpdate()
    {
        if(_currentSpeed < speed)
        {
            _currentSpeed += 10f * Time.fixedDeltaTime + _currentSpeed * 1.1f * Time.fixedDeltaTime;

            if(_currentSpeed > speed) { _currentSpeed = speed; }
        }

        if(target == null & _hasTarget) 
        { 
            GameObject go = MikeGameObject.GetClosestTargetWithTag(transform.position, "Enemy"); 
            if(go != null) 
            { 
                target = go.transform; 
            }
            else
            {
                _hasTarget = false;
            }
        }

        Rb.MovePosition(transform.position + _currentSpeed * Time.fixedDeltaTime * transform.up);

        if(_initialBoost && _boostTimer < boostTime) { _boostTimer += Time.fixedDeltaTime; _currentSpeed -= _currentSpeed * boostDeceleration * Time.deltaTime; return; }
        else if(_initialBoost) { _initialBoost = false; trail.Play(); _currentSpeed = 0; }

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
