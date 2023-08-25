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
    public Material blockParticle;
    public Material critParticle;
    private ParticleSystem particleSys;
    private ParticleSystemRenderer particleRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        particleSys = GetComponent<ParticleSystem>();
        particleRenderer = particleSys.GetComponent<ParticleSystemRenderer>();
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

    public void PlayCrit()
    {
        particleRenderer.material = critParticle;
        particleSys.Play();
    }

    public void PlayBlock()
    {
        particleRenderer.material = blockParticle;
        particleSys.Play();
    }
}
