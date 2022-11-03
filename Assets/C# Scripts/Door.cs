using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Door,
        Barrier
    }

    public GenerateRoom.Side side;

    DoorType doorType = DoorType.Barrier;
    public DoorType Type { get => doorType; set { doorType = value; boxCollider.isTrigger = doorType == 0 ? true : false ; } }
    public bool IsOpen { get => boxCollider.isTrigger; set { if (Type == DoorType.Door) { boxCollider.isTrigger = value; } } } 

    BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
}