using System.Collections;
using System.Collections.Generic;
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
		anim.SetTrigger("slideOut");
	}

	private void OnDisable()
	{
		anim.SetTrigger("disabled");
	}
}
