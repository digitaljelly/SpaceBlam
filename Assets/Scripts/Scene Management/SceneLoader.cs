using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
/*
	SceneLoader.cs
	Use like so: ...<SceneLoader>().LoadLevelWithLoadingScreen ("My Level", skipLoadingScreen);
	Loading scene must contain a slider with the tag 'LoadingProgressBar' for loading bar to work
*/

public class SceneLoader : MonoBehaviour
{

	[SerializeField]
	float minDuration = 1.5f;
	Slider progressBar;
	float startTime = 0f;

	// Keep across all scenes so we can call this to load the loading sceen
	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
	}

	// We have the option to skip the loading screen, added in for argument's sake
	// I don't use it currently
	public void LoadLevelWithLoadingScreen (string levelName, bool skip = false)
	{
		startTime = Time.time;
		if (skip)
		{
			SceneManager.LoadScene (levelName);
		}
		else
		{
			StartCoroutine (LoadSceneAsync (levelName));
		}

	}


	public IEnumerator LoadSceneAsync (string sceneName)
	{
		// Load in our loading scene, don't switch scenes until its loaded, though
		yield return SceneManager.LoadSceneAsync ("Loading");

		// Find a progress bar, use it
		GameObject progressBarObj = GameObject.FindGameObjectWithTag ("LoadingProgressBar");
		if (progressBarObj)
		{
			progressBar = progressBarObj.GetComponent<Slider> ();
		}

		// Used to create a false loading time, as level loads are so fast
		// that I can't show off my pro-tips!
		float endTime = Time.time + minDuration;

		// Check progress of our new level
		AsyncOperation progression = SceneManager.LoadSceneAsync (sceneName);
		progression.allowSceneActivation = false;

		if (progression.progress < 0.9)
		{
			if (progressBar)
			{
				progressBar.value = progression.progress;
			}
			yield return null;
		}

		while (Time.time < endTime)
		{
			float adjustedTimeEnd = startTime + minDuration;
			float adjustedTimeNow = Time.time - startTime;
			float progressValue = (adjustedTimeNow) / minDuration;
			if (progressBar)
			{
				progressBar.value = progressValue;
			}
			yield return null;
		}
		progression.allowSceneActivation = true;

	}

}