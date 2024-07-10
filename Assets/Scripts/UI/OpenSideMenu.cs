using UnityEngine;

public class OpenSideMenu : MonoBehaviour
{
	public GameObject menu;
	private bool open;


	public void OnClick()
	{
		if (open)
		{
			open = false;
			menu.SetActive(false);
		}
		else
		{
			menu.SetActive(true);
			open = true;
		}
	}

}
