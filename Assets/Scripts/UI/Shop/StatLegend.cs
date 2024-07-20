using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatLegend : MonoBehaviour
{
	public GameObject close;
	public GameObject statLegend;

	public bool open;

	private Animator slideAnim;

	private void Start()
	{
		open = false;
		slideAnim = statLegend.GetComponent<Animator>();
	}

	public void OnClick()
	{
		if (open)
		{

			CloseStatLegend();
		} else
		{
			OpenStatLegend();
		}
	}

	public void OpenStatLegend()
	{
		if (!open)
		{
			open = true;
			slideAnim.SetTrigger("show");
			close.SetActive(true);
		}
	}

	public void CloseStatLegend()
	{
		if (open)
		{
			open = false;
			slideAnim.SetTrigger("hide");
			close.SetActive(false);
		}
	}
}
