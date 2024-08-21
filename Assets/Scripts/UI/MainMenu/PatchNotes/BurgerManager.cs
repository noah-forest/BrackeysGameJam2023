using UnityEngine;
using UnityEngine.UI;

public class BurgerManager : MonoBehaviour
{
	public Button burger;

	public GameObject history;

	private Animator anim;
	public bool burgerOpen;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		anim.keepAnimatorStateOnDisable = true;
	}

	private void Start()
	{
		burger.onClick.AddListener(burgerOnClick);
	}

	private void OnDisable()
	{
		if (burgerOpen)
		{
			anim.ResetTrigger("opened");
			anim.ResetTrigger("closed");
			anim.Rebind();
			anim.Update(0f);
			burgerOnClick();
		}
	}

	private void burgerOnClick()
	{
		if (burgerOpen)
		{
			CloseHistory();
		}
		else
		{
			OpenHistory();
		}
	}

	public void OpenHistory()
	{
		burgerOpen = true;
		anim.SetTrigger("opened");
	}

	public void CloseHistory()
	{
		burgerOpen = false;
		anim.SetTrigger("closed");
	}

	public void ShowHistory()
	{
		history.SetActive(true);
	}

	public void HideHistory()
	{
		history.SetActive(false);
	}

	public void priorityButtonPressed()
	{
		if (burgerOpen)
		{
			burgerOnClick();
		}
	}
}
