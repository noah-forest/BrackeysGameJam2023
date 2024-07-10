using UnityEngine;

public class Glow : MonoBehaviour
{
	public GameObject glow;

	private void Start()
	{
		Hide();
	}

	public void Show()
	{
		glow.SetActive(true);
	}

	public void Hide()
	{
		glow.SetActive(false);
	}
}
