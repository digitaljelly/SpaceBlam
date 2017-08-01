using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/*
	BGMusicControl.cs
	Controls the BG music
*/


[RequireComponent (typeof(AudioSource))]
public class BGMusicControl : MonoBehaviour
{

	public SpaceBlamSettings settings;

	AudioSource aSource;

	void Awake ()
	{
		// Make this persistent, keeps everything easy to track and change
		DontDestroyOnLoad (gameObject);
		aSource = GetComponent<AudioSource> ();
		MainTheme ();
	}

	// Play the main theme
	public void MainTheme ()
	{
		if (aSource.clip != settings.mainTheme)
		{
			aSource.clip = settings.mainTheme;
			aSource.Play ();
		}
	}

	// Play the game theme
	public void GameTheme ()
	{
		if (aSource.clip != settings.gameTheme)
		{
			aSource.clip = settings.gameTheme;
			aSource.Play ();
		}
	}
	
	// Delegate that fires on level change, so we know what music to play and when
	void OnEnable ()
	{
		SceneManager.sceneLoaded += ExampleSceneHasLoaded;
	}

	void OnDisable ()
	{
		SceneManager.sceneLoaded -= ExampleSceneHasLoaded;
	}

	void ExampleSceneHasLoaded (Scene scene, LoadSceneMode mode)
	{
		string sceneName = scene.name;
		if (sceneName == "Main Game")
		{
			GameTheme ();
		} else
		{
			MainTheme ();
		}
	}
}