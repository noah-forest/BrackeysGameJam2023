using UnityEngine;

[RequireComponent(typeof(Health))]
public class Actor : MonoBehaviour
{
	// this is the player / enemy
	[HideInInspector]
	public Health health;

	public int actorHealth;

	private GameManager gameManager;

	public UnitController[] units = new UnitController[3];

	private void Awake()
	{
		health = GetComponent<Health>();
		health.InitHealth(actorHealth);
	}
}
