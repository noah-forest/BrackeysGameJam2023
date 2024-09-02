using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Events;

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
	public UnitPerformance unitPerformanceAllTime;
    [HideInInspector]
    public UnitPerformance unitPerformanceLastBattle;

    public UnityEvent<UnitPerformance> performanceUpdatedEvent;
	public Actor parentActor;

	private Grave _grave;
	public UnityEvent<UnitController> unitRespawnEvent;
	/// <summary>
	/// This is a property that will handle assigning the graveDug event whenever you assign a new grave to a unit
	/// </summary>
	public Grave unitGrave
	{
		set
		{
			if (_grave != null)
			{
				_grave.graveFinished.RemoveListener(Respawn);
                _grave.graveClicked.RemoveListener(OnGraveClicked);

            }

            _grave = value;
			_grave.graveFinished.AddListener(Respawn);
			_grave.graveClicked.AddListener(OnGraveClicked);
		}
		get => _grave;
	}

	private GameManager gameManager;
	private BattleManager battleManager;
	private bool isAttacking;
	public bool InCombat {  get; private set; }
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
		InCombat = true;
		gameManager.combatBeganEvent.Invoke();
	}

	private void FixedUpdate()
	{
		if (!InCombat) return;

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
		InCombat = true;
        unitPerformanceLastBattle = new();
		performanceUpdatedEvent.Invoke(unitPerformanceLastBattle);

    }

    private void BattleEnded()
	{
		InCombat = false;
		unitPerformanceAllTime += unitPerformanceLastBattle;
	}

	public void InitCombat()
	{
		SetAttackTime();
		health.blockChance = unitStats.blockChance;
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
		unitRespawnEvent.Invoke(this);
		SetAttackTime();
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject Go = transform.GetChild(i).gameObject;
			Go.SetActive(true);
		}
	}

	public void OnGraveClicked()
	{
		++unitPerformanceLastBattle.timesDug;
	}

	public Sprite GetSlotSprite()
	{

		GameObject sprite = transform.Find("Sprite").gameObject;
		return sprite.GetComponent<SpriteRenderer>().sprite;
	}
}
