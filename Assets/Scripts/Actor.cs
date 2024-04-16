using UnityEngine;

[RequireComponent(typeof(Health))]
public class Actor : MonoBehaviour
{
	// this is the player / enemy
	[HideInInspector]
	public Health health;

	private GameManager gameManager;

	public UnitController[] units = new UnitController[3];

	private void Awake()
	{
		health = GetComponent<Health>();
		health.InitHealth(125);
	}

	//public InitializeRandomTeam()
 //   {
	//	for(int i = 0; i < units.Length; i++)
 //       {
			
 //       }
 //   }
}
