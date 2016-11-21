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
using UnityEngine;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// A html element which represents renderable text.
	/// </summary>
	[HtmlNamespace]
	[Dom.TagName("TextNode")]
	public class HtmlTextNode:Dom.TextNode, HtmlNode, IRenderableNode{
		
		/// <summary>Information such as this nodes computed box model. This one is where 'anonymous' boxes are stored.</summary>
		internal TextRenderableData RenderData_;
		
		
		public HtmlTextNode(){
			RenderData_=new TextRenderableData(this);
		}
		
		/// <summary>This nodes render data.</summary>
		public RenderableData RenderData{
			get{
				return RenderData_;
			}
		}
		
		/// <summary>This nodes computed style.</summary>
		public ComputedStyle ComputedStyle{
			get{
				return (parentNode as HtmlElement ).Style.Computed;
			}
		}
		
		/// <summary>Finds the index of the nearest character to x pixels.</summary>
		/// <param name="x">The number of pixels from the left edge of this text element.</param>
		/// <param name="y">The number of pixels from the top edge of this text element.</param>
		/// <returns>The index of the nearest letter.</returns>
		public int LetterIndex(float x,float y){
			
			// Get the text renderer:
			TextRenderingProperty trp=RenderData.Text;
			
			if(trp==null){
				// It's not been rendered at all yet.
				return 0;
			}
			
			// First, find which box the x/y point is in:
			LayoutBox box=RenderData.BoxAt(x,y);
			
			if(box==null){
				// Nope!
				return 0;
			}
			
			// Walk the characters in the box until we've walked at least x units.
			
			float left=box.X;
			float fontSize=trp.FontSize;
			
			for(int i=box.TextStart;i<box.TextEnd;i++){
				
				// Get the char:
				InfiniText.Glyph glyph=trp.Characters[i];
				
				if(glyph==null){
					continue;
				}
				
				// Move width along:
				left+=glyph.AdvanceWidth * fontSize;
				
				if(left>=x){
					// Got it!
					return i;
				}
				
				// Advance over spacing:
				left+=trp.LetterSpacing;
				
			}
			
			// Not found.
			return 0;
		}
		
		/// <summary>Gets the first element which matches the given selector.</summary>
		public Element querySelector(string selector){
			return null;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector){
			return null;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector,bool one){
			return null;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selectors">The selectors to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public void querySelectorAll(Selector[] selectors,INodeList results,CssEvent e,bool one){}
		
		public void WentOffScreen(){
			RenderData.WentOffScreen();
		}
		
		public void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			// This one never runs, but it's required by the interface.
		}
		
		/// <summary>Gets the relative position in pixels of the letter at the given index.</summary>
		/// <param name="index">The index of the letter in this text element.</param>
		/// <returns>The number of pixels from the left and top edges of this text element the letter is as a vector.</returns>
		public Vector2 GetPosition(ref int index){
			
			// Get the text renderer:
			TextRenderingProperty trp=RenderData.Text;
			
			if(trp==null){
				// It's not been rendered at all yet.
				return Vector2.zero;
			}
			
			// Get the box that contains the given text index.
			LayoutBox box=RenderData.FirstBox;
			
			while(box!=null){
				
				// Note that start is inclusive, end is not.
				if(box.TextStart<=index && box.TextEnd>index){
					// Got it!
					break;
				}
				
				// Next one:
				box=box.NextInElement;
			}
			
			if(box==null){
				// Not found - index is out of range.
				return Vector2.zero;
			}
			
			// Relative to the given box.
			
			// We'll target the very middle of the line on Y:
			float top=box.InnerStartY+(box.InnerHeight/2f);
			
			// And the middle of the character on X.
			
			float left=box.InnerStartX;
			float fontSize=trp.FontSize;
			
			// For each character, excluding the one at 'index'..
			for(int i=box.TextStart;i<index;i++){
				
				// Get the char:
				InfiniText.Glyph glyph=trp.Characters[i];
				
				if(glyph==null){
					continue;
				}
				
				// Advance x over the whole character and spacing:
				left+=(glyph.AdvanceWidth * fontSize)+trp.LetterSpacing;
				
			}
			
			// Advance by half the character:
			left+=trp.Characters[index].AdvanceWidth * fontSize/2f;
			
			// Done!
			return new Vector2(left,top);
			
		}
		
		/// <summary>Called when a @font-face font is done loading.</summary>
		public void FontLoaded(DynamicFont font){
			
			Css.TextRenderingProperty text=RenderData.Text;
			
			if(text!=null){
				
				text.FontLoaded(font);
				
			}
			
		}
		
	}
	
	public class TextRenderableData : RenderableData{
		
		public TextRenderableData(HtmlTextNode node):base(node){}
		
		public override void Reflow(Renderman renderer){
			
			// Create the first anonymous box and apply it:
			LayoutBox box=new LayoutBox();
			box.PositionMode=PositionMode.Static;
			box.DisplayMode=DisplayMode.Inline;
			FirstBox=box;
			LastBox=box;
			
			// Get the text renderer (or create it):
			Css.TextRenderingProperty text=RequireTextProperty();
			
			// Get computed style:
			ComputedStyle cs=computedStyle;
			
			// Get the box meta:
			LineBoxMeta boxMeta=renderer.TopOfStack;
			
			// Get the font:
			text.FontToDraw=boxMeta.FontFamily;
			
			// Colour too:
			Color fontColour=cs.Resolve(Css.Properties.ColorProperty.GlobalProperty).GetColour(this,Css.Properties.ColorProperty.GlobalProperty);
			
			// Colour, style and weight:
			text.BaseColour=fontColour;
			text.Style=cs.ResolveInt(Css.Properties.FontStyle.GlobalProperty);
			text.Weight=cs.ResolveInt(Css.Properties.FontWeight.GlobalProperty);
			
			// Font size update:
			float fontSize=boxMeta.FontSize;
			text.FontSize=fontSize;
			
			// Step 1. Check if the text is 'dirty'.
			// If it is, that means we'll need to rebuild the TextRenderingProperty's Glyph array.
			
			if(text.Dirty){
				
				// Setup text now:
				// (Resets text.Characters based on all the text related CSS properties like variant etc).
				text.LoadCharacters((Node as HtmlTextNode).characterData_,this);
				
			}
			
			if(text.Characters==null || text.AllEmpty){
				return;
			}
			
			// Compute our line boxes based on text.Characters and the available space.
			// Safely ignore direction here because either way the selected characters are the same.
			// Note that things like first-letter are also considered.
			
			// Get the top of the stack:
			LineBoxMeta lbm=renderer.TopOfStack;
			float boxWidth=0f;
			float max=lbm.MaxX-lbm.PenX;
			bool first=true;
			
			for(int i=0;i<text.Characters.Length;i++){
				
				// Get the glyph:
				InfiniText.Glyph glyph=text.Characters[i];
				
				if(glyph==null){
					// Skip!
					continue;
				}
				
				// The glyphs width is..
				float width=glyph.AdvanceWidth * fontSize;
				
				if(first){
					
					// The box always has an inner height of 'font size':
					first=false;
					box.InnerHeight=fontSize;
					boxWidth+=width;
					
				// Does it fit in the current box?
				}else if((boxWidth+width)>max){
					
					// Nope - break!
					
					box.InnerWidth=boxWidth;
					box.TextEnd=i;
					
					if(glyph.Charcode==(int)' '){
						
						// If it's a space, we leave it on *this* line.
						box.TextEnd++;
						box.InnerWidth+=width;
						
						// Ensure dimensions are set:
						box.SetDimensions(false,false);
						
						// Add the box to the line:
						lbm.AddToLine(box);
						
						// Update dim's:
						lbm.AdvancePen(box);
						
						// Complete the current line now:
						lbm.CompleteLine(true,true);
						
						first=true;
						boxWidth=0f;
						box=new LayoutBox();
						box.PositionMode=PositionMode.Static;
						box.DisplayMode=DisplayMode.Inline;
						box.TextStart=i+1;
						
					}else{
						
						// Ensure dimensions are set:
						box.SetDimensions(false,false);
						
						// Add the box to the line:
						lbm.AddToLine(box);
						
						// Update dim's:
						lbm.AdvancePen(box);
						
						// Complete the current line now:
						lbm.CompleteLine(true,true);
						
						// Start the new line width this char:
						boxWidth=width;
						box=new LayoutBox();
						box.PositionMode=PositionMode.Static;
						box.DisplayMode=DisplayMode.Inline;
						box.TextStart=i;
						box.InnerHeight=fontSize;
						
					}
					
					// Update max (clearing floats could potentially reset it):
					max=lbm.MaxX-lbm.PenX;
					
					// add to this element:
					LastBox.NextInElement=box;
					LastBox=box;
					
				}else{
					boxWidth+=width;
				}
				
			}
			
			// Always apply inner w/h:
			box.InnerWidth=boxWidth;
			box.TextEnd=text.Characters.Length;
			
			// Ensure dimensions are set:
			box.SetDimensions(false,false);
			
			// Add the box to the line:
			lbm.AddToLine(box);
			
			// Update dim's:
			lbm.AdvancePen(box);
			
		}
		
	}
	
}