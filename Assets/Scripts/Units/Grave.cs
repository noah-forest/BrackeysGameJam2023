using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class Grave : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public bool playerOwned;
	private ParticleSystem digParticle;
	public UnityEvent graveDug;
	private bool inGrave;
	private int currentDigCount;
	
	private MouseUtils mouseUtils;
	
	private int digCount;
	private float digSpeed = 0.40f; //used by enemy 
	public Sprite[] digLevelSprites;
	private SpriteRenderer sprite;

	private void Start()
	{
		digParticle = GetComponent<ParticleSystem>();
		sprite = GetComponent<SpriteRenderer>();
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
		sprite.sprite = digLevelSprites[digLevelSprites.Length-1];
	}

	public void Dig()
	{
		
		if(!inGrave) return;
		currentDigCount++;

		int spriteInterp = (int)Mathf.Lerp(4, 1, (float)(currentDigCount) / (float)digCount);

		if (currentDigCount >= digCount)
		{
			inGrave = false;
			spriteInterp = 0;
			graveDug.Invoke();
		}
		digParticle.Play();

		sprite.sprite = digLevelSprites[spriteInterp];
	}
	
	void OnMouseDown() // when collider is clicked by player
	{
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

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseUtils.SetHoverCursor();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseUtils.SetToDefaultCursor();
	}
}
