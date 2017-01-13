using UnityEngine;
using System.Collections;
using PowerUI;

public class FlatInWorldUI : PowerUI.Manager {
	
	/// <summary>Is input enabled for WorldUI's?</summary>
	[Tooltip("Disable input for extra performance")]
	public bool InputEnabled=true;
	
	
	// OnEnable is called when the game starts, or when the manager script component is enabled.
	public override void OnEnable () {
		
		// Next, generate a new UI using the given virtual screen dimensions (the name is optional):
		FlatWorldUI ui=new FlatWorldUI("BillboardContent",600,400);
		
		// Use PowerUI.Manager's Navigate function (which reads either Url or HtmlFile depending on which you set):
		Navigate(ui.document);
		
		// Optionally accept input:
		ui.AcceptInput=InputEnabled;
		
		// Apply to the material on this gameObjects mesh renderer:
		// (Note: You can also just grab ui.Texture - this is just a convenience function)
		ui.ApplyTo(gameObject);
		
	}
	
}
