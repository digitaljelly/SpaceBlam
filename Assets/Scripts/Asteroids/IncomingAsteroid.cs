using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
	IncomingAsteroid.cs
	For a specific asteroid prefab that gets instantiated off-screen. 
	We need to handle this differently for these, add this component to do this.
	
	We change its physics layer to only interact with "original" players and asteroids, so 
	that the player doesn't get killed by something they can't see
	
	Once the asteroid is within the bounds of the screen, we then tile it, removing this component as we do so.

*/


[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(CircleCollider2D))]
[RequireComponent (typeof(Asteroid))]
public class IncomingAsteroid : MonoBehaviour
{

	// Useful components to store or re-use
	Rigidbody2D rb;
	Collider2D coll;
	Camera cam;
	ScreenTilingCtrl tiling;

	// This is an incoming asteroid, so tweak its collision layers and tags
	// This gets undone once its within the level boundary
	void Awake ()
	{
		coll = GetComponent<Collider2D> ();
		coll.isTrigger = true;
		rb = GetComponent<Rigidbody2D> ();
		cam = Camera.main;
		GameObject gC = GameObject.FindGameObjectWithTag ("GameController");
		tiling = gC.GetComponent<ScreenTilingCtrl> ();
		gameObject.layer = LayerMask.NameToLayer ("Incoming Asteroid");
		gameObject.tag = "IncomingAsteroid";
	}
	
	// If we leave the screen, destroy this
	// The AsteroidSpawn handler will respawn another one that is heading towards the screen
	void OnTriggerExit2D (Collider2D coll)
	{
		if (coll.tag == "EdgeCollider")
		{
			Destroy (gameObject);		
		}
	}

	// Calculate whether we're on the screen completely (and hence tile and fix layers/tags)
	// or just a bit - enable the normal collision process
	void FixedUpdate ()
	{
		
		Vector3 bottomLeft = cam.ScreenToWorldPoint (new Vector3 (0f, 0f, cam.nearClipPlane));
		Vector3 topRight = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
		
		Bounds bounds = coll.bounds;
		Vector3 myBottomLeft = bounds.min;
		Vector3 myTopRight = bounds.max;
		if (myTopRight.x < topRight.x && myTopRight.y < topRight.y && myBottomLeft.x > bottomLeft.x && myBottomLeft.y > bottomLeft.y)
		{
			if (tiling)
			{
				tiling.TileAnObject (gameObject);
			}
		// if only partially in the screen
		}
		else if (myTopRight.x < topRight.x || myTopRight.y < topRight.y || myBottomLeft.x > bottomLeft.x || myBottomLeft.y > bottomLeft.y)
		{
			coll.isTrigger = false;
		}
	}
	
}
