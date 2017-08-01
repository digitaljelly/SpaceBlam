using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
	ScoreControl.cs
	Controls loading/saving/updating the score
*/


public class ScoreControl : MonoBehaviour
{

	public SpaceBlamSettings settings;
	// Current score
	public int currentScore;
	// The text boxes to display the scores on to
	public Text currentScoreText;
	public Text highScoreText;
	// USed to indicate if the player beat their score this round
	public bool newHighScore = false;

	// Setup
	void Start ()
	{
		UpdateHighScore ();
	}

	// Can be called from anywhere
	public void AddPoints (int points)
	{
		currentScore += points;
		UpdateCurrentScore ();
		if (currentScore > settings.highScore)
		{
			settings.highScore = currentScore;
			newHighScore = true;
			UpdateHighScore ();
		}
	}


	// Updates the Text components
	// Pad the scores with leading digits (8 digits max)
	void UpdateCurrentScore ()
	{
		if (currentScoreText)
		{
			currentScoreText.text = currentScore.ToString ("D8");
		}
	}

	void UpdateHighScore ()
	{
		if (highScoreText)
		{
			highScoreText.text = settings.highScore.ToString ("D8");
		}
	}


}
