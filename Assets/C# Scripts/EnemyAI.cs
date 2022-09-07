using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mike;

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
    [SerializeField] private float movementSpeed = 3;
    [SerializeField] private float playerAvoidanceSpeed = 3;
    [SerializeField] private float stopRange = 10;
    [SerializeField] private float backupRange = 10;
    [SerializeField] private float shootingDistance = 10;
    public float difficultyMultiplier = 1;


    //---------------------------------------------


    Transform target;
    float shotDelayTimer = 0.0f;
    public Room room;


    //---------------------------------------------


    void Start()
    {
        //asign player to target variable
        target = GameObject.FindGameObjectWithTag("Player").transform;

        GetComponent<Health>().OnDeath.AddListener(OnDeath);
        GetComponent<Health>().health *= difficultyMultiplier;
        projectileDamage *= difficultyMultiplier;
    }

    void Update()
    {
        transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, target.position);
        
        UpdateTimers();
        ShootIfAble();
        Move();
    }


    //----------------------------------------------


    void UpdateTimers()
    {
        if(shotDelayTimer < delayBetweenShots) shotDelayTimer += Time.deltaTime;
    }

    void ShootIfAble()
    {
        if (target.gameObject.GetHealth() && shootingDistance >= Vector2.Distance(transform.position, target.position) && shotDelayTimer >= delayBetweenShots)
        {
            gameObject.GetHealth();
            if (burst) StartCoroutine(FireBurst(projectilesPerBurst, delay)); else Shoot();
        }
    }

    void Shoot()
    {
        shotDelayTimer = 0;

        for (int i = 0; i < projectilesPerShot; i++)
        {
            Projectile projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.Euler(0,0,Random.Range(-inaccuracy, inaccuracy) + muzzle.rotation.eulerAngles.z)).GetComponent<Projectile>();
            projectile.damage = projectileDamage;
            projectile.speed = projectileSpeed;
            projectile.shooter = gameObject;
        }
    }

    IEnumerator FireBurst(int numberOfProjectiles, float delay)
    {
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Shoot();

            yield return new WaitForSeconds(delay);
        }
    }

    void Move()
    {
        if(Vector2.Distance(transform.position, target.position) > stopRange)
        {
            transform.position += movementSpeed * Time.deltaTime * transform.up;
        }
        else if(Vector2.Distance(transform.position, target.position) < backupRange) 
        {
            transform.position -= playerAvoidanceSpeed * Time.deltaTime * transform.up;
        }
    }

    void OnDeath()
    {
        room.EndFightIfEnemiesDead();
    }
}
