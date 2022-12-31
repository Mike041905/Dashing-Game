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
    [HideInInspector] public Room[] Rooms = new Room[0];


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
        while (Rooms.Length < minRooms)
        {
            startingRoom.GenerateRooms();
        }

        if (GameManager.Insatnce.IsBossLevel)
        {
            ChooseBossRoom();
        }
        else
        {
            SpawnPortalRoom();
        }
    }

    public bool TryGetRoomAtPosition(Vector2 worldPosition, out Room outRoom)
    {
        if(worldPosition == Vector2.zero) { outRoom = startingRoom.room; return true; }

        outRoom = null;

        foreach (var room in Rooms)
        {
            if ((Vector2)room.transform.position == worldPosition) { outRoom = room; return true; }
        }

        return false;
    }
    
    public bool TryGetRoomAtPosition(Vector2Int gridPosition, out Room outRoom)
    {
        if (gridPosition == Vector2Int.zero) { outRoom = startingRoom.room; return true; }

        outRoom = null;

        foreach (Room room in Rooms)
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
        Room[] temp = Rooms;
        Rooms = new Room[Rooms.Length + 1];
        temp.CopyTo(Rooms, 0);
        Rooms[^1] = room;
    }

    public void RegenerateLevel()
    {
        PersistenceManager.Instance.WipeLevel();
        Rooms = new Room[0];
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
        int choice = 0;

        for (int i = 0; i < Rooms.Length; i++)
        {
            if (Rooms[choice].descendant < Rooms[i].descendant) { choice = i; }
            else if(Rooms[choice].descendant == Rooms[i].descendant) // chooses randomly
            {
                if(UnityEngine.Random.Range(0, 4) == 0)
                {
                    choice = i;
                }
            }
        }

        MakeBossRoom(choice);
    }

    void MakeBossRoom(int roomIndex)
    {
        Rooms[roomIndex] = Rooms[roomIndex].ChangeToBossRoom();
    }
    
    void MakePortalRoom(int roomIndex)
    {
        Rooms[roomIndex].enabled = false;
        GameManager.Insatnce.SpawnPortal(Rooms[roomIndex]);
    }

    void SpawnPortalRoom()
    {
        int choice = 0;

        for (int i = 0; i < Rooms.Length; i++)
        {
            if (Rooms[choice].descendant < Rooms[i].descendant) { choice = i; }
            else if (Rooms[choice].descendant == Rooms[i].descendant) // chooses randomly
            {
                if (UnityEngine.Random.Range(0, 4) == 0)
                {
                    choice = i;
                }
            }
        }

        MakePortalRoom(choice);
    }
}
