using UnityEngine;
using System.Collections;
using PowerUI;


public class FadingScreenExample : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
		// Get the main UI doc:
		var document=UI.document;
		
		
		// - The onload event -
		
		// Grab the fade to black <a>:
		var fade2black=document.getElementById("fade2black");
		
		// Add a handler for the onload event (it's a UIEvent):
		fade2black.onload=delegate(UIEvent e){
			
			Debug.Log("Black fade completed!");
			
		};
		
		
		
		// - Using the fade method from script (rather than via the window protocol) -
		
		// Get the fade to red button:
		var fade2red=document.getElementById("fade2red");
		
		// Hook up our mousedown:
		fade2red.onmousedown=delegate(MouseEvent e){
			
			// Clicked on the button - let's run a fade now!
			// This is using the method inside ScreenFade.cs (which is in this examples files).
			// It returns a Promise (Web API) so we can chain events together in an interesting way!
			
			ScreenFade.Fade(document,Color.red,2f).then(delegate(object o){
				
				// It's done!
				Debug.Log("Red fade completed!");
				
				// Fading to a transparent colour removes it (or use ScreenFade.Close(document) to remove it that way).
				
				// Returning the promise means it gets passed to the following Then instead.
				
				// Alternatively we could just put .Then at the end of it, 
				// but this approach avoids heavy nesting and it's standardised anyway!
				return ScreenFade.Fade(document,new Color(1f,0f,0f,0f),1f);
				
			}).then(delegate(object o){
				
				// We're back!
				Debug.Log("Screen has faded back in!");
				
				// No need to return anything:
				return null;
				
			});
			
		};
		
	}
	
}
