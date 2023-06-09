using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IHasAbility
{
	//DO NOT CHANGE: PlayerStats Functionality depends on this
	private Currency _currency;
	private Lives _lives;
	private bool died;

	//DO NOT CHANGE: Enemy Pathing AI functionality
	private Transform _target;
	private int _wavepointIndex = 0;

	[Header("Enemy Attributes")]
	public float speed = 10f;

	public float maxHealth = 100f;

	public int worth = 1; // Gold Dropped upon kill

	public int lifeCost = 1; // Lives deducted upon exit

	private protected float Health;

	public float damageMultiplier = 1;

	private bool isSlowed = false;
	private float slowness = 1;

	// Setup for subscribers to this death (extra notes in the class)
	public Action<Enemy> OnDeath;
	
	//internal Action<Enemy> effect;
		
	[Header("Ability usage parameters")]	
	public float timeBetweenAbility = 3.0f;

	public float countDown = 5.0f;

	[Header("Unity Required")] 
	public Image healthBar;

	
	public void Start()
	{
		// PlayerStats
		_currency = Currency.currencyManager;
		_lives = Lives.LifeManager;
		
		// Enemy AI
		_target = Waypoints.points[0];
		
		// Enemy Health Setup
		Health = maxHealth;
	}

	void Update()
	{
		if (Health <= 0)
		{
			Die();
		}
		// Enemy Pathing AI 
		Vector3 dir = _target.position - transform.position;
		if (isSlowed)
		{
            transform.Translate(dir.normalized * (speed * slowness * Time.deltaTime));
        } 
		else
		{
            transform.Translate(dir.normalized * (speed * Time.deltaTime));
        }
		

		if (Vector3.Distance(transform.position, _target.position) <= 1f) {
			GetNextWaypoint();
		}

		// Ability Usage Timing
		if (countDown <= 0)
		{
			UseAbility();
			countDown = timeBetweenAbility;
		}
		else
		{
			countDown -= Time.deltaTime;
		}
	}

	public virtual void TakeDamage(float amount)
	{
		Health -= amount * damageMultiplier;
		healthBar.fillAmount = Health / maxHealth;
	}

	public void slowDown(float slowness)
	{
		if (slowness >= 1)
		{
			Debug.Log("Incorrect setting of Slowness in SlowTurret");
		}

		this.slowness = slowness;
		this.isSlowed = true;
	}

	public void normalSpeed()
	{
		this.isSlowed = false;
	}

	private void GetNextWaypoint() {
		if (_wavepointIndex >= Waypoints.points.Length - 1) {
			Destroy(gameObject);
			_lives.drain(lifeCost);
			return;
		}
		_target = Waypoints.points[++_wavepointIndex];
	}

	private protected void Die()
	{
		Destroy(gameObject); 
		OnDeath?.Invoke(this);
		_currency.Gain(worth);
	}

	
	public virtual void UseAbility()
	{
		// Do nothing
	}
	

	public void TakeEffectFromGear(Action<Enemy> effect)
	{
		// Okay so we want enemies to take cumulative effects, BUT we don't want repeat effects to stack
		OnDeath += effect;
	}

	private void doNothing(Enemy enemy)
	{
		
	}
}
