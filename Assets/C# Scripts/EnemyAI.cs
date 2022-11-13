using Mike;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;

    [Header("Projectile Options")]
    [SerializeField] private float projectileDamage = 10;
    [SerializeField] private float projectileSpeed = 10;
    [SerializeField] private float delayBetweenShots = 1;
    [SerializeField] private int projectilesPerShot = 1;
    [SerializeField] private float inaccuracy = 1;

    [Header("Burst Settings")]
    [SerializeField] private bool burst = false;
    [SerializeField] private int projectilesPerBurst = 3;
    [SerializeField] private float delay = .05f;

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
    private float shotDelayTimer = 0.0f;
    public Room room;


    //---------------------------------------------


    public virtual void Start()
    {
        //asign player to target variable
        target = Player.Instance.transform;

        GetComponent<Health>().CurrentHealth *= difficultyMultiplier;
        projectileDamage *= difficultyMultiplier;
    }

    private void FixedUpdate()
    {
        if (Player.Instance.PlayerHealth.Dead) { return; }

        UpdateTimers();
        ShootIfAble();
        Move();
        FaceTarget();
    }


    //----------------------------------------------


    protected virtual void FaceTarget()
    {
        transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, target.position);
    }

    private void UpdateTimers()
    {
        if (shotDelayTimer < delayBetweenShots) shotDelayTimer += Time.fixedDeltaTime;
    }

    private void ShootIfAble()
    {
        if (target.gameObject.activeSelf && shootingDistance >= Vector2.Distance(transform.position, target.position) && shotDelayTimer >= delayBetweenShots)
        {
            if (burst) StartCoroutine(FireBurst(projectilesPerBurst, delay)); else Shoot();
        }
    }

    private void Shoot()
    {
        shotDelayTimer = 0;

        for (int i = 0; i < projectilesPerShot; i++)
        {
            Projectile projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.Euler(0, 0, Random.Range(-inaccuracy, inaccuracy) + muzzle.rotation.eulerAngles.z)).GetComponent<Projectile>();
            projectile.damage = projectileDamage;
            projectile.speed = projectileSpeed;
            projectile.shooter = gameObject;
        }
    }

    private IEnumerator FireBurst(int numberOfProjectiles, float delay)
    {
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Shoot();

            yield return new WaitForSeconds(delay);
        }
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
        room.EndFightIfEnemiesDead();
    }
}
