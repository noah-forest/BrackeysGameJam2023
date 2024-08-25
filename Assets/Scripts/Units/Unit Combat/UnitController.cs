using Assets.Scripts.Units;
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
	[HideInInspector]
	public Actor unitOwner;
	[HideInInspector]
	public UnitPerformance unitPerformanceAllTime;
    [HideInInspector]
    public UnitPerformance unitPerformanceLastBattle;

    public UnityEvent<UnitPerformance> performanceUpdatedEvent;
	public Actor parentActor;


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
	private BattleManager battleManager;
	private bool isAttacking;
	private bool inCombat;
	private float attackCooldownEnd;

	public void Awake()
	{
		gameManager = GameManager.singleton;
		battleManager = BattleManager.singleton;

		gameObject.SetActive(false);

		health = GetComponent<Health>();
		unitAttacker = GetComponent<UnitAttacker>();
		unitStats = GetComponent<UnitStats>();
		unitAnimator = GetComponent<UnitAnimator>();

		unitAnimator.attackHitEvent.AddListener(DamageEnemyUnit);
		unitAnimator.attackEndEvent.AddListener(SetAttackTime);

		gameManager.battleStartedEvent.AddListener(BattleStarted);
		gameManager.battleEndedEvent.AddListener(BattleEnded);


		health.attackedEvent.AddListener(OnUnitAttacked);
		health.died.AddListener(OnDeath);
		health.blockedEvent.AddListener(unitAnimator.PlayBlock);

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
        health.blockChance = unitStats.blockChance; // this may cause problems later due to block not being able to be updated mid combat.
        unitPerformanceLastBattle = new();
		performanceUpdatedEvent.Invoke(unitPerformanceLastBattle);

    }

    private void BattleEnded()
	{
		inCombat = false;
		unitPerformanceAllTime += unitPerformanceLastBattle;
	}

	/// <summary>
	/// called by other units in order to deal damage to THIS unit
	/// </summary>
	/// <param name="dmg"></param>
	/// <param name="crit"></param>
	/// <param name="blocked"></param>
	public void TakeDamage(DamageInfo dmg)
	{ 
        health.TakeDamage(dmg);
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
		unitAttacker.AttackTarget();
	}

	private void OnUnitAttacked()
	{
		unitAnimator.Attacked();
	}

	private void OnDeath()
	{
		health.unitDied?.Invoke(gameObject);
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
