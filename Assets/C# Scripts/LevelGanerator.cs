using System;
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
    public int minRooms = 3;

    [SerializeField] GenerateRoom startingRoom; // NOTE: Starting room must me at position == Vector.Zero


    //----------------------------------------


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

    private void Start()
    {
        startingRoom.GenerateRooms();
    }


    //----------------------------------------


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
        rooms[rooms.Length - 1] = room;
    }

    internal void RegenerateLevel()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            Destroy(rooms[i]);
        }

        rooms = new Room[0];
    }

    internal void SpawnRoom(GenerateRoom parentRoom, GenerateRoom.Side side)
    {
        // connect room if place taken (doesnt account for already existing connections)
        if (TryGetRoomAtPosition(parentRoom.GetSpawnPosition(side), out Room occupier)) 
        {
            parentRoom.room.ConnectRoom(occupier);
            return;
        }

        //instantiate room and lower spawn chance
        GameObject newRoom = Instantiate(roomPrefab, (Vector3)parentRoom.GetSpawnPosition(side) + new Vector3(0, 0, GenerateRoom.Z_OFFSET), Quaternion.identity);
        newRoom.GetComponent<GenerateRoom>().chanceMultiplier = parentRoom.chanceMultiplier / roomChanceDeprecation;

        AddRoom(newRoom.GetComponent<Room>());
        if (parentRoom.room != null) newRoom.GetComponent<Room>().descendant = parentRoom.room.descendant + 1;
    }
}
