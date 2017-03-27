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

namespace PowerUI{

	/// <summary>
	/// Displays a small checkbox for compiling Nitro AOT.
	/// </summary>

	public class NitroSettings : EditorWindow{
		
		/// <summary>A timer which causes Update to run once every 2 seconds.</summary>
		public static int UpdateTimer;
		/// <summary>True if Nitro should be compiled ahead of time.</summary>
		public static bool CompileAOT;
		/// <summary>True if Nitro should not be available at runtime (AOT Only).</summary>
		public static bool NoRuntime;
		
		
		// Add menu item named "Nitro Settings" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Nitro Settings")]
		public static void ShowWindow(){
			// Load CompileAOT value:
			CompileAOT=Symbols.IsSymbolDefined("nitroAot");
			
			// Load No Runtime:
			NoRuntime=Symbols.IsSymbolDefined("NoNitroRuntime");
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(NitroSettings));

			// Give it a title:
			#if PRE_UNITY5_3
			window.title="Nitro Settings";
			#else
			GUIContent title=new GUIContent();
			title.text="Nitro Settings";
			window.titleContent=title;
			#endif

		}
		
		// Called at 100fps.
		void Update(){
			UpdateTimer++;
			if(UpdateTimer<200){
				return;
			}
			UpdateTimer=0;
			// Reduced now to once every 2 seconds.
			
			// Load CompileAOT value, just incase it changed in the actual defines window:
			CompileAOT=Symbols.IsSymbolDefined("nitroAot");
			// Load NoRuntime value, just incase it changed in the actual defines window:
			NoRuntime=Symbols.IsSymbolDefined("NoNitroRuntime");
		}
		
		void OnGUI(){
			bool previousValue=CompileAOT;
			CompileAOT=EditorGUILayout.Toggle("Precompile Nitro code",previousValue);
			PowerUIEditor.HelpBox("Note: Nitro on iOS and WP8 requires this. Normally, Nitro is compiled at runtime just like Javascript in a web browser. "+
									"It can however be compiled here in the Editor ('Ahead of time') for more performance. "+
									"Scripts that haven't been precompiled will still be compiled at runtime.");
			if(previousValue!=CompileAOT){
				OnAOTChanged();
			}
			
			previousValue=NoRuntime;
			NoRuntime=EditorGUILayout.Toggle("No Runtime",previousValue);
			PowerUIEditor.HelpBox("Note: Nitro on WP8 requires this. Forces a precompile only requirement. Nitro will not be able to compile at runtime.");
			if(previousValue!=NoRuntime){
				OnRuntimeChanged();
			}
		}
		
		/// <summary>Called when the Runtime checkbox is changed.</summary>
		private void OnRuntimeChanged(){
			
			// Grab the nitro path:
			string nitroPath=NitroPath;
			
			if(NoRuntime){
				// Rename Compiler folder to 'Editor'.
				if(Directory.Exists(nitroPath+"/Compiler")){
					AssetDatabase.RenameAsset(nitroPath+"/Compiler","Editor");
				}
			}else{
				// Rename the folder back.
				if(Directory.Exists(nitroPath+"/Editor")){
					AssetDatabase.RenameAsset(nitroPath+"/Editor","Compiler");
				}
			}
			AssetDatabase.SaveAssets();
			
			if(NoRuntime){
				Symbols.DefineSymbol("NoNitroRuntime");
			}else{
				Symbols.UndefineSymbol("NoNitroRuntime");
			}
		}
		
		private string NitroPath{
			get{
				return PowerUIEditor.GetPowerUIPath()+"/Source/JavaScript/NitroV1/Engine";
			}
		}
		
		/// <summary>Called when the CompileAOT value changes.</summary>
		private void OnAOTChanged(){
			// Update DLL files:
			if(CompileAOT){
				NitroAOT.CompileAll();
			}else{
				NitroAOT.DeleteAll();
			}
			
			// Write it out into the scripting defines symbols:
			if(CompileAOT){
				Symbols.DefineSymbol("nitroAot");
			}else{
				Symbols.UndefineSymbol("nitroAot");
			}
			
		}
		
	}

}