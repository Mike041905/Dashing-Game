using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// OMG WTF DID I DO IN THIS SCRIPT!!! THIS WHOLE CODE IS A MESS AND I DONT WANT TO FIX IT!
public class GenerateRoom : MonoBehaviour
{
    public const int spawnDistance = 60;
    public const float Z_OFFSET = 10;
    float ChanceOfRoomSpawnPrc { get => LevelGanerator.Instance.initiaChanceOfRoomSpawnPrc * chanceMultiplier; }

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
        Vector2 dir = Vector2.zero;
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
        if (room.enabled && side == Side.Top) { } // I have no Idea why i wrote this if like that
        else if(Random.Range(0f, 100f) > ChanceOfRoomSpawnPrc) { return; }

        LevelGanerator.Instance.SpawnRoom(this, side);

        /*//initializ variable
        Vector2 spawnPosition = Vector2.zero;
        Quaternion spawnRotation = Quaternion.identity;
        int unlockDoor = 0;

        //set variable acordingly to the side at which they spawn
        spawnPosition = transform.position + Vector3.up * 60;
        spawnRotation = Quaternion.Euler(0, 0, 0);
        if (!LevelGanerator.Instance.CheckIfPositionVacant(spawnPosition)) { return; }
        transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>().isTrigger = true;
        transform.GetChild(0).GetChild(0).tag = "Door";
        unlockDoor = (int)side;


        //instantiate room and lower spawn chance
        GameObject go = Instantiate(LevelGanerator.Instance.roomPrefab, (Vector3)spawnPosition + new Vector3(0,0,10), spawnRotation);
        go.GetComponent<GenerateRoom>().chanceMultiplier = chanceMultiplier / LevelGanerator.Instance.roomChanceDeprecation;
        go.transform.GetChild(0).rotation = Quaternion.Euler(Vector3.zero);
        go.transform.GetChild(0).GetChild(unlockDoor).GetComponent<BoxCollider2D>().isTrigger = true;
        go.transform.GetChild(0).GetChild(unlockDoor).tag = "Door";
        room.SetInitialDoorStates();
        if (room != null) go.GetComponent<Room>().descendant = room.descendant + 1;

        //add spawn position to occuipants array
        LevelGanerator.Instance.AddOccupiedPosition(spawnPosition);*/

    }
}
