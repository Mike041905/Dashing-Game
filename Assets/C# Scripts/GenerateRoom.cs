using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoom : MonoBehaviour
{
    public const int spawnDistance = 60;
    public const float Z_OFFSET = 10;
    float ChanceOfRoomSpawnPrc { get => LevelGanerator.Instance.initialChanceOfRoomSpawnPrc * chanceMultiplier; }

    public Room room;

    [HideInInspector] public float chanceMultiplier = 1;

    [System.Serializable]
    public enum Side
    {
        Top,
        right,
        bottom,
        left
    }

    private void Awake()
    {
        room = GetComponent<Room>();
    }


    //-------------------------------------------


    public Vector2 GetSpawnPosition(Side side)
    {
        Vector2 dir = Vector2.up;
        switch (side)
        {
            case Side.Top: break;
            case Side.right: dir = Vector2.right; break;
            case Side.bottom: dir = Vector2.down; break;
            case Side.left: dir = Vector2.left; break;
        }

        return (Vector2)transform.position + dir * spawnDistance;
    }


    public void GenerateRooms()
    {
        for (int i = 0; i < 4; i++)//one side each
        {
            SpawnRoom((Side)i);
        }
    }

    void SpawnRoom(Side side)
    {
        if(Random.Range(0f, 100f) > ChanceOfRoomSpawnPrc) { return; }

        LevelGanerator.Instance.SpawnRoom(this, side);
    }
}
