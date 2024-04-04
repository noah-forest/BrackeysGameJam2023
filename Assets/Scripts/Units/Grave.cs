using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider2D))]
public class Grave : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public bool playerOwned;
	private ParticleSystem digParticle;
	public UnityEvent graveDug;
	private bool inGrave;
	private int currentDigCount;

	private MouseUtils mouseUtils;
	private GameManager gameManager;

	private int digCount;
	private float digSpeed = 0.40f; //used by enemy 
	public Sprite[] digLevelSprites;
	private SpriteRenderer sprite;
	AudioSource audSource;
	public AudioClip digSound;
	public AudioClip finalDigSound;

	public GameObject graveTutorial;

	private void Start()
	{
		gameManager = GameManager.singleton;
		digParticle = GetComponent<ParticleSystem>();
		audSource = GetComponent<AudioSource>();
		sprite = GetComponent<SpriteRenderer>();
		mouseUtils = MouseUtils.singleton;
		gameManager.battleStartedEvent.AddListener(ResetGrave);
		graveTutorial.SetActive(false);
	}

	public void ResetGrave()
	{
		digCount = 0;
		currentDigCount = 0;
		graveTutorial.SetActive(false);
		sprite.sprite = digLevelSprites[0];
	}

	public void ActivateGrave(int digCount)
	{
		this.digCount = digCount;
		inGrave = true;
		currentDigCount = 0;
		if (!playerOwned)
		{
			StartCoroutine(EnemyDig());
		} else
		{
			graveTutorial.SetActive(true);
		}
		sprite.sprite = digLevelSprites[digLevelSprites.Length - 1];
	}

	public void Dig()
	{
		if (!inGrave || gameManager.gameIsPaused) return;
		currentDigCount++;

		int spriteInterp = (int)Mathf.Lerp(digLevelSprites.Length, 1, (float)(currentDigCount) / (float)digCount);

		if (currentDigCount >= digCount)
		{
			inGrave = false;
			graveTutorial.SetActive(false);
			spriteInterp = 0;
			graveDug.Invoke();
		}
		digParticle.Play();
		sprite.sprite = digLevelSprites[spriteInterp];
		PlayDigSound();
	}

	void PlayDigSound()
	{
		if (digCount == currentDigCount)
		{
			audSource.volume = playerOwned ? 0.3f : 0.05f;
			audSource.pitch = 1.5f;
			audSource.clip = finalDigSound;
		}
		else
		{
			audSource.volume = playerOwned ? 1 : 0.35f;
			audSource.clip = digSound;
			audSource.pitch = 1;
			audSource.pitch -= 0.03f * currentDigCount;
		}
		audSource.pitch += Random.Range(-0.01f, 0.01f);
		audSource.Play();

	}

	void OnMouseDown() // when collider is clicked by player
	{
		if (playerOwned) Dig(); // enemies cannot be dug
	}

	/// <summary>
	/// Enemy graves return units to life on cooldown instead of being clicked.
	/// Technically, it clicks the grave on intervals so the enemy. 
	/// </summary>
	/// <returns></returns>
	IEnumerator EnemyDig()
	{
		yield return new WaitForSeconds(digSpeed);
		Dig();
		if (inGrave == true) StartCoroutine(EnemyDig()); //loops until the dig function sets inGrave to false
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseUtils.SetHoverCursor();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseUtils.SetToDefaultCursor();
	}
}
