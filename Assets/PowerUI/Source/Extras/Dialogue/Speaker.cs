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

using System;
using UnityEngine;
using PowerUI;
using Json;


namespace Dialogue{
	
	/// <summary>
	/// A speaker. Either an item (which includes NPCs), an item instance or a user.
	/// Note that if there are multiple instances of an item in the scene, all of them are selected.
	/// </summary>
	
	public class Speaker{
		
		/// <summary>
		/// Typically an ID but can also be a custom reference used when resolving who the speaker is.
		/// </summary>
		public string Reference;
		/// <summary>The type of speaker.</summary>
		public SpeakerType Type;
		/// <summary>The card this is a speaker for.</summary>
		public Card Card;
		
		
		public Speaker(Card card){
			Card=card;
		}
		
		/// <summary>Loads the speaker meta from the given JSON data.</summary>
		public Speaker(Card card,JSObject data){
			Card=card;
			Load(data);
		}
		
		/// <summary>Creates speaker meta for the given type and reference.</summary>
		public Speaker(SpeakerType type,string reference){
			
			Type=type;
			Reference=reference;
			
			Init();
			
		}
		
		/// <summary>Loads the speaker meta from the given JSON data.</summary>
		public void Load(JSObject data){
			
			// Load the type:
			string type=data.CaseString("type",false);
			
			if(type=="user" || type=="player"){
				Type=SpeakerType.User;
			}else if(type=="instance"){
				Type=SpeakerType.Instance;
			}else{
				Type=SpeakerType.Item;
			}
			
			// Ref:
			Reference=data.String("id");
			Init();
		}
		
		/// <summary>Makes sure the ref is setup correctly.</summary>
		private void Init(){
			
			// Check for 'self' or 0:
			if(Type==SpeakerType.User){
				
				string reference=Reference.ToLower().Trim();
				
				if(reference=="self" || reference=="0"){
					// Got it! Just use null:
					Reference=null;
				}
				
			}
			
		}
		
		/// <summary>True if this speaker represents the current user. Use 'self', null or 0 as your reference.</summary>
		public bool IsCurrentUser{
			get{
				return (Type==SpeakerType.User && Reference==null);
			}
		}
		
	}
	
	/// <summary>
	/// The type of a speaker.
	/// </summary>
	public enum SpeakerType{
		/// <summary>An item ID. Includes NPCs.</summary>
		Item,
		/// <summary>A particular instance of an item.</summary>
		Instance,
		/// <summary>Referencing some player. Use 'self'/null/0 to refer to *this* player.</summary>
		User
	}
	
}