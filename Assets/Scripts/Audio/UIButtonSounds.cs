using UnityEngine;
using System.Collections;
/*
	UIButtonSounds.cs
	Currently just for non-scene-changing buttons
	Buttons that load a new scene have their own script so the sond plays before scene change.
	BUT the sound clips are from the same place (SpaceBlamSettings) and so are consistent
*/


[RequireComponent (typeof(AudioSource))]
public class UIButtonSounds : MonoBehaviour
{
	// Get the sounds we want
	public SpaceBlamSettings settings;

	// Create this on Start
	AudioSource aSource;

	void Start ()
	{
		aSource = GetComponent<AudioSource> ();
		aSource.clip = settings.uIButtonForward;
	}

	// Avaiable to anyone who wants it
	public void PlayForwardSounds ()
	{
		aSource.Play ();
	}
}
