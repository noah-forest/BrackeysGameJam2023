using System;
using System.Collections;
using System.Collections.Generic;
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

    public Actor  parentActor;
    public UnityEvent attacked;

    private Grave _grave;
    /// <summary>
    /// This is a property that will handle assigning the graveDug event whenever you assign a new grave to a unit
    /// </summary>
    public Grave unitGrave
    {
        set
        {
            value.graveDug.RemoveListener(Respawn); //this may be not needed, and may also cause problems later, and also may not even work.
            _grave = value;
            _grave.graveDug.AddListener(Respawn);
        }
        get => _grave;
    }

    private GameManager gameManager;
    private bool isAttacking;

    private void Awake()
    {
        gameManager = GameManager.singleton;

        health = GetComponent<Health>();
        unitAttacker = GetComponent<UnitAttacker>();
        unitStats = GetComponent<UnitStats>();
        unitAnimator = GetComponent<UnitAnimator>();
        StartCoroutine(AttackLoop());
        unitAnimator.attackHitEvent.AddListener(DamageEnemyUnit);
        attacked.AddListener(onUnitAttacked);

        health.died.AddListener(OnDeath);
    }

    private void Update()
    {
        if (health.isDead) return;	
        if (isAttacking) return;
        if (!unitAttacker.targetUnit) return;
        StartCoroutine(AttackLoop());
    }

    /// <summary>
    /// called by other units in order to deal damage to THIS unit
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="crit"></param>
    /// <param name="blocked"></param>
    public void TakeDamage(float dmg)
    {
        // if the unit is dead when it would normally take damage, damage this units actor
        if (health.isDead)
        {
            parentActor.health.TakeDamage(dmg);
            return;
        }

        //check if the unit blocks the incoming damage
        bool blocked = false;
        float blockRoll = Random.value;
        if (blockRoll < unitStats.blockChance)
        {
            // blocked hit
            blocked = true;
        } // this is seperated out from the follwing 'damage the unit if' as other behavior may rely on block status.

        // damage the unit
        if (!blocked)
        {
            health.TakeDamage(dmg);
            attacked.Invoke();
        };
    }

    private IEnumerator AttackLoop()
    {
        isAttacking = true;
        while (!health.isDead && !gameManager.resolved)
        {
            yield return new WaitForSeconds(unitStats.attackInterval);
            StartAttack();
        }

        isAttacking = false; // unit is dead
        yield return null;
    }

    private void StartAttack()
    {
        unitAnimator.Attack();
    }

    private void DamageEnemyUnit()
    {
        unitAttacker.AttackTarget();
    }

    private void onUnitAttacked()
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

    private void Respawn()
    {
        health.Revive();
        isAttacking = false;
        for(int i = 0; i < transform.childCount; i++)
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
