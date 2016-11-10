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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using Css;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a clickable link. Note that target is handled internally by the http protocol.
	/// </summary>
	
	[Dom.TagName("a")]
	public class HtmlAnchorElement:HtmlElement{
		
		/// <summary>The target url that should be loaded when clicked.</summary>
		public string Href;
		
		
		public HtmlAnchorElement(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				Element node=lexer.FormattingCurrentlyOpen("a");
				
				if(node!=null){
					
					// (parse error)
					lexer.AdoptionAgencyAlgorithm("a");
					
					lexer.CloseNode(node);
					lexer.FormattingElements.Remove(node);
					
				}
				
				lexer.AddFormattingElement(this);
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.AdoptionAgencyAlgorithm("a");
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="href"){
				Href=this["href"];
				return true;
			}
			
			return false;
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Time to go to our Href.
			
			#if MOBILE || UNITY_METRO
			
			// First, look for <source> elements.
			
			// Grab the kids:
			NodeList kids=childNodes_;
			
			if(kids!=null){
				// For each child, grab it's src value. Favours the most suitable protocol for this platform (e.g. market:// on android).
				
				foreach(Node child in kids){
					
					HtmlElement el=child as HtmlElement ;
					
					if(el==null || el.Tag!="source"){
						continue;
					}
					
					// Grab the src:
					string childSrc=el["src"];
					
					if(childSrc==null){
						continue;
					}
					
					// Get the optional type - it can be Android,W8,IOS,Blackberry:
					string type=el["type"];
					
					if(type!=null){
						type=type.Trim().ToLower();
					}
					
					#if UNITY_ANDROID
						
						if(type=="android" || childSrc.StartsWith("market:")){
							
							Href=childSrc;
							
						}
						
					#elif UNITY_WP8 || UNITY_METRO
					
						if(type=="w8" || type=="wp8" || type=="windows" || childSrc.StartsWith("ms-windows-store:")){
							
							Href=childSrc;
							
						}
						
					#elif UNITY_IPHONE
						
						if(type=="ios" || childSrc.StartsWith("itms:") || childSrc.StartsWith("itms-apps:")){
							
							Href=childSrc;
							
						}
						
						
					#endif
					
				}
				
			}
			
			#endif
			
			if(!string.IsNullOrEmpty(Href)){
				
				Location path=new Location(Href,document.basepath);
				
				// Do we have a file protocol handler available?
				FileProtocol fileProtocol=path.Handler;
				
				if(fileProtocol!=null){
					fileProtocol.OnFollowLink(this,path);
				}
				
			}
			
		}
		
	}
	
}