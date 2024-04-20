using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideElement : MonoBehaviour
{
	private Animator anim;

	private void Awake()
	{
		anim = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		anim.SetTrigger("show");
	}
}
