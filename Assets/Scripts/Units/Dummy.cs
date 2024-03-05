using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, ISlotPayloadChangeHandler
{
	public string Name;
	public int Slot;
	public int currentLevel;

	public GameObject unitInstance;

	public void SlotPayloadChanged(GameObject payload)
	{
		if(payload != null)
		{
			unitInstance = Instantiate(payload);
			unitInstance.SetActive(false);
		} else
		{

		}
	}
}
