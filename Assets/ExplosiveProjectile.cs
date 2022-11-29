using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] EnemyAI.ProjectileData _shrapnel;
    [SerializeField] float _shrapnelAmount;
    [SerializeField] float _fuseTime = 5;

    [SerializeField] bool _randomSpread = false;

    private async void Start()
    {
        await Task.Delay((int)(_fuseTime * 1000));

        if(this == null) { return; }
        if(!isActiveAndEnabled) { return; }

        Hit();
    }

    protected override void Die()
    {
        Explode();

        base.Die();
    }

    void Explode()
    {
        if (_randomSpread) { SpawnShrapnelRandom(_shrapnel, _shrapnelAmount); }
        else { SpawnShrapnel(_shrapnel, _shrapnelAmount); }
    }

    void SpawnShrapnel(EnemyAI.ProjectileData shrapnel, float amount)
    {
        float increment = 360 / amount;

        for (int i = 0; i < amount; i++)
        {
            shrapnel.Shoot(increment * i, 0);
        }
    }

    void SpawnShrapnelRandom(EnemyAI.ProjectileData shrapnel, float amount)
    {
        for (int i = 0; i < amount; i++)
        {
            shrapnel.Shoot();
        }
    }
}
