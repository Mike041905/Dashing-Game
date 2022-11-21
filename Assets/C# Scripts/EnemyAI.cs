using Mike;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // TODO: Put all of this data into the enemy manager or a seperate singleton for optmiztion;

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

        public bool Execute(float deltaTime)
        {
            if (_inTimer >= _inTime)
            {
                if (_outTimer >= _outTime)
                {
                    OnExecuteFinish(deltaTime);

                    return true;
                }

                if (OnExecuteIn(deltaTime))
                {
                    _outTimer += deltaTime;
                }
            }

            _inTimer += deltaTime;
            return false;
        }

        public virtual void OnExecuteFinish(float deltaTime)
        {

        }
        
        public virtual bool OnExecuteIn(float deltaTime)
        {
            return false;
        }
    }

    [System.Serializable]
    public struct ProjectileData
    {
        public Projectile ProjectilePrefab;
        public Transform FirePoint;

        public float ProjectileDamage;
        public float ProjectileSpeed;
        public float DelayBetweenShots;
        public int ProjectilesPerShot;
        public float Inaccuracy;

        public void Shoot()
        {

        }
    }

    [System.Serializable]
    public class FireKeyFrame : PatternKey
    {
        [SerializeField] ProjectileData _projectile;

        public override void OnExecuteFinish(float deltaTime)
        {
            _projectile.Shoot();
        }
    }

    [System.Serializable]
    public class RepeatPattern : PatternKey
    {
        [SerializeField] FireKeyFrame[] _firePattern = new FireKeyFrame[0];
        [SerializeField] int _repeat;

        int _index = 0;

        int _repeatIndex = 0;

        public override bool OnExecuteIn(float deltaTime)
        {
            if(_repeatIndex >= _repeat) { return true; }

            foreach (FireKeyFrame pattern in _firePattern)
            {
                if (_index >= _firePattern.Length) { return true; }

                if (pattern.Execute(deltaTime)) { _index++; }
            }

            return false;
        }
    }

    [SerializeField]
    public class FirePattern : PatternKey
    {
        RepeatPattern[] _repeat;
        int _index = 0;

        public override bool OnExecuteIn(float deltaTime)
        {
            if(_index >= _repeat.Length) { return true; }

            if (_repeat[_index].Execute(deltaTime)) { _index++; }

            return false;
        }

        public override void OnExecuteFinish(float deltaTime)
        {
            _index = 0;
        }
    }


    [Header("Essential")]
    [SerializeField] private ParticleSystem _muzzleFlash;

    [Header("FirePattern Settings")]
    [SerializeField] FirePattern _pattern;

    [Header("AI")]
    [Tooltip("The minimum range from the target")]
    [SerializeField] protected float movementSpeed = 3;
    [SerializeField] private float playerAvoidanceSpeed = 3;
    [SerializeField] private float stopRange = 10;
    [SerializeField] private float backupRange = 10;
    [SerializeField] private float shootingDistance = 10;
    public float difficultyMultiplier = 1;


    //---------------------------------------------


    protected Transform target;
    private float _timer = 0f;
    public Room room;

    Health _enemyHealth;
    public Health EnemyHealth { get { if (_enemyHealth == null) { _enemyHealth = GetComponent<Health>(); } return _enemyHealth; } }

    bool _initialized = false;
    
    
    //---------------------------------------------

    private void FixedUpdate()
    {
        if(!_initialized) return;

        if (Player.Instance.PlayerHealth.Dead) { return; }
        if (target == null) { return; }

        UpdateTimers();
        ShootIfAble();
        Move();
        FaceTarget();
    }


    //----------------------------------------------

    public virtual void Initialize(Room room, float difficulty)
    {
        this.room = room;
        difficultyMultiplier = difficulty;

        //asign player to target variable
        target = Player.Instance.transform;

        EnemyHealth.CurrentHealth *= difficultyMultiplier;

        _initialized = true;
    }

    protected virtual void FaceTarget()
    {
        transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, target.position);
    }

    private void UpdateTimers()
    {
        _timer += Time.fixedDeltaTime;
    }

    private void ShootIfAble()
    {
        if (target.gameObject.activeSelf && shootingDistance >= Vector2.Distance(transform.position, target.position))
        {
            ExecuteFirePattern();
        }
    }

    void ExecuteFirePattern()
    {
        
    }

    private void Shoot(Projectile projectile, ProjectileData data, Transform firePoint)
    {
        if (_muzzleFlash != null) { _muzzleFlash.Play(); }

        Projectile projectileInstnce = Instantiate(projectile, firePoint.position, Quaternion.Euler(0, 0, Random.Range(-data.Inaccuracy, data.Inaccuracy) + firePoint.rotation.eulerAngles.z));
        projectileInstnce.damage = data.ProjectileDamage;
        projectileInstnce.speed = data.ProjectileSpeed;
        projectileInstnce.shooter = gameObject;
    }

    protected virtual void Move()
    {
        if (Vector2.Distance(transform.position, target.position) > stopRange)
        {
            transform.position += movementSpeed * Time.fixedDeltaTime * transform.up;
        }
        else if (Vector2.Distance(transform.position, target.position) < backupRange)
        {
            transform.position -= playerAvoidanceSpeed * Time.fixedDeltaTime * transform.up;
        }
    }

    private void OnDestroy()
    {
        if(!_initialized) return;

        room.EndFightIfEnemiesDead();
    }
}
