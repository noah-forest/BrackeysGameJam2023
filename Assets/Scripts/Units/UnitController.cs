using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(UnitAnimator))]
[RequireComponent(typeof(UnitAttacker))]
[RequireComponent(typeof(UnitStats))]
public class UnitController : MonoBehaviour
{
    Health health;
    UnitAttacker unitAttacker;
    UnitAnimator unitAnimator;
    UnitStats unitStats;

    public Grave grave;

    private GameManager gameManager;
    private bool isAttacking;

    private void Start()
    {
        gameManager = GameManager.singleton;

        health = GetComponent<Health>();
        unitAttacker = GetComponent<UnitAttacker>();
        unitStats = GetComponent<UnitStats>();
        unitAnimator = GetComponent<UnitAnimator>();
        StartCoroutine(AttackLoop());
        unitAnimator.attackHitEvent.AddListener(DamageEnemyUnit);
        unitAttacker.attacked.AddListener(onUnitAttacked);
        
        health.died.AddListener(OnDied);
        if (grave)
        {
            grave.graveDug.AddListener(Respawn);
            health.died.AddListener(OnDeath);
        }
    }

    private void Update()
    {
        if (health.isDead) return;	
        if (isAttacking) return;
        StartCoroutine(AttackLoop());
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
        if (grave)
        {
            grave.ActivateGrave(unitStats.digCount);
        }
    }

    private void Respawn()
    {
        health.Revive();
        unitAttacker.Respawn();
        Debug.Log("Respawned");
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject Go = transform.GetChild(i).gameObject;
            Go.SetActive(true);
        }
    }

    private void OnDied()
    {
        // go to grave
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject Go = transform.GetChild(i).gameObject;
            Go.SetActive(false);
        }
    }

}
