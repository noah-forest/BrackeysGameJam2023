using UnityEngine;
using UnityEngine.UI;

public class LockShop : MonoBehaviour
{
	public GameObject openLock;
	public GameObject closedLock;
	public GameObject refreshLock;
	public GameObject refreshCost;

	public GameObject bars;
	public Animator barsAnim;
	public Image lockBorder;
	public Image foreground;

	public bool locked;

	private GameManager gameManager;

	private void Start()
	{
		gameManager = GameManager.singleton;

		Button button = GetComponent<Button>();
		button.onClick.AddListener(SetShopLocked);

		gameManager.startGame.AddListener(UnlockShop);
	}

	private void OnEnable()
	{
		if (locked)
		{
			LockShopItems();
		}
	}

	private void SetShopLocked()
	{
		if (!locked) LockShopItems();
		else UnlockShop();
	}

	private void LockShopItems()
	{
		locked = true;
		openLock.SetActive(false);
		refreshCost.SetActive(false);
		closedLock.SetActive(true);
		refreshLock.SetActive(true);
		bars.SetActive(true);
		barsAnim.SetTrigger("shopLocked");


		lockBorder.color = new Color32(183, 75, 72, 255);
		foreground.color = new Color32(135, 126, 92, 255);

		gameManager.shopLocked.Invoke();
	}

	private void UnlockShop()
	{
		locked = false;
		openLock.SetActive(true);
		refreshCost.SetActive(true);
		closedLock.SetActive(false);
		refreshLock.SetActive(false);
		bars.SetActive(false);

		lockBorder.color = new Color32(163, 143, 99, 255);
		foreground.color = new Color32(207, 199, 167, 255);

		gameManager.shopUnlocked.Invoke();
	}
}
