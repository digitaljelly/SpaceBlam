using UnityEngine;
using System.Collections;
/*
	AsteroidSpawn.cs
	Controls spawning of new Asteroids - when and where
*/


public class AsteroidSpawn : MonoBehaviour
{

	public SpaceBlamSettings settings;
	public float screenEdgeOffset = 15f;
	// Could do some clever maths here instead, but this is much easier

	ScreenTilingCtrl tiling;
	Camera cam;

	void Start ()
	{
		cam = Camera.main;
		GameObject gC = GameObject.FindGameObjectWithTag ("GameController");
		if (gC == null)
		{
			Debug.LogError ("Game Controller missing");
		}
		tiling = gC.GetComponent<ScreenTilingCtrl> ();
	}

	// Work out if we need to spawn a new asteroid
	void Update ()
	{
		if (CountAsteroids () < settings.maximumCompleteAsteroids)
		{
			Spawn ();
		}
	}

	// Determine where to spawn a new asteroid and what direction it should head (somewhere in the middle of the screen)
	void Spawn ()
	{
		// get an edge to spawn behind, 0 = top, 1 = right, 2 = bottom, 3 = left
		int direction = Random.Range (0, 4);
		Vector3 spawnPos = Vector3.zero;
		switch (direction)
		{
		case 0:
			spawnPos = cam.ScreenToWorldPoint (new Vector3 (Random.Range (0, cam.pixelWidth), cam.pixelHeight, cam.nearClipPlane));
			break;
		case 1:
			spawnPos = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, Random.Range (0, cam.pixelHeight), cam.nearClipPlane));
			break;
		case 2:
			spawnPos = cam.ScreenToWorldPoint (new Vector3 (Random.Range (0, cam.pixelWidth), 0, cam.nearClipPlane));
			break;
		case 3:
			spawnPos = cam.ScreenToWorldPoint (new Vector3 (0, Random.Range (0, cam.pixelHeight), cam.nearClipPlane));
			break;
		}
		// Add offset
		Vector3 offsetPos = Vector3.zero;
		switch (direction)
		{
		case 0:
			offsetPos = spawnPos + new Vector3 (0f, screenEdgeOffset, 0f);
			break;
		case 1:
			offsetPos = spawnPos + new Vector3 (screenEdgeOffset, 0f, 0f);
			break;
		case 2:
			offsetPos = spawnPos + new Vector3 (0f, -screenEdgeOffset, 0f);
			break;
		case 3:
			offsetPos = spawnPos + new Vector3 (-screenEdgeOffset, 0f, 0f);
			break;
		}

		GameObject newAsteroid = Instantiate (settings.largeAsteroid, offsetPos, Quaternion.identity) as GameObject;
		newAsteroid.AddComponent<IncomingAsteroid> ();
		newAsteroid.GetComponent<Rigidbody2D> ().velocity = (GetNewDirection () - offsetPos).normalized * settings.asteroidSpawnForce / 1000f;
		newAsteroid.tag = "IncomingAsteroid";
	}

	// Get the "total" number of asteroids
	float CountAsteroids ()
	{
		int smallAsteroids = GameObject.FindGameObjectsWithTag ("AsteroidsSmall").Length;
		int mediumAsteroids = GameObject.FindGameObjectsWithTag ("AsteroidsMedium").Length;
		int largeAsteroids = GameObject.FindGameObjectsWithTag ("AsteroidsLarge").Length;
		int incoming = GameObject.FindGameObjectsWithTag ("IncomingAsteroid").Length;
		float total = (smallAsteroids / 4f) + (mediumAsteroids / 2f) + largeAsteroids + incoming;
		return total;
	}

	// Calculate a new direction for a new asteroid  - middle-ish
	Vector3 GetNewDirection ()
	{
		float width = cam.pixelWidth;
		float height = cam.pixelHeight;

		// Calculate a new target position that is within the central 1/2 portion of the screen
		float directionX = (width / 2f) - Random.Range (-(width / 4f), width / 4f);
		float directionY = (height / 2f) - Random.Range (-(height / 4f), height / 4f);

		Vector3 targetPos = cam.ScreenToWorldPoint (new Vector3 (directionX, directionY, cam.nearClipPlane));
		return targetPos;
	}

}