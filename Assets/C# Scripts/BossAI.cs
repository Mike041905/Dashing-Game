using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI
{
    [field: SerializeField] public string BossName { get; private set; }
    [field: SerializeField] public Transform BossBehaviourHolder { get; private set; }

    public override void Initialize(Room room, float difficulty)
    {
        base.Initialize(room, difficulty);

        BossBar.Instance.DisplayBar(this);
    }
}

