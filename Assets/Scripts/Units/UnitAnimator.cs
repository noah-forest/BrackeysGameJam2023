using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class UnitAnimator : MonoBehaviour
{
    private Animator animator;
    public UnityEvent attackHitEvent;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void AttackHitEvent()
    {
        attackHitEvent.Invoke();
    }

    public void Attacked()
    {
        animator.SetTrigger("Damaged");
    }
}
