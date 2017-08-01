using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
	Asteroid.cs
	Applied on all asteroid-type objects. Controls health/downsizing/destroying
*/

[RequireComponent (typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour, IKillable, ITakeDamage<float>
{
	
	// Things we need to access
	public SpaceBlamSettings settings;
	public int size = 3;
	
	// Some useful components
	ScoreControl scoreCtrl;
	Rigidbody2D rb;
	ScreenTilingCtrl tilingCtrl;

	// Used to store values pulled from the settings
	float health;
	float damagePerHit;

	void Start ()
	{
		health = size == 1 ? settings.asteroidSmallHealth : size == 2 ? settings.asteroidMediumHealth : settings.asteroidLargeHealth;
		damagePerHit = settings.damagerPerHit;
		GameObject gameController = GameObject.FindGameObjectWithTag ("GameController");
		tilingCtrl = gameController.GetComponent<ScreenTilingCtrl> ();
		scoreCtrl = gameController.GetComponent<ScoreControl> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	// Incase we get well out of bounds, Destroy this and we'll spawn another one
	// This is more to handle the case where an un-tiled asteroid that has been
	// instantiated to keep the numbers of asteroids up gets bumped our of bounds
	// before we can tile it safely
	void FixedUpdate ()
	{
		if(tag == "IncomingAsteroid")
		{
			if (Vector2.Distance (rb.position, Vector2.zero) > 20f) {
				// Kill() only works if we're tiled, so call Destroy in case
				Kill ();
				Destroy (gameObject);
			}
		}
	}

	// Controls behaviour if we get hit by lasers or a player
	void OnCollisionEnter2D (Collision2D collision)
	{
		string tag = collision.collider.tag;
		if (tag == "Laser") {
			Destroy (collision.collider.gameObject);
			Damage (damagePerHit);
			if (health <= 0f) {
				DownSize (collision.contacts [0].normal);
			}
		} else if (tag == "Player") {
			Death death = collision.collider.gameObject.GetComponent<Death> ();
			if (death) {
				death.Kill ();
			} else {
				Debug.LogError ("Death script missing on player");
			}
		}
	}
	
	// If we get hit, do we die, or shrink down?
	void DownSize (Vector3 normalHit)
	{
		// Play sound
		GameObject audioPlayerObject = Instantiate (settings.audioPlayerPrefab, transform.position, Quaternion.identity) as GameObject;
		AudioPlayer aPlayer = audioPlayerObject.GetComponent<AudioPlayer> ();
		if (aPlayer) {
			aPlayer.aSource.clip = settings.asteroidShrink;
		}
		
		if (size <= 1) {
			scoreCtrl.AddPoints (settings.pointsPerSmallDestroyed);
			Kill ();
			Destroy (gameObject);
		} else {
			if (size == 2) {
				scoreCtrl.AddPoints (settings.pointsPerMediumDestroyed);
			} else if (size == 3) {
				scoreCtrl.AddPoints (settings.pointsPerSmallDestroyed);
			}
			size--;
			GameObject smallerAsteroid = size == 1 ? settings.smallAsteroid : size == 2 ? settings.mediumAsteroid : settings.largeAsteroid;
			GameObject spawned;
			float offset = smallerAsteroid.GetComponent<Collider2D> ().bounds.size.magnitude;
			for (int i = 0; i < settings.smallerAsteroidsSpawnedOnKill; i++) {
				Quaternion rot = Quaternion.FromToRotation (Vector3.up, normalHit);
				Vector3 direction = rot * normalHit;//Quaternion.AngleAxis( i == 0 ? 90f : -90f, normalHit) * normalHit;
				spawned = Instantiate (smallerAsteroid, transform.position + (2 * offset * (i == 0 ? direction : -direction)), Quaternion.identity) as GameObject;
				tilingCtrl.TileAnObject (spawned);
				spawned.GetComponent<Rigidbody2D> ().AddForce (settings.asteroidSpawnForce * (i == 0 ? direction : -direction));
			}
			Kill ();

			// Shouldn't be necessary, this object should be destroyed in the Kill() function before it gets here.
			// Just for my own sanity
			Destroy (gameObject);
		}
	}

	// Implements IKillable
	public void Kill ()
	{
		tilingCtrl.DestroyATiledObject (gameObject);
	}
		
	// Implements ITakeDamage<T>
	// We take a value, rather than read the damagePerHit directly
	// because I may tweak damagePerHit to do more complex things later

	// Keep track of cloned asteroids for quick adjustment of values later
	List<Asteroid> siblings = new List<Asteroid> ();

	public void Damage (float damageAmount)
	{
		
		// Play sound
		// Instantiate a new object as this one gets destroyed
		GameObject audioPlayerObject = Instantiate (settings.audioPlayerPrefab, transform.position, Quaternion.identity) as GameObject;
		AudioPlayer aPlayer = audioPlayerObject.GetComponent<AudioPlayer> ();
		if (aPlayer) {
			aPlayer.aSource.clip = settings.shotHit;
		}

		// Only need to do this once, if an asteroid is damaged
		// grabs all tiled copies of this asteroid
		if (siblings.Count < 9) {
			siblings = new List<Asteroid> ();
			Transform parent = transform.parent;
			int siblingCount = parent.childCount;
			for (int i = 0; i < siblingCount; i++) {
				GameObject sibling = parent.GetChild (i).gameObject;
				siblings.Add (sibling.GetComponent<Asteroid> ());
			}
		}
		// Notify all instances of this asteroid
		// TODO: Put this into a script on the parent, so we only have one instance to care about
		foreach (Asteroid asteroid in siblings) {
			asteroid.health -= damageAmount;
		}
		scoreCtrl.AddPoints (settings.pointsPerHit);
	}
}

