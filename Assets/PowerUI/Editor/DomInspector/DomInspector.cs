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

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;
using Dom;
using System.Collections;
using System.Collections.Generic;
using PowerUI.Http;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// A simple DOM inspector.
	/// </summary>
	
	public class DomInspector : EditorWindow{
		
		/// <summary>The node with the mouse over it.</summary>
		public static Node MouseOverNode;
		/// <summary>The document being viewed.</summary>
		public static Document Document;
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		/// <summary>All unfolded nodes.</summary>
		public static Dictionary<Node,bool> ActiveUnfolded;
		
		
		// Add menu item named "DOM Inspector" to the PowerUI menu:
		[MenuItem("Window/PowerUI/DOM Inspector")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(DomInspector));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="DOM Inspector";
			#else
			GUIContent title=new GUIContent();
			title.text="DOM Inspector";
			Window.titleContent=title;
			#endif
			
		}
		
		// Called at 100fps.
		void Update(){
			
			if(Document==null){
				
				// Grab the main UI document (may still be null):
				Document=UI.document;
				
			}
			
		}
		
		/// <summary>Redraws the node inspector.</summary>
		void RedrawNodeInspector(){
			
			if(NodeInspector.Window!=null){
				
				NodeInspector.Window.Repaint();
				
			}
			
		}
		
		void DrawNode(Node node){
			
			string name=(node is DocumentType)?"#doctype":node.nodeName;
			
			if(name==null){
				name="(Unnamed node)";
			}
			
			if(node.childCount==0){
				
				// Leaf node of the DOM:
				GUILayout.BeginHorizontal();
				GUILayout.Space(EditorGUI.indentLevel * 20);
				GUILayout.Label(name);
				GUILayout.EndHorizontal();
				return;
				
			}
			
			bool unfolded=ActiveUnfolded.ContainsKey(node);
			bool status=EditorGUILayout.Foldout(unfolded, name);
			
			if(status!=unfolded){
				
				// Changed - update active:
				if(status){
					ActiveUnfolded[node]=true;
				}else{
					ActiveUnfolded.Remove(node);
				}
				
				MouseOverNode=node;
				
				RedrawNodeInspector();
				
			}
			
			// Check mouse (Doesn't work unfortunately!):
			Rect zone=GUILayoutUtility.GetLastRect();
			
			if(zone.Contains(Event.current.mousePosition)){
				
				// Mark as the node with the mouse over it:
				MouseOverNode=node;
				RedrawNodeInspector();
				
			}
			
			if(status){
				
				// It's open - draw kids:
				NodeList kids=node.childNodes;
				if(kids!=null){
					
					int indent=EditorGUI.indentLevel+1;
					
					for(int i=0;i<kids.length;i++){
						
						// Get the child:
						Node child=kids[i];
						
						EditorGUI.indentLevel=indent;
						
						// Draw it too:
						DrawNode(child);
						
					}
					
				}
				
			}
			
		}
		
		void OnGUI(){
			
			if(Document==null){
				PowerUIEditor.HelpBox("Waiting for a suitable document (click this window after hitting play). Open the node inspector to view a particular node's properties.");
				return;
			}
			
			if(ActiveUnfolded==null){
				ActiveUnfolded=new Dictionary<Node,bool>();
			}
			
			EditorGUI.indentLevel=0;
			
			// For each node in the document..
			DrawNode(Document);
			
		}
		
	}
	
}