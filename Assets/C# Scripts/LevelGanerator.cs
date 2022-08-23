using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGanerator : MonoBehaviour
{
    [Header("Essential")]
    public GameObject roomPrefab;
    [Range(0f, 100f)] public float initiaChanceOfRoomSpawnPrc = 15f;
    public float roomChanceDeprecation = 1.5f;
    public int maxBranch = 4;


    //----------------------------------------


    [HideInInspector] public Room[] rooms = new Room[0];
    Vector2[] occpiedPositions = new Vector2[0];


    //----------------------------------------


    public static LevelGanerator Instance
    {
        get
        {
            return _Instance;
        }
    }
    static LevelGanerator _Instance;


    //----------------------------------------


    private void Awake()
    {
        _Instance = this;
        AddOccupiedPosition(Vector2.zero);
    }


    //----------------------------------------



    public bool CheckIfPositionVacant(Vector2 position)
    {
        foreach (var item in occpiedPositions)
        {
            if(item == position) { return false; }
        }
        return true;
    }

    public void AddOccupiedPosition(Vector2 position)
    {
        Vector2[] temp = occpiedPositions;
        occpiedPositions = new Vector2[occpiedPositions.Length + 1];
        temp.CopyTo(occpiedPositions, 0);
        occpiedPositions[occpiedPositions.Length - 1] = position;
    }

    public void AddRoom(Room room)
    {
        Room[] temp = rooms;
        rooms = new Room[rooms.Length + 1];
        temp.CopyTo(rooms, 0);
        rooms[rooms.Length - 1] = room;
    }
}
