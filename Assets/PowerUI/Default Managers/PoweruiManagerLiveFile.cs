using UnityEngine;
using System.Collections;
using PowerUI;

/// <summary>
/// This class is the default PowerUI Manager Monobehaviour with a live HTML file reloader.
/// It's used in the same way as the normal manager only when you save your file it will instantly reload.
/// </summary>

public class PoweruiManagerLiveFile : UnityEngine.MonoBehaviour {
	
	/// <summary>A File containing the html/css/nitro of your UI.</summary>
	public TextAsset HtmlFile;
	/// <summary>The UI screen Resolution. See UI.Resolution for more info. Note that this is a very basic usage!
	/// PowerUI can also select different images too without affecting your html.</summary>
	public float SimpleResolution=1f;
	#if UNITY_EDITOR && !UNITY_WEBPLAYER
	/// <summary>The file watcher which does the bulk of the work.</summary>
	private LiveHtml Reloader;
	#endif
	
	
	// OnEnable is called when the game starts, or when the manager script component is enabled.
	void OnEnable () {
		
		// Optional. This allows PowerUI to easily work with screens of varying resolution.
		if(SimpleResolution!=1f){
			UI.Resolution=new ResolutionInfo(SimpleResolution);
		}
		
		// Load the UI from the above HtmlFile. Note that UI's don't have to be loaded like this! You
		// can also just set a string of text if needed.
		if(HtmlFile==null){
		
			Debug.Log("Please provide a HTML file for your UI. "+
				"If you're stuck, please see the Getting Started guide on the website (http://powerUI.kulestar.com/)"+
				", in the GettingStarted folder, or feel free to contact us! We're always happy to help :)");
			
			#if UNITY_EDITOR && !UNITY_WEBPLAYER
			
			Reloader.Stop();
			Reloader=null;
			
			#endif
			
		}else{
			
			// Write the html:
			UI.Html=HtmlFile.text;
		
			#if UNITY_EDITOR && !UNITY_WEBPLAYER
			
			// Get the full asset path:
			string fullPath=UnityEditor.AssetDatabase.GetAssetPath(HtmlFile);
			
			// Create the loader:
			Reloader=new LiveHtml(fullPath,OnHtmlChanged);
			
			#endif
			
		}
		
	}
	
	/// <summary>Called when the .html file is saved.</summary>
	void OnHtmlChanged(string path,string html){
		
		UI.Html=html;
		
	}
	
	// OnDisable is called when the manager script component is disabled. You don't need this.
	void OnDisable () {
		UI.Destroy();
		
		#if UNITY_EDITOR && !UNITY_WEBPLAYER
		
		Reloader.Stop();
		Reloader=null;
		
		#endif
		
	}
	
}
