using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class Grave : MonoBehaviour
{
	public bool playerOwned;

	public UnityEvent graveDug;
	private bool inGrave;
	private int currentDigCount;

	private MouseUtils mouseUtils;
	
	private int digCount;
	private float digSpeed = 0.25f; //used by enemy 

	private void Start()
	{
		mouseUtils = MouseUtils.singleton;
	}

	public void ActivateGrave(int digCount)
	{
		this.digCount = digCount;
		inGrave = true;
		currentDigCount = 0;
        if (!playerOwned)
        {
			StartCoroutine(EnemyDig());
        }
	}

	public void Dig()
	{
		Debug.Log($"grave: { gameObject.name} was dug");
		if(!inGrave) return;
		currentDigCount++;
        
		if (currentDigCount >= digCount)
		{
			inGrave = false;
			graveDug.Invoke();
		}
	}
	
	void OnMouseDown() // when collider is clicked
	{
		Debug.Log($"Grave: {gameObject.name} was clicked\n PlayerOwned?: {playerOwned}");
        if (playerOwned) Dig(); // enemies cannot be dug
	}
	
	/// <summary>
	/// Enemy graves return units to life on cooldown instead of being clicked.
	/// Technically, it clicks the grave on intervals so the enemy. 
	/// </summary>
	/// <returns></returns>
	IEnumerator EnemyDig()
    {
		yield return new WaitForSeconds(digSpeed);
		Dig();
		if (inGrave == true) StartCoroutine(EnemyDig()); //loops until the dig function sets inGrave to false
    }
}
