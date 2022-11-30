using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] bool _useFixedUpdate = true;
    [SerializeField] bool _canColideWithOtherProjectiles = false;
    [SerializeField] bool _destroyOnHit = true;

    GameObject _shooter;
    bool _initialized = false;

    float _speed = 0;
    float _damage = 0;

    string[] _tagDamageBlacklist = new string[0];
    string[] _tagHitBlacklist = new string[0];

    Rigidbody2D _rb;
    Rigidbody2D Rb { get { if (_rb == null) { _rb = GetComponent<Rigidbody2D>(); } return _rb; } }


    //---------------------------------------


    private void FixedUpdate()
    {
        if (!_initialized) { return; }
        if (!_useFixedUpdate) { return; }

        Rb.MovePosition(Rb.position + (_speed * Time.fixedDeltaTime * (Vector2)transform.up));
    }


    //-----------------------------------------


    public Projectile Initialize(GameObject shooter, float speed, float damage, string[] tagHitBlacklist, string[] tagDamageBlacklist)
    {
        _shooter = shooter;
        _speed = speed;
        _damage = damage;
        _tagHitBlacklist = tagHitBlacklist;
        _tagDamageBlacklist = tagDamageBlacklist;
        _initialized = true;

        return this;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!_initialized) { return; }
        if(collider.transform.gameObject == _shooter) { return; }

        OnHit(collider.transform.gameObject);
    }

    protected virtual void OnHit(GameObject other)
    {
        if (_canColideWithOtherProjectiles && other.TryGetComponent(out Projectile _)) { return; }
        if (_tagHitBlacklist.Contains(other.tag)) { return; }

        if(!_tagDamageBlacklist.Contains(other.tag) && other.TryGetComponent(out Health health))
        {
            health.TakeDamage(_damage, gameObject);
        }

        Die();
    }

    public void Hit() { Die(); }

    protected virtual void Die()
    {
        if (_hitEffect != null) Instantiate(_hitEffect, transform.position, Quaternion.identity);
        if (_destroyOnHit) { Destroy(gameObject); }
        else { gameObject.SetActive(false); }
    }
}
