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


namespace PowerSlide{
	
	/// <summary>
	/// A PowerSlide manager. Active slides operate in these and there is only ever one manager per Document.
	/// </summary>
	
	public class Manager{
		
		/// <summary>The document that this is managing.</summary>
		public PowerUI.HtmlDocument document;
		/// <summary>The actively displaying slide.</summary>
		public Slide currentSlide;
		/// <summary>The actively displaying track.</summary>
		public Track currentTrack;
		
		
		public Manager(PowerUI.HtmlDocument document){
			this.document=document;
		}
		
	}
	
}

namespace PowerUI{

	public partial class HtmlDocument{
		
		/// <summary>Instance of a PowerSlide manager. May be null.</summary>
		private PowerSlide.Manager slides_;
		
		/// <summary>The document.slides API. Read only.</summary>
		public PowerSlide.Manager slides{
			get{
				
				if(slides_==null){
					slides_=new PowerSlide.Manager(this);
				}
				
				return slides_;
			}
		}
		
	}
	
}