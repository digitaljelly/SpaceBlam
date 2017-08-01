using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
/*
	InitScript.cs
	For allowing space to setup all DontDestroyOnLoad objects we want to keep
	throughout all our scenes
	They do this themselves, this script simply loads to the first main menu
*/


public class InitScript : MonoBehaviour
{

	void Start ()
	{
		SceneManager.LoadScene ("Main Menu");
	}	
}
