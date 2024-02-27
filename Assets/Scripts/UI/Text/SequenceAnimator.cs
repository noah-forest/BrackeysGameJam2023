using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceAnimator : MonoBehaviour
{
	private List<Animator> animators;

	public bool DoAnim = false;

	public float animDuration = 0.06f;
	public float animCooldown = 4f;

	private void Awake()
	{
		animators = new List<Animator>(GetComponentsInChildren<Animator>());
	}

	private void OnEnable()
	{
		DoAnim = true;
		StartTextAnim();
	}

	private void OnDisable()
	{
		DoAnim = false;
	}

	public void StartTextAnim()
	{
		StartCoroutine(DoAnimation(animDuration, animCooldown));
	}

	IEnumerator DoAnimation(float animDuration, float animCooldown)
	{
		while (DoAnim)
		{
			foreach (Animator animator in animators)
			{
				animator.SetTrigger("DoAnimation");
				yield return new WaitForSeconds(animDuration);
			}

			yield return new WaitForSeconds(animCooldown);
		}
	}
}
