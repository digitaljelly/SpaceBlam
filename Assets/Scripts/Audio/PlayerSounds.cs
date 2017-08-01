using UnityEngine;
using System.Collections;
/*
	PlayerSounds.cs
	Controls the player-specific sounds
	- Note these can be called externall as well
*/

public class PlayerSounds : MonoBehaviour
{
	// Use this to get our desired sound effects
	public SpaceBlamSettings settings;

	// Need to be accessible to be played from elsewhere
	[HideInInspector]
	public AudioSource thrustersSource;
	[HideInInspector]
	public AudioSource gunSource;
	[HideInInspector]
	public AudioSource explosionSource;

	// Setup
	void Start ()
	{
		thrustersSource = gameObject.AddComponent<AudioSource> ();
		gunSource = gameObject.AddComponent<AudioSource> ();
		explosionSource = gameObject.AddComponent<AudioSource> ();

		thrustersSource.clip = settings.thruster;
		gunSource.clip = settings.fireShot;
		explosionSource.clip = settings.asteroidShrink;

		thrustersSource.loop = true;
		explosionSource.loop = false;

		thrustersSource.playOnAwake = false;
		gunSource.playOnAwake = false;
		explosionSource.playOnAwake = false;

	}

}
