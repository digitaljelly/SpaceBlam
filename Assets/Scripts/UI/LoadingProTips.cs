using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
	LoadingProTips.cs
	Fun facts and tips for your loading screen!
*/


[RequireComponent (typeof(Text))]
public class LoadingProTips : MonoBehaviour
{

	[SerializeField]
	string[] proTips;

	void Start ()
	{
		// Pick a random string to show
		GetComponent<Text> ().text = proTips [Random.Range (0, proTips.Length)];
	}
}