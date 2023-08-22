using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class Grave : MonoBehaviour
{
	public UnityEvent graveDug;

	private bool inGrave;
	private int currentDigCount;

	private int digCount;

	public void ActivateGrave(int digCount)
	{
		this.digCount = digCount;
		inGrave = true;
		currentDigCount = 0;
	}

	public void Dig()	//called by button onClick function
	{
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
		Dig();
	}
}
