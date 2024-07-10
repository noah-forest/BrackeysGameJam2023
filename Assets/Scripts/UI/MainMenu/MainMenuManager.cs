using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
	public List<GameObject> menus = new();

	//[HideInInspector]
	public List<bool> bools = new();

    // Start is called before the first frame update
    void Start()
    {
		for (int i = 0; i < menus.Count; i++)
		{ 
			bool menuOpen = false;
			bools.Add(menuOpen);
		}
    }

	public void SetMenuOpen(GameObject menu)
	{
		SetMenuClosed(menu);
		for(int i = 0; i < menus.Count;i++)
		{
			if (menus[i].activeInHierarchy)
			{
				bools[i] = true;
			}
		}
	}

	public void SetMenuClosed(GameObject menu)
	{
		for(int i = 0; i < menus.Count; ++i)
		{
			if (menus[i] != menu && menus[i].activeInHierarchy)
			{
				OpenSideMenu sideMenu = menus[i].GetComponent<OpenSideMenu>();
				sideMenu.OnClick();
				bools[i] = false;
			}
		}
	}

	public void CloseMenus()
	{
		for (int i = 0; i < menus.Count; ++i)
		{
			if (menus[i].activeInHierarchy)
			{
				OpenSideMenu sideMenu = menus[i].GetComponent<OpenSideMenu>();
				sideMenu.OnClick();
				bools[i] = false;
			}
		}
	}
}
