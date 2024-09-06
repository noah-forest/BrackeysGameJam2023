using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{
	public UnitMasterComponent unitMaster;
	public Image progressImage;

	private UnitAnimator unitAnimator;
	private UnitController unitController;
	private Health unitHealth;
	private GameManager gameManager;
	
	private float currentTimer;
	private float maxTimer;

	private bool timerActive;
	
	private void Start()
	{
		maxTimer = unitMaster.unitStats.attackSpeed;
		unitAnimator = unitMaster.unitAnimatorScript;
		unitController = unitMaster.unitController;
		unitHealth = unitMaster.unitHealth;
		gameManager = GameManager.singleton;
		
		StartTimer();
		
		unitHealth.died.AddListener(ResetBar);
		gameManager.battleStartedEvent.AddListener(StartTimer);
		unitController.unitRespawnEvent.AddListener(RestartOnSpawn);
		gameManager.battleEndedEvent.AddListener(ResetBar);
		unitAnimator.attackEndEvent.AddListener(StartTimer);
	}

	private void Update()
	{
		if (!timerActive) return;
		
		UpdateBar();
	}

	private void UpdateBar()
	{
		currentTimer += Time.deltaTime;
		progressImage.fillAmount = currentTimer / maxTimer;

		if (currentTimer >= maxTimer)
		{
			ResetBar();
		}
	}

	private void StartTimer()
	{
		timerActive = true;
	}

	private void RestartOnSpawn(UnitController controller)
	{
		timerActive = true;
	}

	private void ResetBar()
	{
		timerActive = false;
		currentTimer = 0;
		progressImage.fillAmount = 0;
	}
}
