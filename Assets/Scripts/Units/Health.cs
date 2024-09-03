using UnityEngine;
using UnityEngine.Events;
using glumpis.CharacterStats;
using Assets.Scripts.Units;
using UnityEditor;
using Unity.VisualScripting.FullSerializer.Internal.Converters;

public class Health : MonoBehaviour
{
	public CharacterStat maxHealth;
	public float _health;
	public float blockChance;
	public Health OwnerHealth;
	public GameManager gameManager;

    public UnityEvent blockedEvent;
    public UnityEvent attackedEvent;

	public bool isDead;
	public UnityEvent died = new();
	public UnityEvent<GameObject> unitDied = new();
	public UnityEvent<float, float> healthChanged;


    public float health
    {
        get => _health;
        set
        {
            float previousHealth = _health;
            _health = value;
            healthChanged.Invoke(previousHealth, value);
        }
    }

    private void Start()
	{
		Revive();
	}

    private void Awake()
    {
		gameManager = GameManager.singleton;
    }

    public void TakeDamage(DamageInfo dmg)
	{
		if(OwnerHealth && OwnerHealth.isDead)
		{
			return;
		}

		if (health <= 0 || isDead )
		{
			if (OwnerHealth)
			{
				OwnerHealth.TakeDamage(dmg);
			}
			return;
		}

		//proccess incoming damage info
        DamageReport damageReport = new DamageReport();
        damageReport.damageInfo = dmg;
        damageReport.victim = gameObject;

        damageReport.healthBeforeDamage = health;
		damageReport.incomingDamage = dmg.isCrit ? dmg.damage * dmg.critMultiplier : dmg.damage;

		float roll = Random.value;
        damageReport.wasBlocked = (roll <= blockChance);
		if (damageReport.wasBlocked)
		{
			damageReport.damageDealt = 0;
            blockedEvent.Invoke();
        }
        else
		{
			damageReport.damageDealt = damageReport.incomingDamage;
            health -= damageReport.damageDealt; //-------------------------------------------- DAMAGE ACTUALLY DEALT HERE
			
            if ((int)health <= 0)
            {
                health = 0;
                isDead = true;
                Die();
            }
            damageReport.wasLethal = isDead;


            attackedEvent.Invoke();
        }
        damageReport.healthAfterDamage = health;

		damageReport.damageRemainder = Mathf.Abs(damageReport.healthBeforeDamage - damageReport.damageDealt);

        // broadcast damage report
		DamageSystem.singleton.OnAnyHealthAttacked.Invoke(damageReport);

		//determine if damage should overflow to owner
		if (gameManager.debugMenu.overkillEnabled && OwnerHealth && isDead && damageReport.damageRemainder > 0)
		{
			DamageInfo overflowDamage = dmg;
			overflowDamage.damage = damageReport.damageRemainder;
            overflowDamage.isCrit = false;
			overflowDamage.isOverflow = true;
            overflowDamage.critMultiplier = 0;
            //Debug.Log($"Calculating overflow for {damageReport.damageInfo.attacker.name} => {damageReport.victim.name} :: Remainder = {overflowDamage.damage}");
            OwnerHealth.TakeDamage(overflowDamage);
		}



	}

    public void InitHealth(int health)
	{
		maxHealth = new CharacterStat(health, 1);
	}

	public void Revive()
	{
		isDead = false;
		health = maxHealth;
	}

	private void Die()
	{
		died.Invoke();
	}
}
