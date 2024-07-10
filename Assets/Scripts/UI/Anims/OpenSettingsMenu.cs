using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSettingsMenu : MonoBehaviour
{
	public GameObject settingsMenu;

	private Animator anim;

	private bool settingsOpen = true;

	private void Start()
	{
		anim = settingsMenu.GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.F1))
		{
			settingsOpen = !settingsOpen;
			settingsMenu.SetActive(settingsOpen);
		}
	}
}
