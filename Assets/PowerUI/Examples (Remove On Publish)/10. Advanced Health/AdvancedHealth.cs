using UnityEngine;
using System.Collections;
using PowerUI;

public class AdvancedHealth : MonoBehaviour {
	
	/// <summary>An instance of the power bar - a dynamic graphic.</summary>
	public PowerBar PowerBarGraphic;
	/// <summary>An instance of the health bar - a dynamic graphic.</summary>
	public HealthBar HealthBarGraphic;
	
	
	// Use this for initialization
	void Start () {
		
		// Create our dynamic textures:
		PowerBarGraphic=new PowerBar();
		HealthBarGraphic=new HealthBar();
		
		// Internally they have now setup 
		// dynamic://healthbar and dynamic://powerbar
		// links for easy access from the html.
		
	}
	
	// Update is called once per frame
	void Update () {
		
		// Increase health/power by 0.2 a second:
		PowerBarGraphic.IncreasePower(0.2f*Time.deltaTime);
		
		HealthBarGraphic.IncreaseHealth(0.2f*Time.deltaTime);
		
	}
	
}