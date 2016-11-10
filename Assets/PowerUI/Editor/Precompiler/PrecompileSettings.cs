//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY5
#endif

#if PRE_UNITY5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define PRE_UNITY5_3
#endif

using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Pico;


namespace PowerUI{

	/// <summary>
	/// Displays options for precompiling PowerUI. Highly recommended you use this!
	/// </summary>

	public class PrecompileSettings : EditorWindow{
		
		/// <summary>True if PowerUI is precompiled.</summary>
		public static bool Precompiled;
		/// <summary>True if the current precompile is in editor mode.</summary>
		public static bool EditorMode=true;
		
		
		// Add menu item named "Precompile" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Precompile")]
		public static void ShowWindow(){
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(PrecompileSettings));

			// Give it a title:
			#if PRE_UNITY5_3
			window.title="Precompile";
			#else
			GUIContent title=new GUIContent();
			title.text="Precompile";
			window.titleContent=title;
			#endif

			LoadSettings();
			
		}
		
		public static void LoadSettings(){
			
			// Get the is precompiled and editor state from the settings:
			Module settings=Precompiler.GetModule("PowerUI");
			
			if(settings==null){
				Precompiled=false;
				EditorMode=true;
			}else{
				Precompiled=true;
				EditorMode=settings.EditorMode;
			}
			
		}
		
		void OnGUI(){
			
			bool previousValue=Precompiled;
			Precompiled=EditorGUILayout.Toggle("Precompile PowerUI",previousValue);
			PowerUIEditor.HelpBox("Ensure you have a backup first as this will move files. Highly recommended that you use this - PowerUI is a big library! Precompiles PowerUI such that it doesn't get built every time you change any of your scripts. Note that this precompiler can be used for your scripts too.");
			
			if(previousValue!=Precompiled){
				OnPrecompileChanged();
			}
			
			previousValue=EditorMode;
			EditorMode=EditorGUILayout.Toggle("Editor Mode",previousValue);
			PowerUIEditor.HelpBox("Compile with the UNITY_EDITOR flag.");
			
			if(previousValue!=EditorMode){
				OnEditorModeChanged();
			}
			
			if(Precompiled && GUILayout.Button("Recompile")){
				
				Recompile();
				
			}
			
		}
		
		private void OnEditorModeChanged(){
			
			if(!Precompiled){
				return;
			}
			
			Recompile();
			
		}
		
		private void OnPrecompileChanged(){
			
			Precompile();
			
		}
		
		public void Recompile(){
			
			Precompiler.Recompile("PowerUI",EditorMode);
			
		}
		
		private void Precompile(){
			
			if(!Precompiled){
				
				// Undo the "PowerUI" precompiled module.
				Precompiler.Reverse("PowerUI");
				
				return;
				
			}
			
			List<string> paths=new List<string>();
			
			// Find PowerUI:
			string powerUIPath=PowerUIEditor.GetPowerUIPath();
			
			paths.Add(powerUIPath+"/Source");
			
			Precompiler.Precompile(
				paths,
				"PowerUI",
				EditorMode
			);
			
		}
		
	}

}