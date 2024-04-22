using System.Collections;
using UnityEngine;

public class SlideOutElement : MonoBehaviour
{
	private Animator anim;

	private void Awake()
	{
		anim = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		if (gameObject.name == "shop" || gameObject.name == "playerInfo" || gameObject.name == "unitInventory")
		{
			StartCoroutine(WaitForTransition());
		}
		else
		{
			anim.SetTrigger("slideOut");
		}
	}

	private void OnDisable()
	{
		anim.ResetTrigger("disabled");
	}

	private IEnumerator WaitForTransition()
	{
		yield return new WaitForSeconds(0.2f);
		anim.SetTrigger("slideOut");
	}
}
