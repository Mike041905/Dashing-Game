using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGanerator : MonoBehaviour
{
    [Header("Essential")]
    public GameObject roomPrefab;
    [Range(0f, 100f)] public float initialChanceOfRoomSpawnPrc = 15f;
    public float roomChanceDeprecation = 1.5f;
    public int maxBranch = 4;
    public int minRooms = 3;

    [SerializeField] GenerateRoom startingRoom; // NOTE: Starting room must me at position == Vector.Zero


    //----------------------------------------


    // IDK why this isn't a list
    [HideInInspector] public Room[] rooms = new Room[0];


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
    }


    //----------------------------------------

    public void GenerateLevel()
    {
        while (rooms.Length < minRooms)
        {
            startingRoom.GenerateRooms();
        }

        if (GameManager.Insatnce.IsBossLevel)
        {
            ChooseBossRoom();
        }
    }

    public bool TryGetRoomAtPosition(Vector2 worldPosition, out Room outRoom)
    {
        if(worldPosition == Vector2.zero) { outRoom = startingRoom.room; return true; }

        outRoom = null;

        foreach (var room in rooms)
        {
            if ((Vector2)room.transform.position == worldPosition) { outRoom = room; return true; }
        }

        return false;
    }
    
    public bool TryGetRoomAtPosition(Vector2Int gridPosition, out Room outRoom)
    {
        if (gridPosition == Vector2Int.zero) { outRoom = startingRoom.room; return true; }

        outRoom = null;

        foreach (Room room in rooms)
        {
            if (room.PositionInGrid == gridPosition) { outRoom = room; return true; }
        }

        return true;
    }

    public bool CheckIfPositionVacant(Vector2 position)
    {
        return !TryGetRoomAtPosition(position, out _);
    }

    public void AddRoom(Room room)
    {
        Room[] temp = rooms;
        rooms = new Room[rooms.Length + 1];
        temp.CopyTo(rooms, 0);
        rooms[^1] = room;
    }

    public void RegenerateLevel()
    {
        PersistenceManager.Instance.WipeLevel();
        rooms = new Room[0];
        startingRoom.room.ResetDoors();
        GenerateLevel();
    }

    public void SpawnRoom(GenerateRoom parentRoom, GenerateRoom.Side side)
    {
        // connect room if place taken (doesnt account for already existing connections)
        if (TryGetRoomAtPosition(parentRoom.GetSpawnPosition(side), out Room occupier)) 
        {
            parentRoom.room.ConnectRoom(occupier);
            return;
        }

        //instantiate room and lower spawn chance
        GenerateRoom newRoom = Instantiate(roomPrefab, (Vector3)parentRoom.GetSpawnPosition(side) + new Vector3(0, 0, GenerateRoom.Z_OFFSET), Quaternion.identity).GetComponent<GenerateRoom>();
        newRoom.chanceMultiplier = parentRoom.chanceMultiplier / roomChanceDeprecation;

        AddRoom(newRoom.room);
        if (parentRoom.room != null) newRoom.room.descendant = parentRoom.room.descendant + 1;

        parentRoom.room.ConnectRoom(newRoom.room);
        newRoom.GenerateRooms();
    }

    void ChooseBossRoom()
    {
        Room choice = rooms[0];

        foreach (Room room in rooms)
        {
            if(choice.descendant < room.descendant) { choice = room; }
            else if(choice.descendant == room.descendant) // chooses randomly
            {
                if(UnityEngine.Random.Range(0, 4) == 0)
                {
                    choice = room;
                }
            }
        }

        MakeBossRoom(choice);
    }

    void MakeBossRoom(Room room)
    {
        room.ChangeToBossRoom();
    }
}
