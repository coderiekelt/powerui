// MIT license (Free to do whatever you want with)
// Originates from the PowerUI Wiki: http://powerui.kulestar.com/wiki/index.php?title=Screen_Fading_(Fade_to_black/_Whiteouts)
// It's formatted as a window so you can stack other things on top of it.

using System;
using PowerUI;
using Windows;
using System.Collections;
using System.Collections.Generic;
	
/// <summary>
/// Fades the screen to a specified colour in a specified amount of time.
/// </summary>

[Dom.TagName("screenfade")]
public class ScreenFade : Windows.Window{
	
	/// <summary>A helper function for instantly removing a screen fade.</summary>
	[Values.Preserve]
	public static void Close(PowerUI.HtmlDocument doc){
		doc.sparkWindows.close("screenfade",null);
	}
	
	/// <summary>A helper function for fading the screen in the given document.</summary>
	[Values.Preserve]
	public static Promise Fade(PowerUI.HtmlDocument doc,UnityEngine.Color to,float timeInSeconds){
		
		// Open up the window:
		var w=doc.sparkWindows.open("screenfade",null,"to",to,"time",timeInSeconds);
		
		Promise p=new Promise();
		
		if(timeInSeconds<=0f){
			// Instant:
			p.resolve(w);
		}else{
			
			// Add an event cb:
			w.addEventListener("load",delegate(UIEvent e){
				
				// Ok!
				p.resolve(w);
				
			});
			
		}
		
		return p;
		
	}
	
	public override StackMode StackMode{
		get{
			// Hijack an existing window so we can fade from the 'current' colour onwards
			return StackMode.Hijack;
		}
	}
	
	/// <summary>The depth that this type of window lives at.</summary>
	public override int Depth{
		get{
			// Very high (always right at the front)
			return 100000;
		}
	}
	
	/// <summary>Called when asked to fade.</summary>
	public override void Load(string url,Dictionary<string,object> globals){
		
		// Get the colour:
		UnityEngine.Color colour=GetColour("to",globals,UnityEngine.Color.black);
		
		// Get the time:
		float time=(float)GetDecimal("time",globals,0);
		
		// Element is not null when we 'hijacked' an existing window (and we're fading from its current color instead).
		if(element==null){
			
			// Write the HTML now:
			SetHtml("<div style='width:100%;height:100%;position:fixed;top:0px;left:0px;'></div>");
			
		}
		
		// Run the animation:
		element.animate("background-color:"+colour.ToCss()+";",time).OnDone(delegate(UIAnimation animation){
			
			// If the opacity is 0, close the window:
			if(colour.a<=0.001f){
				close();
			}
			
			// Done! Trigger a 'load' event.
			// It will run on element (the window itself), the Window object and 
			// (if there is one), the original anchor tag.
			trigger("load",globals);
			
		});
		
	}
	
}