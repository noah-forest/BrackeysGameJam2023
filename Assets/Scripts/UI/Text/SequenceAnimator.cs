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

		foreach(var animator in animators)
		{
			animator.keepAnimatorStateOnDisable = true;
		}
	}

	private void OnEnable()
	{
		foreach(var animator in animators)
		{
			animator.ResetTrigger("DoAnimation");
			animator.Rebind();
			animator.Update(0f);
		}

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
				yield return new WaitForSecondsRealtime(animDuration);
			}

			yield return new WaitForSecondsRealtime(animCooldown);
		}
	}
}
