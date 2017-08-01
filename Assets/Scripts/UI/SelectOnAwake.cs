using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
  	SelectOnAwake.cs
	Simple utility to automatically make a UI component be selected when its un-hidden
*/

[RequireComponent (typeof(Selectable))]
public class SelectOnAwake : MonoBehaviour
{

	// Dont call in Awake or Enable, as Select() doesn't work there
	void Start ()
	{
		Selectable selectableComponent = GetComponent<Selectable> ();
		selectableComponent.Select ();
	}
}