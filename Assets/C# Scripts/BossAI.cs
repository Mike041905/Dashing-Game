using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI
{
    [field: SerializeField] public string BossName { get; private set; }
    [field: SerializeField] public Transform BossBehaviourHolder { get; private set; }

    public override void Start()
    {
        base.Start();

        BossBar.Instance.DisplayBar(this);
    }
}
