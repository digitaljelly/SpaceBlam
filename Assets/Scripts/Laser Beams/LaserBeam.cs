using UnityEngine;
using System.Collections;
/*
	LaserBeam.cs
	Component to control laser behaviour.
*/


[RequireComponent (typeof(Rigidbody2D))]
public class LaserBeam : MonoBehaviour
{
	// These get set externally when spawned (so we have an opportunity
	// at that point to do more complex logic, if we wish
	// Hidden so we can edit in scripts, but its useless for anyone to edit
	[HideInInspector]
	public float shotForce = 3f;

	[SerializeField]
	float lifeTime = 20f;

	// On spawn, make the beam move!
	void Start ()
	{
		GetComponent<Rigidbody2D> ().AddForce (shotForce * transform.up, ForceMode2D.Impulse);
		StartCoroutine (DestroyAfterTimer ());
	}

	// Lasers will self-destruct if they go beyond the screen bounds
	// However, asteroid shields could create lasers that bounce across the screen
	// for a lot of time, and that could hit performance. We add a max lifetime to combat that
	IEnumerator DestroyAfterTimer ()
	{
		yield return new WaitForSeconds (lifeTime);
		Destroy (gameObject);
	}
}
