using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowEnemy : EnemyAI
{
    [Header("Yellow Enemy Stats")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float pointPositionTolerance = .5f;

    private State currentState = State.PreparingForAttack;
    Vector2[] path;

    enum State
    {
        Attacking,
        PreparingForAttack
    }

    protected override void Move()
    {
        if(target != null)
        {
            switch (currentState)
            {   
                case State.Attacking:
                    break;
                case State.PreparingForAttack:
                    break;
            }
        }
    }

    void Attack()
    {
        
    }

    void PrepareForAttack()
    {

    }

    protected override void FaceTarget()
    {
        if(currentState == State.Attacking)

        base.FaceTarget();
    }

    void GeneratePath()
    {

    }
}
