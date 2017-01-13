using UnityEngine;
using System.Collections;
using PowerUI;

public class TVScreen : PowerUI.Manager {
	
	/// <summary>Is input enabled for WorldUI's?</summary>
	[Tooltip("Disable input for extra performance")]
	public bool InputEnabled=true;
	
	
	public override void OnEnable () {
		
		// Generate a new UI (the name is optional).
		// The two numbers are the dimensions of our virtual screen:
		WorldUI ui=new WorldUI("TVScreenContent",800,600);
		
		// But we need to define the resolution - that's how many pixels per world unit.
		// This of course entirely depends on the model and usage.
		// This is just setting the scale so you don't have to use this particular method.
		ui.SetResolution(540,610);
		
		// Give it some content using PowerUIManager's Navigate method:
		Navigate(ui.document);
		
		// Parent it to the TV:
		ui.transform.parent=transform;
		
		// Let's move it around a little too:
		ui.transform.localPosition=new Vector3(0f,1.02f,0.03f);
		
		// And spin it around - the TV mesh is facing the other way:
		ui.transform.localRotation=Quaternion.AngleAxis(180f,Vector3.up);
		
		// Optionally accept input:
		ui.AcceptInput=InputEnabled;
		
	}
	
}
