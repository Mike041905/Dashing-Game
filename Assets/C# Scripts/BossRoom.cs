using EZCameraShake;
using Mike;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Room;

public class BossRoom : Room
{
    [SerializeField] Door[] _doors;

    public BossAI Boss { get; private set; }

    public BossRoom Initialize(Door[] doors)
    {
        _doors = doors;
        InitializeDoors();

        return this;
    }

    void InitializeDoors()
    {
        foreach (Door door in _doors)
        {
            door.OnEnteredThroughDoor += OnDoorTrigger;
        }
    }

    protected override void OnDoorTrigger(Collider2D collider)
    {
        base.OnDoorTrigger(collider);
    }
}
