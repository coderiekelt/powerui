using UnityEngine;
using System.Collections;
using PowerUI;

public class FlatInWorldUI : PowerUI.Manager, Dom.IEventTarget {
	
	/// <summary>Is input enabled for WorldUI's?</summary>
	[Tooltip("Disable input for extra performance")]
	public bool InputEnabled=true;
	/// <summary>Amount of pixel space the UI has.</summary>
	public int Width=600;
	/// <summary>Amount of pixel space the UI has.</summary>
	public int Height=400;
	/// <summary>The FlatWorldUI itself.</summary>
	internal FlatWorldUI FlatUI;
	
	
	// OnEnable is called when the game starts, or when the manager script component is enabled.
	public override void OnEnable () {
		
		// Next, generate a new UI using the given virtual screen dimensions (the name is optional but helps debug the document):
		FlatUI=new FlatWorldUI(gameObject.name,Width,Height);
		
		// Use PowerUI.Manager's Navigate function (which reads either Url or HtmlFile depending on which you set):
		Navigate(FlatUI.document);
		
		// Optionally accept input:
		FlatUI.AcceptInput=InputEnabled;
		
		// Apply to the material on this gameObjects mesh renderer:
		// (Note: You can also just grab FlatUI.Texture - this is just a convenience function)
		FlatUI.ApplyTo(gameObject);
		
	}
	
	/// <summary>Called when an event is being dispatched here.</summary>
	public bool dispatchEvent(Dom.Event e){
		
		Debug.Log("EVENT DISPATCHED!");
		
		// Ok!
		return true;
		
	}
	
}
