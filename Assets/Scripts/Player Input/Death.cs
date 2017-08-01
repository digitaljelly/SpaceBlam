using UnityEngine;
using System.Collections;

/*
	Death.cs
	Controls player death
*/

[RequireComponent (typeof(PlayerSounds))]
[RequireComponent (typeof(FireShots))]
[RequireComponent (typeof(SpaceShipMovement))]
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class Death : MonoBehaviour, IKillable
{

	/*
		TODO:
		* Add in some effects, etc on death of the player
	*/

	// Ideally End() should be called from a central "Game-End-Contoller"
	// script, but as I've no other logic to be called than from
	// GameUIControl, I'm bypassing that step for now.
	GameUIControl gameUICtrl;
	ScreenTilingCtrl tiling;

	// Store some components
	void Start ()
	{
		GameObject gC = GameObject.FindGameObjectWithTag ("GameController");
		gameUICtrl = gC.GetComponent<GameUIControl> ();
		tiling = gC.GetComponent<ScreenTilingCtrl> ();
	}

	// Implements IKillable
	public void Kill ()
	{
		GameObject[] lasers = GameObject.FindGameObjectsWithTag ("Laser");
		foreach (GameObject laser in lasers)
		{
			Destroy (laser);
		}
		if (gameUICtrl != null)
		{
			gameUICtrl.End ();
		}

		PlayerSounds pS = gameObject.GetComponent<PlayerSounds> ();
		pS.explosionSource.Play ();

		DisableComponents ();
		// Play an explosion sound when the player dies
		StartCoroutine (WaitUntilSoundFinished (pS.explosionSource));
	}

	void DisableComponents ()
	{
		// Disable everything until sound has played
		PlayerSounds pS = gameObject.GetComponent<PlayerSounds> ();
		FireShots fS = gameObject.GetComponent<FireShots> ();
		SpaceShipMovement sS = gameObject.GetComponent<SpaceShipMovement> ();
		Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D> ();
		Collider2D coll = gameObject.GetComponent<Collider2D> ();

		fS.enabled = false;
		sS.enabled = false;
		coll.enabled = false;
		rb.Sleep ();

		foreach (Transform child in transform)
		{
			child.gameObject.SetActive (false);
		}

		pS.gunSource.Stop ();
		pS.thrustersSource.Stop ();
	}

	// Only destroy the object after we've died
	// Safe to do this, "Restart" reloads the scene
	IEnumerator WaitUntilSoundFinished (AudioSource aSource)
	{
		while (aSource.isPlaying)
		{
			yield return null;
		}
		tiling.DestroyATiledObject (gameObject);
	}
}


