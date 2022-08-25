using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoom : MonoBehaviour
{
    private GameObject roomPrefab;
    private float chanceOfRoomSpawnPrc;

    [HideInInspector]
    public float chanceMultiplier = 1;

    enum Side
    {
        Top,
        right,
        bottom,
        left
    }

    private void Start()
    {
        LevelGanerator.Instance.AddRoom(GetComponent<Room>());
        roomPrefab = LevelGanerator.Instance.roomPrefab;
        chanceOfRoomSpawnPrc = LevelGanerator.Instance.initiaChanceOfRoomSpawnPrc * chanceMultiplier;

        if(GetComponent<Room>() != null && GetComponent<Room>().descendant < LevelGanerator.Instance.maxBranch) GenerateRooms();
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
        if (!GetComponent<Room>().enabled && side == Side.Top) { }
        else if(Random.Range(0f, 100f) > chanceOfRoomSpawnPrc) { return; }

        //initializ variable
        Vector2 spawnPosition = Vector2.zero;
        Quaternion spawnRotation = Quaternion.identity;
        int unlockDoor = 0;

        //set variable acordingly to the side at which they spawn
        switch (side)
        {
            case Side.Top:
                spawnPosition = transform.position + Vector3.up * 60;
                spawnRotation = Quaternion.Euler(0, 0, 0);
                if (!LevelGanerator.Instance.CheckIfPositionVacant(spawnPosition)) { return; }
                transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>().isTrigger = true;
                transform.GetChild(0).GetChild(0).tag = "Door";
                unlockDoor = 2;
                break;
            case Side.left:
                spawnPosition = transform.position + Vector3.left * 60;
                spawnRotation = Quaternion.Euler(0, 0, 90);
                if (!LevelGanerator.Instance.CheckIfPositionVacant(spawnPosition)) { return; }
                transform.GetChild(0).GetChild(1).GetComponent<BoxCollider2D>().isTrigger = true;
                transform.GetChild(0).GetChild(1).tag = "Door";
                unlockDoor = 3;
                break;
            case Side.bottom:
                spawnPosition = transform.position + Vector3.down * 60;
                spawnRotation = Quaternion.Euler(0, 0, 180);
                if (!LevelGanerator.Instance.CheckIfPositionVacant(spawnPosition)) { return; }
                transform.GetChild(0).GetChild(2).GetComponent<BoxCollider2D>().isTrigger = true;
                transform.GetChild(0).GetChild(2).tag = "Door";
                unlockDoor = 0;
                break;
            case Side.right:
                spawnPosition = transform.position + Vector3.right * 60;
                spawnRotation = Quaternion.Euler(0, 0, 270);
                if (!LevelGanerator.Instance.CheckIfPositionVacant(spawnPosition)) { return; }
                transform.GetChild(0).GetChild(3).GetComponent<BoxCollider2D>().isTrigger = true;
                transform.GetChild(0).GetChild(3).tag = "Door";
                unlockDoor = 1;
                break;
        }

        //instantiate room and lower spawn chance
        GameObject go = Instantiate(roomPrefab, (Vector3)spawnPosition + new Vector3(0,0,10), spawnRotation);
        go.GetComponent<GenerateRoom>().chanceMultiplier = chanceMultiplier / LevelGanerator.Instance.roomChanceDeprecation;
        go.transform.GetChild(0).rotation = Quaternion.Euler(Vector3.zero);
        go.transform.GetChild(0).GetChild(unlockDoor).GetComponent<BoxCollider2D>().isTrigger = true;
        go.transform.GetChild(0).GetChild(unlockDoor).tag = "Door";
        GetComponent<Room>().SetInitialDoorStates();
        if (GetComponent<Room>() != null) go.GetComponent<Room>().descendant = GetComponent<Room>().descendant + 1;

        //add spawn position to occuipants array
        LevelGanerator.Instance.AddOccupiedPosition(spawnPosition);
    }
}
