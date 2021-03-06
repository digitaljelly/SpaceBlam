﻿using UnityEngine;
using System.Collections;
/*
	EdgeCollisionDetection.cs
	Creates a center collider that has the CentreCollision.cs component attached on Start() 
*/


public class EdgeCollisionDetection : MonoBehaviour
{
	public GameObject centreCollisionPrefab;

	// Use this for initialization
	void Start ()
	{
		Camera cam = Camera.main;

		// Get screen edges in world-space so we know screen bounds easily
		Vector3 bottomLeft = cam.ScreenToWorldPoint (Vector3.zero);
		Vector3 topRight = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, cam.pixelHeight, 1f));

		GameObject centre = Instantiate (centreCollisionPrefab, bottomLeft, Quaternion.identity) as GameObject;

		centre.name = "Centre Collider";

		centre.transform.SetParent (cam.transform);

		BoxCollider2D box = centre.GetComponent<BoxCollider2D> ();
		box.size = new Vector2 (topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
		box.offset = new Vector2 (0.5f * box.size.x, 0.5f * box.size.y);
	
	}
}