using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
	Customize.cs
	Controls player's ship custom colours.
	If no sliders are provided, it'll still load and set the colours loaded from the settings
*/


public class Customize : MonoBehaviour
{

	// Used to get our previous saved settings and store our changes
	public SpaceBlamSettings settings;
	// WHat custom shader are we editing the look of?
	public Material customShader;

	// As it says below - this script will just be a getter-style
	// script unless we specify sliders, where upon we gain the
	// ability to edit the values
	[Header ("Fill if customization is available in the UI")]
	public Slider sliderR1;
	public Slider sliderG1;
	public Slider sliderB1;

	public Slider sliderR2;
	public Slider sliderG2;
	public Slider sliderB2;

	// Load values
	void Start ()
	{
		customShader.SetColor ("_CustomColor1", settings.shipColor1);
		customShader.SetColor ("_CustomColor2", settings.shipColor2);
		// update UI - condensed the carriage returns here (sorry), otherwise we get a long
		// repeating list
		if (sliderR1)
			sliderR1.value = settings.shipColor1.r;
		if (sliderG1)
			sliderG1.value = settings.shipColor1.g;
		if (sliderB1)
			sliderB1.value = settings.shipColor1.b;

		if (sliderR2)
			sliderR2.value = settings.shipColor2.r;
		if (sliderG2)
			sliderG2.value = settings.shipColor2.g;
		if (sliderB2)
			sliderB2.value = settings.shipColor2.b;
	}

	// All these functions should be attached to the OnChanged events on the slider
	// components, so that the sliders change the values.
	// If it wasn't obvious, r = red channel, g= = green channel, b = blue channel, a = alpha channel
	public void modR1 (float newVal)
	{
		Color newColor = new Color (newVal, settings.shipColor1.g, settings.shipColor1.b, settings.shipColor1.a);
		customShader.SetColor ("_CustomColor1", newColor);
		settings.shipColor1 = newColor;
	}

	public void modG1 (float newVal)
	{
		Color newColor = new Color (settings.shipColor1.r, newVal, settings.shipColor1.b, settings.shipColor1.a);
		customShader.SetColor ("_CustomColor1", newColor);
		settings.shipColor1 = newColor;
	}

	public void modB1 (float newVal)
	{
		Color newColor = new Color (settings.shipColor1.r, settings.shipColor1.g, newVal, settings.shipColor1.a);
		customShader.SetColor ("_CustomColor1", newColor);
		settings.shipColor1 = newColor;
	}

	public void modR2 (float newVal)
	{
		Color newColor = new Color (newVal, settings.shipColor2.g, settings.shipColor2.b, settings.shipColor2.a);
		customShader.SetColor ("_CustomColor2", newColor);
		settings.shipColor2 = newColor;
	}

	public void modG2 (float newVal)
	{
		Color newColor = new Color (settings.shipColor2.r, newVal, settings.shipColor2.b, settings.shipColor2.a);
		customShader.SetColor ("_CustomColor2", newColor);
		settings.shipColor2 = newColor;
	}

	public void modB2 (float newVal)
	{
		Color newColor = new Color (settings.shipColor2.r, settings.shipColor2.g, newVal, settings.shipColor2.a);
		customShader.SetColor ("_CustomColor2", newColor);
		settings.shipColor2 = newColor;
	}
}