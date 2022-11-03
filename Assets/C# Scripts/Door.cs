using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Door,
        Barrier
    }

    [SerializeField] GameObject doorSprite;
    public GenerateRoom.Side side;


    DoorType doorType = DoorType.Barrier;
    public DoorType Type { get => doorType; set { doorType = value; BoxCollider.isTrigger = doorType == 0 ; } }
    
    public bool IsOpen { get => BoxCollider.isTrigger; set { if (Type == DoorType.Door) { BoxCollider.isTrigger = value; doorSprite.SetActive(!value); } } } 

    BoxCollider2D boxCollider;
    public BoxCollider2D BoxCollider { get { if (boxCollider == null) { boxCollider = GetComponent<BoxCollider2D>(); } return boxCollider; } }

    public event UnityAction<Collider2D> OnEnteredThroughDoor;


    //--------------


    private void OnTriggerExit2D(Collider2D collider)
    {
        OnEnteredThroughDoor?.Invoke(collider);
    }
}