using UnityEngine;
using System.Collections;
using PowerUI;

public class FlatInWorldUI : PowerUI.Manager {
	
	/// <summary>Is input enabled for WorldUI's?</summary>
	[Tooltip("Disable input for extra performance")]
	public bool InputEnabled=true;
	/// <summary>Amount of pixel space the UI has.</summary>
	public int Width=600;
	/// <summary>Amount of pixel space the UI has.</summary>
	public int Height=400;
	
	// OnEnable is called when the game starts, or when the manager script component is enabled.
	public override void OnEnable () {
		
		// Next, generate a new UI using the given virtual screen dimensions (the name is optional but helps debug the document):
		FlatWorldUI ui=new FlatWorldUI(gameObject.name,Width,Height);
		
		// Use PowerUI.Manager's Navigate function (which reads either Url or HtmlFile depending on which you set):
		Navigate(ui.document);
		
		// Optionally accept input:
		ui.AcceptInput=InputEnabled;
		
		// Apply to the material on this gameObjects mesh renderer:
		// (Note: You can also just grab ui.Texture - this is just a convenience function)
		ui.ApplyTo(gameObject);
		
	}
	
}
