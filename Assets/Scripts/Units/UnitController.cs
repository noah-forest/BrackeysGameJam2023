using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(UnitAnimator))]
[RequireComponent(typeof(UnitAttacker))]
[RequireComponent(typeof(UnitStats))]
public class UnitController : MonoBehaviour, ISlotItem
{
	[HideInInspector]
	public Health health;
	[HideInInspector]
	public UnitAttacker unitAttacker;
	UnitAnimator unitAnimator;
	[HideInInspector]
	public UnitStats unitStats;

	public Actor parentActor;
	public UnityEvent attacked;

	[HideInInspector]
	public UnityEvent blockedEvent;

	private Grave _grave;
	/// <summary>
	/// This is a property that will handle assigning the graveDug event whenever you assign a new grave to a unit
	/// </summary>
	public Grave unitGrave
	{
		set
		{
			if (_grave != null)
			{
				_grave.graveDug.RemoveListener(Respawn);
			}
			
			_grave = value;
			_grave.graveDug.AddListener(Respawn);
		}
		get => _grave;
	}

	private GameManager gameManager;
	private bool isAttacking;
	private bool inCombat;
	private float attackCooldownEnd;

	public void Awake()
	{
		gameManager = GameManager.singleton;

		gameObject.SetActive(false);

		health = GetComponent<Health>();
		unitAttacker = GetComponent<UnitAttacker>();
		unitStats = GetComponent<UnitStats>();
		unitAnimator = GetComponent<UnitAnimator>();

		unitAnimator.attackHitEvent.AddListener(DamageEnemyUnit);
		unitAnimator.attackEndEvent.AddListener(SetAttackTime);

		gameManager.battleStartedEvent.AddListener(BattleStarted);
		gameManager.battleEndedEvent.AddListener(BattleEnded);


		attacked.AddListener(OnUnitAttacked);
		health.died.AddListener(OnDeath);
		blockedEvent.AddListener(unitAnimator.PlayBlock);
		unitAttacker.critEvent.AddListener(unitAnimator.PlayCrit);
	}

	private void OnEnable()
	{
		inCombat = true;
		gameManager.combatBeganEvent.Invoke();
	}

	private void FixedUpdate()
	{
		if (!inCombat) return;

		if (health.isDead)
		{
			isAttacking = false;
			return;
		} else if(Time.time > attackCooldownEnd && !isAttacking)
		{
			Attack();
		}
	}

	private void SetAttackTime()
	{
		attackCooldownEnd = Time.time + unitStats.attackSpeed;
		isAttacking = false;
	}

	private void Attack()
	{
		isAttacking = true;
		StartAttack();
	}

	private void BattleStarted()
	{
		inCombat = true;
	}

	private void BattleEnded()
	{
		inCombat = false;
	}

	/// <summary>
	/// called by other units in order to deal damage to THIS unit
	/// </summary>
	/// <param name="dmg"></param>
	/// <param name="crit"></param>
	/// <param name="blocked"></param>
	public void TakeDamage(float dmg)
	{
		float remainder = health.health - dmg;

		// if the unit is dead when it would normally take damage, damage this units actor
		if (health.isDead)
		{
			if (!parentActor.health.isDead)
			{
				parentActor.health.TakeDamage(dmg);
			}

			return;
		}

		//check if the unit blocks the incoming damage
		bool blocked = false;
		float blockRoll = Random.value;
		if (blockRoll < unitStats.blockChance)
		{
			// blocked hit
			blockedEvent.Invoke();
			blocked = true;
		} // this is seperated out from the follwing 'damage the unit if' as other behavior may rely on block status.

		// damage the unit
		if (!blocked)
		{
            health.TakeDamage(dmg);
			if(health.health <= 0 && gameManager.overkillEnabled)
			{
				parentActor.health.TakeDamage(Mathf.Abs(remainder));
			}
			attacked.Invoke();
		};
	}

	public void InitCombat()
	{
		SetAttackTime();
	}

	private void StartAttack()
	{
		unitAnimator.Attack();
	}

	private void DamageEnemyUnit()
	{
		if (!unitAttacker.targetUnit)
		{
			gameManager.playerActor.health.TakeDamage(unitStats.damage); // this is fucking awful dont get me started. This only works cause the enemy will always have all 3 units
			return;
		}
		unitAttacker.AttackTarget();
	}

	private void OnUnitAttacked()
	{
		unitAnimator.Attacked();
	}

	private void OnDeath()
	{
		if (unitGrave)
		{
			unitGrave.ActivateGrave(unitStats.digCount);
			// go to grave
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObject Go = transform.GetChild(i).gameObject;
				Go.SetActive(false);
			}
		}
	}

	public void Respawn()
	{
		health.Revive();
		SetAttackTime();
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject Go = transform.GetChild(i).gameObject;
			Go.SetActive(true);
		}
	}

	public Sprite GetSlotSprite()
	{

		GameObject sprite = transform.Find("Sprite").gameObject;
		return sprite.GetComponent<SpriteRenderer>().sprite;
	}
}
