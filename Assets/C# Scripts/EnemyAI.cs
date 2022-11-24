using Mike;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Bindings;

public class EnemyAI : MonoBehaviour
{
    // This could be reused
    // TODO: Make this into a seperate script in the Mike library!
    public class PatternKey
    {
        [SerializeField] float _inTime;
        float _inTimer = 0;

        [SerializeField] float _outTime;
        float _outTimer = 0;

        public virtual void Reset()
        {
            _outTimer = 0;
            _inTimer = 0;
        }

        public async Task Execute()
        {
            bool executedIn = false;

            while (true)
            {
                while (_inTimer >= _inTime)
                {
                    if (!executedIn)
                    {
                        await OnExecuteIn();
                        executedIn = true;
                    }

                    if (_outTimer >= _outTime)
                    {
                        OnExecuteFinish();
                        Reset();
                        return;
                    }

                    _outTimer += Time.deltaTime;

                    await Task.Yield();
                }

                _inTimer += Time.deltaTime;

                await Task.Yield();
            }
        }

        public virtual void OnExecuteFinish()
        {

        }
        
        public virtual async Task OnExecuteIn()
        {

        }
    }

    [System.Serializable]
    public struct ProjectileData
    {
        [SerializeField] Projectile _projectilePrefab;
        [SerializeField] Transform _firePoint;
        [SerializeField] ParticleSystem _muzzleFlash;

        [SerializeField] float _projectileDamage;
        [SerializeField] float _projectileSpeed;
        [SerializeField] float _inaccuracy;

        public void Shoot()
        {
            if(_firePoint == null) { return; }
            if (_muzzleFlash != null) { _muzzleFlash.Play(); }

            Instantiate(_projectilePrefab, _firePoint.position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(-_inaccuracy, _inaccuracy) + _firePoint.rotation.eulerAngles.z)).Initialize
            (
                _firePoint.root.gameObject,
                _projectileSpeed,
                _projectileDamage * GameManager.Insatnce.Difficulty,
                new string[2] { "Coin", "PowerUp" },
                new string[1] { _firePoint.root.tag }
            );
        }
    }

    [System.Serializable]
    public class FireKeyFrame : PatternKey
    {
        [SerializeField] ProjectileData _projectile;

        public override Task OnExecuteIn()
        {
            _projectile.Shoot();

            return base.OnExecuteIn();
        }
    }

    [System.Serializable]
    public class RepeatPattern : PatternKey
    {
        [SerializeField] FireKeyFrame[] _firePattern = new FireKeyFrame[0];
        [SerializeField] int _repeat;

        public override async Task OnExecuteIn()
        {
            for (int i = 0; i < _repeat + 1; i++)
            {
                foreach (FireKeyFrame key in _firePattern)
                {
                    await key.Execute();
                }
            }
        }
    }

    [System.Serializable]
    public class FirePattern : PatternKey
    {
        [SerializeField] RepeatPattern[] _repeat;

        public override async Task OnExecuteIn()
        {
            foreach (RepeatPattern pattern in _repeat)
            {
                await pattern.Execute();
            }
        }
    }


    [Header("FirePattern Settings")]
    [SerializeField] FirePattern _pattern;

    [Header("AI")]
    [Tooltip("The minimum range from the target")]
    [SerializeField] protected float movementSpeed = 3;
    [SerializeField] private float playerAvoidanceSpeed = 3;
    [SerializeField] private float stopRange = 10;
    [SerializeField] private float backupRange = 10;
    [SerializeField] private float shootingDistance = 10;
    [SerializeField] float _wakeUpTimeMin = .5f;
    [SerializeField] float _wakeUpTimeMax = 2;


    //---------------------------------------------


    protected Transform Target { get => Player.Instance.transform; }
    public Room Room;

    Health _enemyHealth;
    public Health EnemyHealth { get { if (_enemyHealth == null) { _enemyHealth = GetComponent<Health>(); } return _enemyHealth; } }

    Rigidbody2D _rb;
    public Rigidbody2D Rb { get { if (_rb == null) { _rb = GetComponent<Rigidbody2D>(); } return _rb; } }


    bool _initialized = false;

    //---------------------------------------------

    private void Start()
    {
        if (!_initialized) return;

        ShootIfAble();
    }

    private void FixedUpdate()
    {
        if (!_initialized) { return; }

        if (Player.Instance.PlayerHealth.Dead) { return; }
        if (Target == null) { return; }

        Move();
        FaceTarget();
    }


    //----------------------------------------------

    public virtual void Initialize(Room room, float difficulty)
    {
        Room = room;
        EnemyHealth.SetMaxHealth(EnemyHealth.Maxhealth * difficulty);

        _initialized = true;
    }

    protected virtual void FaceTarget()
    {
        transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, Target.position);
    }

    private async void ShootIfAble()
    {
        while (true)
        {
            if(this == null) return;
            if(!enabled) return;

            if (!Player.Instance.PlayerHealth.Dead && shootingDistance >= Vector2.Distance(transform.position, Target.position))
            {
                await Task.Delay((int)UnityEngine.Random.Range(_wakeUpTimeMin * 1000, _wakeUpTimeMax * 1000));
                
                while (!Player.Instance.PlayerHealth.Dead &&
                    this != null &&
                    enabled &&
                    shootingDistance >= Vector2.Distance(transform.position, Target.position))
                {
                    await _pattern.Execute();
                }
            }

            await Task.Yield();
        }
    }

    protected virtual void Move()
    {
        if (Vector2.Distance(transform.position, Target.position) > stopRange)
        {
            Rb.MovePosition(Rb.position + (movementSpeed * Time.fixedDeltaTime * (Vector2)transform.up));
        }
        else if (Vector2.Distance(transform.position, Target.position) < backupRange)
        {
            Rb.MovePosition(Rb.position - (movementSpeed * Time.fixedDeltaTime * (Vector2)transform.up));
        }
    }

    private void OnDestroy()
    {
        if(!_initialized) return;

        Room.EndFightIfEnemiesDead();
    }
}