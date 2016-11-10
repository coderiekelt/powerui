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
using Dom;
using Blaze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Css;


namespace MathML{
	
	/// <summary>
	/// A base class for all MathML tag types. This is used to distictively identify them.
	/// </summary>
	
	[XmlNamespace("http://www.w3.org/1998/Math/MathML","mml",typeof(MathDocument))]
	[Dom.TagName("Default")]
	public class MathElement:Element, IRenderableNode{
		
		/// <summary>This elements style.</summary>
		public ElementStyle Style;
		
		
		public MathElement(){
			
		}
		
		/// <summary>This nodes computed style.</summary>
		public ComputedStyle ComputedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Called when an attribute of the element was changed.
		/// Returns true if the method handled the change to prevent unnecessary checks.</summary>
		public override bool OnAttributeChange(string property){
			
			if(property=="style"){
				Style.cssText=this["style"];
				return true;
			}
			
			// Style refresh:
			if(Style.Computed.Matches!=null){
				// This is a runtime attribute change.
				// We must consider if it's affecting the style or not:
				Style.Computed.AttributeChanged(property);
			}
			
			if(property=="id"){
				return true;
			}else if(property=="class"){
				return true;
			}else if(property=="name"){
				// Nothing happens with this one - ignore it.
				return true;
				
			}else if(property=="href"){
				
				// MathML link to some location.
				return true;
			
			}else if(property=="dir"){
				
				// Text direction.
				return true;
				
			}else if(property=="fontstyle"){
				
				Style.Computed.ChangeTagProperty(
					"font-style",
					this[property]
				);
				
				return true;
			
			}else if(property=="fontweight"){
				
				Style.Computed.ChangeTagProperty(
					"font-weight",
					this[property]
				);
				
				return true;
			
			}else if(property=="mathvariant"){
				
				Style.Computed.ChangeTagProperty(
					"-spark-math-variant",
					this[property]
				);
				
				return true;
			
			}else if(property=="scriptsizemultiplier"){
				
				Style.Computed.ChangeTagProperty(
					"-spark-script-size-multiplier",
					this[property]
				);
				
				return true;
			
			}else if(property=="scriptminsize"){
				
				Style.Computed.ChangeTagProperty(
					"-spark-script-min-size",
					this[property]
				);
				
				return true;
			
			}else if(property=="scriptlevel"){
				
				Style.Computed.ChangeTagProperty(
					"-spark-script-level",
					this[property]
				);
				
				return true;
			
			}else if(property=="fontfamily"){
				
				Style.Computed.ChangeTagProperty(
					"font-family",
					this[property]
				);
				
				return true;
			
			}else if(property=="mathbackground" || property=="background"){
				
				Style.Computed.ChangeTagProperty(
					"background-color",
					new Css.Units.ColourUnit(
						Css.ColourMap.ToSpecialColour(this[property])
					)
				);
				
				return true;
			
			}else if(property=="mathcolor" || property=="color"){
				
				Style.Computed.ChangeTagProperty(
					"color",
					new Css.Units.ColourUnit(
						Css.ColourMap.ToSpecialColour(this[property])
					)
				);
				
				return true;
				
			}else if(property=="font-size" || property=="mathsize"){
				
				Style.Computed.ChangeTagProperty(
					"font-size",
					this[property]
				);
				
				return true;
				
			}else if(property=="onmousedown"){
				return true;
			}else if(property=="onmouseup"){
				return true;
			}else if(property=="onkeydown"){
				return true;
			}else if(property=="onkeyup"){
				return true;
			}else if(property=="height"){
				string height=this["height"];
				if(height.IndexOf("%")==-1 && height.IndexOf("px")==-1 && height.IndexOf("em")==-1){
					height+="px";
				}
				style.height=height;
				return true;
			}else if(property=="width"){	
				string width=this["width"];
				if(width.IndexOf("%")==-1 && width.IndexOf("px")==-1 && width.IndexOf("em")==-1){
					width+="px";
				}
				style.width=width;
				return true;
			}
			
			return false;
		}
		
		/// <summary>Gets the first element which matches the given selector.</summary>
		public Element querySelector(string selector){
			HTMLCollection results=querySelectorAll(selector,true);
			
			if(results==null || results.length==0){
				return null;
			}
			
			return results[0] as Element;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector){
			return querySelectorAll(selector,false);
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector,bool one){
		
			// Create results set:
			HTMLCollection results=new HTMLCollection();
			
			if(string.IsNullOrEmpty(selector)){
				// Empty set:
				return results;
			}
			
			// Create the lexer:
			Css.CssLexer lexer=new Css.CssLexer(selector,this);
			
			// Read a value:
			Css.Value value=lexer.ReadValue();
			
			// Read the selectors from the value:
			List<Selector> selectors=new List<Selector>();
			Css.CssLexer.ReadSelectors(null,value,selectors);
			
			// Create a blank event to store the targets, if any:
			CssEvent e=new CssEvent();
			
			// Perform the selection process:
			querySelectorAll(selectors.ToArray(),results,e,false);
			
			return results;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selectors">The selectors to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public void querySelectorAll(Selector[] selectors,INodeList results,CssEvent e,bool one){
			if(childNodes_==null){
				return;
			}
			
			for(int i=0;i<childNodes_.length;i++){
				Node node=childNodes_[i];
				IRenderableNode child=node as IRenderableNode;
				
				if(child==null){
					continue;
				}
				
				RenderableData renderData=child.RenderData;
				
				for(int s=0;s<selectors.Length;s++){
					
					// Match?
					if(selectors[s].Test(renderData,e)!=null){
						// Yep!
						results.push(node);
						
						if(one){
							return;
						}
					}
					
				}
				
				child.querySelectorAll(selectors,results,e,one);
				
				if(one && results.length==1){
					return;
				}
			}
			
		}
		
		/// <summary>This nodes render data.</summary>
		public RenderableData RenderData{
			get{
				return Style.Computed.RenderData;
			}
		}
		
		/// <summary>This elements style.</summary>
		public ElementStyle style{
			get{
				return Style;
			}
		}
		
		/// <summary>Gets the computed style of this element.</summary>
		public Css.ComputedStyle computedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Called during the box compute process. Useful if your element has clever dimensions, such as the img tag or words.</summary>
		public virtual void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
		}
		
		/// <summary>Called when a font-face is ready.</summary>
		public void FontLoaded(PowerUI.DynamicFont font){
			
		}
		
	}
	
}