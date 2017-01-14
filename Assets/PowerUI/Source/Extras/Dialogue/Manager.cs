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


namespace Dialogue{
	
	/// <summary>
	/// A dialogue manager. Active dialogue operates in these and there is only ever one manager per Document.
	/// </summary>
	
	public class Manager{
		
		/// <summary>The document that this is managing.</summary>
		public PowerUI.HtmlDocument document;
		/// <summary>The actively displaying card.</summary>
		public Card currentCard;
		/// <summary>The actively displaying train.</summary>
		public Train currentTrain;
		
		
		public Manager(PowerUI.HtmlDocument document){
			this.document=document;
		}
		
	}
	
}

namespace PowerUI{

	public partial class HtmlDocument{
		
		/// <summary>Instance of a window manager. May be null.</summary>
		private Dialogue.Manager dialogue_;
		
		/// <summary>The document.dialogue API. Read only.</summary>
		public Dialogue.Manager sparkDialogue{
			get{
				
				if(dialogue_==null){
					dialogue_=new Dialogue.Manager(this);
				}
				
				return dialogue_;
			}
		}
		
	}
	
}