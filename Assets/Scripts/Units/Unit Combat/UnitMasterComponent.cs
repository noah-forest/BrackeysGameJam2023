using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMasterComponent : MonoBehaviour
{
	[Header("Important")]
	public Health unitHealth;
	public SpriteRenderer unitSprite;
	public UnitStats unitStats;

	[Space(10)]
	[Header("Less Important")]
	public UnitAttacker unitAttacker;
	public UnitHealthBar unitHealthBar;
	public UnitAnimator unitAnimatorScript;
	public UnitController unitController;
	public Experience unitExperience;

	[Space(10)]
	[Header("Your mother")]
	public RadialProgress radialProgress;
	public Animator unitAnimator;
	public Canvas healthBar;
	public ParticleSystem unitParticleSystem;
	public AudioSource unitAudioSource;
}
