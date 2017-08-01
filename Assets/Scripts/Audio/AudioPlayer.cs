using UnityEngine;
using System.Collections;
/*
	AudioPlayer.cs
	This will play an audio clip on its AudioSource, then destroy itself (the gameobject, that is)
*/


[RequireComponent (typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{

	public AudioSource aSource;

	void Awake ()
	{
		aSource = GetComponent<AudioSource> ();

	}

	void Start ()
	{
		aSource.Play ();
		StartCoroutine (DestroyWhenFinshed ());
	}

	IEnumerator DestroyWhenFinshed ()
	{
		while (aSource.isPlaying)
		{
			yield return null;
		}
		Destroy (gameObject);
	}

}