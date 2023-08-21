using UnityEngine;

[RequireComponent(typeof(Health))]
public class Actor : MonoBehaviour
{
	// this is the player / enemy
	public Health health;

	private GameManager gameManager;

	private void Start()
	{
		gameManager = GameManager.singleton;
		health.died.AddListener(OnActorDeath);
	}

	private void OnActorDeath()
	{
		gameManager.ActorDead(this.gameObject);
	}
}
