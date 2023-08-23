using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LootTable", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{   
    // This list is populated from the editor
    public List<Rarity> rarities;

    // This is NonSerialized as we need it false everytime we run the game.
    // Without this tag, once set to true it will be true even after closing and restarting the game
    // Which means any future modification of our item list is not properly considered
    [System.NonSerialized] private bool isInitialized = false;

    private float _totalWeight;

    private void Initialize()
    {
        if (!isInitialized)
		{
            _totalWeight = rarities.Sum(item => item.weight);
            isInitialized = true;
		}
    }


    public Rarity GetRarity()
    {
        // Make sure it is initalized
        Initialize();

        // Roll our dice with _totalWeight faces
        float diceRoll = Random.Range(0f, _totalWeight);

			// Cycle through our items
        foreach (var item in rarities)
	    {
			// If item.weight is greater (or equal) than our diceRoll, we take that item and return
			if (item.weight >= diceRoll)
			{
				// Return here, so that the cycle doesn't keep running
				return item;
			}

			// If we didn't return, we substract the weight to our diceRoll and cycle to the next item
			diceRoll -= item.weight;
	    }
	    // As long as everything works we'll never reach this point, but better be notified if this happens!
	    throw new System.Exception("Reward generation failed!");
    }
}

[System.Serializable]
public class Rarity
{
	public string rarityName;
	public float weight;
}