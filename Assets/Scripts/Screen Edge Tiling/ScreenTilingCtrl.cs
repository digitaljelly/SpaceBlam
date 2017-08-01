using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	ScreenTilingCtrl.cs
	Controls tiling of objects.
	If we create a new object in the scene, we call TileAnObject(GameObject obj) to tile it
	To destroy a tiled object and all its instances we call DestroyATiledObject(GameObject obj)
*/


public class ScreenTilingCtrl : MonoBehaviour
{
	// Track objects that we are allowed the tile, on level start, that are in our scene
	// TODO: add this to TileAnObject as a final check on objects that we are allowed
	// to tile
	public List<string> tagsThatTile;
	Camera cam;
	Dictionary<string, Vector2> boundaries = new Dictionary<string, Vector2> ();

	void Start ()
	{
		// Get screen size in world coords..
		cam = Camera.main;
		Vector3 bottomLeft = cam.ScreenToWorldPoint (new Vector3 (0f, 0f, cam.nearClipPlane));

		float screenWidth = GetScreenWidthInWorldUnits ();
		float screenHeight = GetScreenHeightInWorldUnits ();

		bottomLeft += new Vector3 (0.5f * screenWidth, 0.5f * screenHeight, 0f);

		// screenBounds.x is bottom left
		boundaries ["offsetBottomLeft"] = new Vector2 (bottomLeft.x - screenWidth, bottomLeft.y - screenHeight);
		boundaries ["offsetLeft"] = new Vector2 (bottomLeft.x - screenWidth, bottomLeft.y);
		boundaries ["offsetTopLeft"] = new Vector2 (bottomLeft.x - screenWidth, bottomLeft.y + screenHeight);
		boundaries ["offsetTop"] = new Vector2 (bottomLeft.x, bottomLeft.y + screenHeight);
		boundaries ["offsetTopRight"] = new Vector2 (bottomLeft.x + screenWidth, bottomLeft.y + screenHeight);
		boundaries ["offsetRight"] = new Vector2 (bottomLeft.x + screenWidth, bottomLeft.y);
		boundaries ["offsetBottomRight"] = new Vector2 (bottomLeft.x + screenWidth, bottomLeft.y - screenHeight);
		boundaries ["offsetBottom"] = new Vector2 (bottomLeft.x, bottomLeft.y - screenHeight);

		// Get tilable objects
		foreach (string str in tagsThatTile)
		{
			foreach (GameObject gObj in GameObject.FindGameObjectsWithTag (str))
			{
				TileAnObject (gObj);
			}
		}
	}

	// Utility functions
	float GetScreenWidthInWorldUnits ()
	{
		float z = cam.nearClipPlane;
		Vector3 bottomLeft = cam.ScreenToWorldPoint (new Vector3 (0f, 0f, z));
		Vector3 bottomRight = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, 0f, z));
		return bottomRight.x - bottomLeft.x;
	}

	float GetScreenHeightInWorldUnits ()
	{
		float z = cam.nearClipPlane;
		Vector3 bottomLeft = cam.ScreenToWorldPoint (new Vector3 (0f, 0f, z));
		Vector3 topLeft = cam.ScreenToWorldPoint (new Vector3 (0f, cam.pixelHeight, z));
		return topLeft.y - bottomLeft.y;
	}

	// Called externall, we want to tile this specified object
	public void TileAnObject (GameObject gObj)
	{
	
		// Remove IncomingAsteroid if it exists. This script is for newly instantiated asteroids only
		IncomingAsteroid incoming = gObj.GetComponent<IncomingAsteroid> ();
		if (incoming)
		{
			Collider2D coll = gObj.GetComponent<Collider2D> ();
			if (coll)
			{
				coll.isTrigger = false;
			}
			Destroy (incoming);
			gObj.layer = LayerMask.NameToLayer ("Asteroid");
			gObj.tag = "Asteroid";
		}
		
		// Instance around our grid 8 additional times, fixing with a fixedJoint
		GameObject parentObj = new GameObject ();
		if (gObj.tag == "Asteroid")
		{
			Asteroid asteroidScript = gObj.GetComponent<Asteroid> ();
			if (asteroidScript != null)
			{
				parentObj.name = asteroidScript.size == 1 ? "AsteroidsSmall" :
								 asteroidScript.size == 2 ? "AsteroidsMedium" : "AsteroidsLarge";
				parentObj.tag = parentObj.name;
			} else
			{
				parentObj.name = gObj.name;
			}
		} else
		{
			parentObj.name = gObj.name;
		}
		gObj.name = System.Guid.NewGuid ().ToString ();
		gObj.AddComponent<ObjectTilingCtrl> ().isOriginal = true;
		
		// Get shield sync information
		//		Setting these variables here so we can re-use them later without using more memory
		Transform shieldTf;
		GameObject childObj;
		AsteroidShields shieldScript;
		
		if (gObj.tag == "Asteroid")
		{
			shieldTf = gObj.transform.GetChild (0);
			if (shieldTf != null)
			{
				childObj = shieldTf.gameObject;
				shieldScript = childObj.GetComponent<AsteroidShields> ();
				if (shieldScript)
				{
					shieldScript.SyncSheilds ();
				}
			}
		}
		
		foreach (KeyValuePair<string, Vector2> bound in boundaries)
		{
			GameObject newObj = TileItHere (bound.Value, gObj);
			if (newObj != null)
			{
				newObj.transform.SetParent (parentObj.transform);
				if (gObj.tag == "Asteroid")
				{
					// Sync up shields...
					shieldTf = newObj.transform.GetChild (0);
					if (shieldTf != null)
					{
						childObj = shieldTf.gameObject;
						shieldScript = childObj.GetComponent<AsteroidShields> ();
						if (shieldScript)
						{
							shieldScript.SyncSheilds ();
						}
					}
				}
			}
		}
		// Configure some layers for original objects so 
		// we can control how non-tiled objects interact
		gObj.transform.SetParent (parentObj.transform);
		if (gObj.tag == "Asteroid")
		{
			gObj.layer = LayerMask.NameToLayer ("Original Asteroid");
		} else
		{
			gObj.layer = LayerMask.NameToLayer ("Original Player");
		}
	}


	// Called for each section that we're creating a tiled version for
	GameObject TileItHere (Vector2 coordOffset, GameObject original)
	{

		Rigidbody2D rb = original.GetComponent<Rigidbody2D> ();
		if (rb == null)
		{
			return null;
		}

		Vector3 originalPos = original.transform.position;
		Quaternion originalRot = original.transform.rotation;

		coordOffset += new Vector2 (originalPos.x, originalPos.y);
		Vector3 spawnPos = new Vector3 (coordOffset.x, coordOffset.y, cam.nearClipPlane);
		GameObject obj = Instantiate (original, spawnPos, originalRot) as GameObject;
		obj.name = original.name;

		// Fix joints on asteroids
		if (obj.tag != "Player")
		{
			FixedJoint2D cJ = obj.AddComponent<FixedJoint2D> ();
			cJ.autoConfigureConnectedAnchor = true;
			cJ.connectedBody = rb;
			obj.layer = LayerMask.NameToLayer ("Asteroid");
			obj.tag = "Asteroid";
		}

		// These are clones
		obj.GetComponent<ObjectTilingCtrl> ().isOriginal = false;
		return obj;

	}

	public void DestroyATiledObject (GameObject gObj)
	{
		string guid = gObj.name;
		Transform parentTf = gObj.transform.parent;
		GameObject parent = null;
		if (parentTf)
		{
			parent = parentTf.gameObject;
		}
		List<GameObject> objs = new List<GameObject> (GameObject.FindGameObjectsWithTag (gObj.tag));
		foreach (GameObject obj in objs)
		{
			if (guid == obj.name)
			{
				Destroy (obj);
			}
		}
		if (parent != null)
		{
			Destroy (parent);
		}

	}
}
