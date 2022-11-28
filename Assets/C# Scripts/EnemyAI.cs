using Mike;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Events;

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
                        OnStartExecuteIn?.Invoke();
                        await OnExecuteIn();
                        OnFinishExecuteIn?.Invoke();

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

        public UnityAction OnStartExecuteIn;
        public UnityAction OnFinishExecuteIn;
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
                new string[] { "Coin", "PowerUp", "EnemyShield" },
                new string[] { _firePoint.root.tag }
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
    [SerializeField] private bool _canMoveWhileShooting = true;
    [SerializeField] private bool _canRotateWhileShooting = true;
    [SerializeField] private float _rotationSpeed = 180;
    [SerializeField] float _wakeUpTimeMin = .5f;
    [SerializeField] float _wakeUpTimeMax = 2;


    //---------------------------------------------


    protected Transform Target { get => Player.Instance.transform; }
    public Room Room;

    Health _enemyHealth;
    public Health EnemyHealth { get { if (_enemyHealth == null) { _enemyHealth = GetComponent<Health>(); } return _enemyHealth; } }
    [field: SerializeField] public Health[] AdditionalHealthScripts { get; private set; }

    Rigidbody2D _rb;
    public Rigidbody2D Rb { get { if (_rb == null) { _rb = GetComponent<Rigidbody2D>(); } return _rb; } }


    bool _initialized = false;
    bool _isShooting = false;

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
        SetMaxHealthBasedOnDifficulty(difficulty);
        _initialized = true;


        void SetMaxHealthBasedOnDifficulty(float difficulty)
        {
            EnemyHealth.SetMaxHealth(EnemyHealth.Maxhealth * difficulty);
            foreach (Health health in AdditionalHealthScripts)
            {
                health.SetMaxHealth(health.Maxhealth * difficulty);
            }
        }
    }

    protected virtual void FaceTarget()
    {
        if(!_canRotateWhileShooting && _isShooting) { return; }

        Rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, MikeTransform.Rotation.LookTwards(transform.position, Target.position), _rotationSpeed * Time.fixedDeltaTime));
    }

    private async void ShootIfAble()
    {
        _pattern.OnStartExecuteIn += () => _isShooting = true;
        _pattern.OnFinishExecuteIn += () => _isShooting = false;

        while (true)
        {
            _isShooting = false;

            if (this == null) return;
            if(!enabled) return;

            if (!Player.Instance.PlayerHealth.Dead && shootingDistance >= Vector2.Distance(transform.position, Target.position))
            {
                await Task.Delay((int)UnityEngine.Random.Range(_wakeUpTimeMin * 1000, _wakeUpTimeMax * 1000));

                if (this == null) { return; }
                if (Player.Instance.gameObject == null) { return; }
                while 
                    (
                        !Player.Instance.PlayerHealth.Dead &&
                        this != null &&
                        enabled &&
                        shootingDistance >= Vector2.Distance(transform.position, Target.position)
                    )
                {
                    await _pattern.Execute();
                }
            }

            await Task.Yield();
        }
    }

    protected virtual void Move()
    {
        if(!_canMoveWhileShooting && _isShooting) { return; }

        if (Vector2.Distance(transform.position, Target.position) > stopRange)
        {
            Rb.MovePosition(Rb.position + (movementSpeed * Time.fixedDeltaTime * (Vector2)transform.up));
        }
        else if (Vector2.Distance(transform.position, Target.position) < backupRange)
        {
            Rb.MovePosition(Rb.position - (movementSpeed * Time.fixedDeltaTime * (Vector2)transform.up));
        }

        if(Vector2.Distance(transform.position, Room.transform.position) > 27) { EnemyHealth.Die(); }
    }

    private void OnDestroy()
    {
        if(!_initialized) return;

        Room.EndFightIfEnemiesDead();
    }
}