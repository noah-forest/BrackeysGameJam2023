using UnityEngine;
using Random = UnityEngine.Random;

public class TreasureHazard : BoatHazard
{
	[SerializeField] private int maxReward = 8;
	[SerializeField] private int minReward = 2;
	private int treasureAmount;
	
	public override void InteractWithBoat(BoatMaster boat, Collision collision)
	{
		base.InteractWithBoat(boat, collision);
		if (boat.gameManager)
		{
			treasureAmount = Random.Range(minReward, maxReward+1);
			boat.gameManager.Cash += treasureAmount;
		}
		Destroy(gameObject);
	}
}
