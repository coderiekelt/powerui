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
	/// A simple node inspector. Companion of the DOM inspector.
	/// </summary>
	
	public class NodeInspector : EditorWindow{
		
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		
		
		// Add menu item named "Node Inspector" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Node Inspector")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(NodeInspector));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Node Inspector";
			#else
			GUIContent title=new GUIContent();
			title.text="Node Inspector";
			Window.titleContent=title;
			#endif
			
		}
		
		void OnGUI(){
			
			// This is always null - if you can get a node in here then great; see the comment below!
			Node node=DomInspector.MouseOverNode;
			
			if(node==null){
				PowerUIEditor.HelpBox("Currently unavailable - check back later! This will display applied CSS rules, computed style, attached JS events etc. (If you're interested, see the source of this editor UI at PowerUI/Editor/NodeInspector/NodeInspector.cs).");
				return;
			}
			
			string name=node.nodeName;
			
			if(name==null){
				name="(Unnamed node)";
			}
			
			GUILayout.Label(name);
			
			/*
			
			// Is the node renderable?
			IRenderableNode renderable=(node as IRenderableNode);
			
			if(renderable!=null){
				
				// Get the computed style:
				ComputedStyle cs=renderable.ComputedStyle;
				
				// All applied styles:
				List<MatchingRoot> matches=cs.Matches;
				
				if(matches!=null){
					
					foreach(MatchingRoot matching in matches){
						
						// Note! These nodes can be participants from other nearby elements.
						// That happens with any combined selector.
						// To filter those ones out, check if this element is the actual target:
						if(matching.IsTarget){
							
							// Rule is simply matching.Rule:
							StyleRule rule=matching.Rule;
							
							// To get the underlying selector, it's rule.Selector:
							Selector selector=rule.Selector;
							
							// Note that this builds the selector text - avoid calling from OnGUI!
							// string str=selector.selectorText;
							
						}
						
					}
					
				}
				
				// Computed values:
				foreach(KeyValuePair<CssProperty,Css.Value> kvp in cs.Properties){
					
					// kvp.Key (a CSS property) is set to kvp.Value (a CSS value)
					
				}
				
			}
			
			// JS event hooks:
			EventTarget target=(node as EventTarget);
			
			if(target!=null && target.Events!=null){
				
				// Grab the JS event handlers:
				Dictionary<string,List<EventListener>> allHandlers=target.Events.Handlers;
				
				// Key is e.g. "mousedown"
				// Value is all the things that'll run when the event triggers.
				
			}
			
			*/
			
		}
		
	}
	
}