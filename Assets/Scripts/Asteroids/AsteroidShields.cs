using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
	AsteroidSheild.cs
	Controls the shields on asteroids and the laser interaction on them
*/


[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class AsteroidShields : MonoBehaviour
{

	// Some useful components, made public as we could always change
	// them specifically to create more varied asteroids
	public SpaceBlamSettings settings;
	public SpriteRenderer visuals;
	public Asteroid parentScript;
	public bool isEnabled = true;
	public float initialDelay = 1f;
	
	// Some useful components we'll make use of that we don't need to share
	GameObject pauseUI;
	Rigidbody2D myRb;
	Collider2D myCol;
	AudioSource aSource;

	// Store some coroutines for cancelling/starting
	Coroutine shieldRunner = null;
	Coroutine shieldDelay = null;

	// Used to store shield on/off switch speed
	float timeInterval;

	// These values are used for pause sycning.
	float timeRun = 0f;
	float newTimerDelayFromPausing = 0f;
	bool stoppedOnPause = false;

	// Set things up immediately
	void Awake ()
	{
		myCol = GetComponent<Collider2D> ();
		myRb = GetComponent<Rigidbody2D> ();
		if (!parentScript)
		{
			Debug.LogError ("Missing parentScript <Asteroid>");
		}
		else
		{
			timeInterval = parentScript.size == 1 ? settings.asteroidSmallShieldUpInterval : 
			parentScript.size == 2 ? settings.asteroidMediumShieldUpInterval :
			settings.asteroidLargeShieldUpInterval;
		}
		shieldDelay = StartCoroutine (StartDelay ());
		aSource = gameObject.AddComponent<AudioSource> ();
		aSource.playOnAwake = false;
		aSource.clip = settings.shotBounce;
	}

	// Used for tracking pause states of the game
	void Update ()
	{
		// Is the game paused?
		if (GameState.currentState == GameState.AvailableStates.Paused)
		{
			stoppedOnPause = true;
			newTimerDelayFromPausing = timeInterval - timeRun;
			if (shieldRunner != null)
			{
				StopCoroutine (shieldRunner);
			}
		}
		else if (stoppedOnPause)
		{
			// Un-paused
			stoppedOnPause = false;
			shieldRunner = StartCoroutine (EnableDisableTimer (newTimerDelayFromPausing));
		}
		else
		{
			// iterate every frame to find out how long our shield has been active
			timeRun += Time.deltaTime;
		}
	}

	// Only really used by the first 3 large asteroids, just to make some variation in the original 3
	IEnumerator StartDelay (float overrideTimer = 0f)
	{
		yield return new WaitForSeconds (overrideTimer != 0f ? overrideTimer : initialDelay);
		shieldRunner = StartCoroutine (EnableDisableTimer ());
	}
	
	// Called externally to sync up shields
	public void SyncSheilds ()
	{
		isEnabled = true;
		StopAllCoroutines ();
		shieldDelay = StartCoroutine (StartDelay ());
	}

	// overrideTimer is used after the game has been unpaused, otherwise we apply the standard delay
	IEnumerator EnableDisableTimer (float overrideTimer = 0f)
	{
		if (isEnabled) 
		{
			visuals.enabled = false;
			myCol.enabled = false;
		}
		else
		{
			visuals.enabled = true;
			myCol.enabled = true;
		}

		yield return new WaitForSeconds (overrideTimer != 0f ? overrideTimer : timeInterval);
		// if paused, stop here so we don't call the Coroutine again and lose the reference to this one
		if (GameState.currentState == GameState.AvailableStates.Paused)
		{
			yield return 0;
		}
		timeRun = 0f;
		isEnabled = !isEnabled;
		shieldRunner = StartCoroutine (EnableDisableTimer ());
	}

	// If theres a stray bullet within the shield, we give it a gentle push out of the field
	// subtle, but nice effect (IMO)
	void OnTriggerStay2D (Collider2D pullMe)
	{
		if (pullMe.tag != "Laser")
		{
			return;
		}

		Rigidbody2D rb = pullMe.GetComponent<Rigidbody2D> ();
		if (rb == null)
		{
			return;
		}

		float force = (settings.shieldResistance * rb.mass * myRb.mass);
		Vector2 direction = rb.worldCenterOfMass - myRb.worldCenterOfMass;
		rb.AddForce (force * direction);
	}

	// Used to play hit sounds on shields
	void OnTriggerEnter2D (Collider2D pullMe)
	{
		if (pullMe.tag != "Laser") 
		{
			return;
		}

		Rigidbody2D rb = pullMe.GetComponent<Rigidbody2D> ();
		if (rb == null) 
		{
			return;
		}

		rb.velocity = Quaternion.AngleAxis (Random.Range (160f, 200f), new Vector3 (0f, 0f, 1f)) * rb.velocity;
		if (aSource) 
		{
			aSource.Play ();
		}
	}
}

