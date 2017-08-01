using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
/*
	GameUIControl.cs
	Controls the start and end- game UIs
*/


public class GameUIControl : MonoBehaviour
{

	public GameObject infoPanel;
	public GameObject endPanel;
	public GameObject highScoreNotification;
	public ScoreControl scoreCtrl;


	// This is what we call when hit "Play" from the UI
	public void StartGame ()
	{
		// Hide the start UI
		infoPanel.SetActive (false);

		// Turn on scripts we don't want for any players in the scene
		List<GameObject> players = new List<GameObject> (GameObject.FindGameObjectsWithTag ("Player"));
		foreach (GameObject player in players)
		{
			FireShots playerFireShots = player.GetComponent<FireShots> ();
			Rigidbody2D playerRB = player.GetComponent<Rigidbody2D> ();
			SpaceShipMovement playerMovement = player.GetComponent<SpaceShipMovement> ();
			if (playerFireShots)
			{
				playerFireShots.enabled = true;
			}

			if (playerRB)
			{
				playerRB.WakeUp ();
			}

			if (playerMovement)
			{
				playerMovement.enabled = true;
			}
		}
		GameState.currentState = GameState.AvailableStates.Playing;
	}

	void Start ()
	{
		// Turn off scripts we don't want for any players in the scene
		List<GameObject> players = new List<GameObject> (GameObject.FindGameObjectsWithTag ("Player"));
		foreach (GameObject player in players)
		{
			FireShots playerFireShots = player.GetComponent<FireShots> ();
			Rigidbody2D playerRB = player.GetComponent<Rigidbody2D> ();
			SpaceShipMovement playerMovement = player.GetComponent<SpaceShipMovement> ();

			if (playerFireShots)
			{
				playerFireShots.enabled = false;
			}

			if (playerRB)
			{
				playerRB.Sleep ();
			}

			if (playerMovement)
			{
				playerMovement.enabled = false;
			}
		}
	}

	// Called when the player dies
	public void End ()
	{
		// We instantiate lots of players for tiling, we need to  turn scripts off
		List<GameObject> players = new List<GameObject> (GameObject.FindGameObjectsWithTag ("Player"));
		foreach (GameObject player in players)
		{
			FireShots playerFireShots = player.GetComponent<FireShots> ();
			Rigidbody2D playerRB = player.GetComponent<Rigidbody2D> ();
			SpaceShipMovement playerMovement = player.GetComponent<SpaceShipMovement> ();
			
			if (playerFireShots)
			{
				playerFireShots.enabled = false;
			}

			if (playerRB)
			{
				playerRB.Sleep ();
			}

			if (playerMovement)
			{
				playerMovement.enabled = false;
			}
		}

		// Did we get a new high score? 
		if (scoreCtrl.newHighScore && scoreCtrl.currentScore > 0)
		{
			highScoreNotification.SetActive (true);
		}

		//Show the end UI
		endPanel.SetActive (true);
		// Prevent opening the pause menu, etc
		GameState.currentState = GameState.AvailableStates.Ended;
	}

	public void Restart()
	{
		GameState.currentState = GameState.AvailableStates.MainGameStart;
		SceneManager.LoadScene ("Main Game");
	}
}
