using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/*
	SceneControl.cs
	Used to load and change current scene and quit the game
*/


public class SceneControl : MonoBehaviour
{

	/* 
		COMMENTS:
		We could call DontDestroyOnLoad on this object, but we need these functions
		accessible in the Inspector for easy UI configuarion in each scene. 
		Instead, we'll just	dump it in each scene on the GameController as we only have 
		three scenes to worry about.
	*/

	public SpaceBlamSettings settings;
	SceneLoader sceneLoader;
	AudioSource audioSourceBtn;

	void Start ()
	{
		GameObject sC = GameObject.FindGameObjectWithTag ("SceneController");
		if (sC != null)
		{
			sceneLoader = sC.GetComponent<SceneLoader> ();
		}
		audioSourceBtn = gameObject.AddComponent<AudioSource> ();
		audioSourceBtn.clip = settings.uIButtonForward;
	}

	// Public functions to be called from the UI components
	public void Quit ()
	{
		StartCoroutine (WaitForSound ("Quit"));
	}

	public void GoToCustomize ()
	{
		StartCoroutine (WaitForSound ("Customize"));
	}

	public void GoToMainMenu ()
	{
		StartCoroutine (WaitForSound ("Main Menu"));
	}

	public void GoToGame ()
	{
		StartCoroutine (WaitForSound ("Main Game"));
	}


	IEnumerator WaitForSound (string level)
	{
		// Wait for button sound to play before changing anything
		audioSourceBtn.Play ();
		while (audioSourceBtn.isPlaying)
		{
			yield return null;
		}
		Destroy (audioSourceBtn);
		// Quit isn't really a scene, so just close the application
		if (level == "Quit")
		{
			Application.Quit ();
		} 
		else
		{
			GameState.currentState = GameState.AvailableStates.MainGameStart;
			// sceneLoader is our loading screen, but we can skip this in case anything happens to the object
			if (sceneLoader)
			{
				sceneLoader.LoadLevelWithLoadingScreen (level);
			} 
			else
			{
				SceneManager.LoadScene (level);
			}
		}
	}
}
