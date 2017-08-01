using UnityEngine;
using System.Collections;
/*
	GameState.cs
	Controls state of the game - paused, playing, ended, etc
	This is static because we want to access it globally and easily, and only need one of it
*/

public class GameState : MonoBehaviour 
{

	public enum AvailableStates {MainGameStart, Playing, Paused, Ended};
	public static AvailableStates currentState = AvailableStates.MainGameStart;

}
