using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Dash), typeof(Health))]
public class Player : MonoBehaviour
{
	static Player _instance;
	public static Player Instance { get => _instance; }

	private void Awake()
	{
		_instance = this;
	}

	Dash _dash;
	public Dash PlayerDash { get { return _dash = _dash != null ? _dash : GetComponent<Dash>(); } }

	Health _health;
	public Health PlayerHealth { get { return _health = _health != null ? _health : GetComponent<Health>(); } }

	Room _currentRoom;
	public Room CurrentRoom { get { return _currentRoom = _currentRoom != null ? _currentRoom : Mike.MikeGameObject.GetClosestTargetWithTag(transform.position, "Room").GetComponent<Room>(); } set => _currentRoom = value; }

    [SerializeField] private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer { get { return _spriteRenderer = _spriteRenderer != null ? _spriteRenderer : GetComponent<SpriteRenderer>(); } }

    public void OnDeath()
	{
		SetCommon(false);

		PlayerDash.OnDeath();
    }

	public void OnRevive()
	{
		SetCommon(true);

		PlayerDash.OnRevive();
    }

    public void SetCommon(bool isAlive)
	{
        GetComponent<Collider2D>().enabled = isAlive;
        PowerUpAdder.Instance.gameObject.SetActive(isAlive);
    }
}
