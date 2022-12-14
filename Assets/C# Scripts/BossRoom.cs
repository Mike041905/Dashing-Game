using EZCameraShake;
using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Room;

public class BossRoom : Room
{
    public BossAI Boss { get; private set; }

    EnemyManager.Boss _boss;

    BossAI _dummy;

    public BossRoom Initialize(Door[] doors, SpriteRenderer minimapIcon, Color normalColor, Color completecolor)
    {
        _boss = EnemyManager.Instance.GetBossForCurrentLevel();
        this.doors = doors;
        InitializeDoors();

        // This is a dummy do not initialize!
        _dummy = Instantiate(_boss.BossPrefab, transform.position, Quaternion.identity);
        _dummy.gameObject.tag = "Untagged";
        Destroy(_dummy.EnemyHealth);

        _minimapIcon = minimapIcon;
        _normalColor = normalColor;
        _completedColor = completecolor;

        return this;
    }

    void InitializeDoors()
    {
        foreach (Door door in doors)
        {
            door.OnEnteredThroughDoor += OnDoorTrigger;
        }
    }

    protected override void OnDoorTrigger(Collider2D collider)
    {
        base.OnDoorTrigger(collider);
    }

    protected override void SpawnEnemies(List<EnemyManager.Enemy> enemies, float tickets)
    {
        Boss = Instantiate(_boss.BossPrefab, _dummy.transform.position, _dummy.transform.rotation);
        Destroy(_dummy.gameObject);

        Boss.Initialize(this, GameManager.Insatnce.Difficulty);
        spawnedEnemies = spawnedEnemies.Append(Boss.gameObject);


        base.SpawnEnemies(new(_boss.Enemies), EnemyManager.Instance.GetSpawnTickets(_boss.EnemySpawnTickets));
    }

    public override void EndFightIfEnemiesDead()
    {
        base.EndFightIfEnemiesDead();

        if (!CheckIfAllEnemiesDead()) { return; }
        GameManager.Insatnce.SpawnPortal(this);
    }
}
