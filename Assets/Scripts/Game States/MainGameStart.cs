using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
	MainGameStart.cs
	Instantiates the player properly. 
	Provides a convenient function to find all player Rigidbod2Ds if needed
	Doesn't do anything else yet, could be expanded if needed.
*/


public class MainGameStart : MonoBehaviour
{
	// Our spaceship
	public GameObject playerPrefab;

	// Create the player
	void Awake ()
	{
		Instantiate (playerPrefab, new Vector3 (0f, 0f, 0f), Quaternion.identity);
	}

}
