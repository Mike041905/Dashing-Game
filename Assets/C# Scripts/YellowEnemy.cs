using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowEnemy : EnemyAI
{
    [Header("Yellow Enemy Stats")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float AttackBreakOffDistance = 2.5f;
    [SerializeField] private float pointPositionTolerance = .5f;

    private State currentState = State.Attacking;

    Vector2[] path = new Vector2[2];
    int currentPathTargetIndex = 0;
    public int PathIndex { get => currentPathTargetIndex; set { currentPathTargetIndex = value; if (currentPathTargetIndex >= path.Length) { currentPathTargetIndex = 0; } } }

    enum State
    {
        Attacking,
        PreparingForAttack
    }

    protected override void Move()
    {
        if(Target != null)
        {
            switch (currentState)
            {   
                case State.Attacking: Attack(); break;
                case State.PreparingForAttack: PrepareForAttack(); break;
            }
        }
    }

    void Attack()
    {
        if (Vector2.Distance(transform.position, Target.position) > AttackBreakOffDistance)
        {
            transform.position += movementSpeed * Time.deltaTime * transform.up;
            FaceTarget();
        }
        else
        {
            GeneratePath();
            currentState = State.PreparingForAttack;
        }
    }

    void PrepareForAttack()
    {
        transform.position += movementSpeed * Time.deltaTime * transform.up;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, MikeTransform.Rotation.LookTwards(transform.position, path[currentPathTargetIndex]), turnSpeed * Time.deltaTime);
        MoveOnPath();
    }


    //----------------------


    void MoveOnPath()
    {
        if (Vector2.Distance((Vector2)transform.position, path[currentPathTargetIndex]) <= pointPositionTolerance)
        {
            if (PathIndex == path.Length - 1) { currentState = State.Attacking; }
            PathIndex++;
        }
    }

    protected override void FaceTarget()
    {
        if(currentState == State.PreparingForAttack) { return; }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, MikeTransform.Rotation.LookTwards(transform.position, Target.position), turnSpeed * Time.deltaTime);
    }

    void GeneratePath()
    {
        Path1();
        Path2();
    }

    bool ValidateRoute(Vector2 finalPosition, Transform[] exclusions)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, finalPosition, Vector2.Distance(transform.position, finalPosition));

        if(hits.Length == 0) { return true; }

        foreach (RaycastHit2D hit in hits)
        {
            if(!MikeArray.Contains(exclusions, hit.transform)) { return false; }
        }

        return true;
    }

    void Path1()
    {
        // left -1, right 1
        int dir = Random.Range(-1, 2);
        Vector2 position = transform.position + transform.right * dir * 7;

        for (int i = 0; i < 2; i++)
        {
            if (ValidateRoute(position, new Transform[] { transform })) { path[0] = position; return; }

            dir *= -1;
        }

        for (int i = 0; i < 10; i++)
        {
            //Generate pos
            position = MikeRandom.RandomVector2(-10, 10, -10, 10) + (Vector2)transform.position;

            // find pos far from player
            if(Vector2.Distance(position, (Vector2)Target.position) > 7)
            {
                //check if doesnt hit anything
                if (ValidateRoute(position, new Transform[] { transform } )) { path[0] = position; return; }
            }
        }
    }

    void Path2()
    {
        Vector2 position = Vector2.zero;

        for (int i = 0; i < 10; i++)
        {
            position = path[0] + (Vector2)transform.up * 15 + MikeRandom.RandomVector2(-10, 10);
            RaycastHit2D[] hits = Physics2D.RaycastAll(path[0], position);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform == transform) { continue; }
                if (hit.transform.CompareTag("Barrier") && Vector2.Distance(hit.point, path[0]) < 15)
                {
                    path[1] = hit.point - (Vector2)transform.up * 2;
                    return;
                }
            }
            path[1] = position;
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision == null) { return; }
        if (collision.transform.CompareTag("Barrier")) { GetComponent<Health>().Die(); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, path[currentPathTargetIndex]);
        Gizmos.DrawLine(path[0], path[1]);

        Gizmos.DrawSphere(path[0], pointPositionTolerance);
        Gizmos.DrawSphere(path[1], pointPositionTolerance);
    }
}
