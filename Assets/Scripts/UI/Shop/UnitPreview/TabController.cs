using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabController : MonoBehaviour
{
	private bool shopOpen;

	private Animator anim;
	private static readonly int TabHover = Animator.StringToHash("tab_hover");
	private static readonly int TabHoverExit = Animator.StringToHash("tab_no_hover");

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	///<summary>
	///Play shop opening animation
	/// </summary>
	public void OpenShop()
	{
		anim.ResetTrigger(TabHoverExit);
		anim.SetTrigger(TabHover);
	}

	public void CloseShop()
	{
		anim.ResetTrigger(TabHover);
		anim.SetTrigger(TabHoverExit);
	}
}
