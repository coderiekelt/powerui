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
	/// Displays some general settings for PowerUI.
	/// </summary>

	public class GeneralSettings : EditorWindow{
		
		/// <summary>A timer which causes Update to run once every 2 seconds.</summary>
		public static int UpdateTimer;
		/// <summary>The tickboxes.</summary>
		public static SettingTickbox[] Settings;
		
		// Add menu item named "General Settings" to the PowerUI menu:
		[MenuItem("Window/PowerUI/General Settings")]
		public static void ShowWindow(){
			
			Settings=new SettingTickbox[]{
				#if !UNITY_4_6 && !UNITY_4_7 && PRE_UNITY5
					new SettingTickbox("Isolate UI classes","IsolatePowerUI","This isolates the 'UI' class inside the PowerUI namespace just incase you've got a class called UI of your own."),
				#endif
				new SettingTickbox("Disable Input","NoPowerUIInput","Disable PowerUI's input handling. You'll need to use e.g. Input.ElementFromPoint to find an element at a particular point on the UI.")
				//new SettingTickbox("Focus Graph","PowerUIFGraph","Pressing the arrow keys will cause PowerUI to move focus to the nearest input.")
			};
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(GeneralSettings));
			
			// Give it a title:
			#if PRE_UNITY5_3
			window.title="General Settings";
			#else
			GUIContent title=new GUIContent();
			title.text="General Settings";
			window.titleContent=title;
			#endif

		}
		
		// Called at 100fps.
		void Update(){
			UpdateTimer++;
			
			if(UpdateTimer<100){
				return;
			}
			
			UpdateTimer=0;
			
			// Reduced now to once every second.
			foreach(SettingTickbox setting in Settings){
				
				// Check for the symbol:
				setting.Update();
				
			}
			
		}
		
		void OnGUI(){
			
			foreach(SettingTickbox setting in Settings){
				
				// Draw it:
				setting.OnGUI();
				
			}
			
		}
		
	}

}