using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Dashing Game/Boss Behaviours/BossDash")]
public class BossDashBehaviour : BossBehaviour
{
    [SerializeField] BossAI _boss;

    [SerializeField] float _cooldown = 10;
    [SerializeField] float _speed = 10;
    [SerializeField] float _distance = 10;

    float _cooldownTimer = Mathf.Infinity;

    void Start()
    {

    }

    void Update()
    {
        if(_cooldownTimer < _cooldown) { _cooldownTimer += Time.deltaTime; }
    }
}
