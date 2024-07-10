using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionReset : MonoBehaviour
{
	private MouseUtils mouseUtils;

	private void Awake()
	{
		mouseUtils = MouseUtils.singleton;
	}

	private void OnEnable()
	{
		mouseUtils.SetToDefaultCursor();
	}
}
