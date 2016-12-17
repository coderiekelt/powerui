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
		
		/// <summary>The total number of spaces.</summary>
		public int SpaceCount;
		/// <summary>The advance width (for a 1px font) of the longest word.</summary>
		public float LongestWord;
		/// <summary>The total advance width (for a 1px font). Doesn't include word spacing.</summary>
		public float TotalAdvanceWidth=float.MinValue;
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
				return (parentNode as IRenderableNode).ComputedStyle;
			}
		}
		
		/// <summary>Finds the best box for the given y coordinate.</summary>
		public LayoutBox BestBox(float y){
			
			// First, find which box the y point is in:
			LayoutBox box=RenderData.FirstBox;
			
			if(y<box.Y){
				return box;
			}
			
			// For each one..
			while(box!=null){
				
				if(y<box.MaxY){
					return box;
				}
				
				// Next box:
				box=box.NextInElement;
				
			}
			
			return RenderData.LastBox;
			
		}
		
		/// <summary>Finds the index of the nearest character to x pixels.</summary>
		/// <param name="x">The number of pixels from the left edge of the screen.</param>
		/// <param name="y">The number of pixels from the top edge of the screen.</param>
		/// <returns>The index of the nearest letter.</returns>
		public int LetterIndex(float x,float y){
			
			// First, find which box the x/y point is in:
			LayoutBox box=BestBox(y);
			
			return LetterIndex(x,box); // <- fitting!
			
		}
		
		/// <summary>Finds the index of the nearest character to x pixels.</summary>
		/// <param name="x">The number of pixels from the left edge of the screen.</param>
		public int LetterIndex(float x,LayoutBox box){
			
			if(box==null){
				// Nope!
				return 0;
			}
			
			// Get the text renderer:
			TextRenderingProperty trp=RenderData.Text;
			
			if(trp==null){
				// It's not been rendered at all yet.
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
			
			// End of the box.
			return box.TextEnd;
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
		
		public void OnRender(Renderman renderer){}
		
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
				text.RequestLayout();
				
			}
			
		}
		
		/// <summary>Part of shrink-to-fit. Computes the maximum and minimum possible width for an element.</summary>
		public void GetWidthBounds(out float min,out float max){
			
			// Get the text renderer (or create it):
			Css.TextRenderingProperty text=RenderData_.RequireTextProperty();
			
			// Need to compute TAW?
			if(TotalAdvanceWidth==float.MinValue){
				
				float width=0f;
				int spaceCount=0;
				float wordWidth=0f;
				LongestWord=0f;
				
				for(int i=0;i<text.Characters.Length;i++){
					
					// Get the glyph:
					InfiniText.Glyph glyph=text.Characters[i];
					
					if(glyph==null){
						// Skip!
						continue;
					}
					
					// The glyph's width is..
					float gWidth=glyph.AdvanceWidth+text.LetterSpacing;
					wordWidth+=gWidth;
					width+=gWidth;
					
					// Got a space?
					if(glyph.Charcode==(int)' '){
						
						if(wordWidth>LongestWord){
							LongestWord=wordWidth;
							wordWidth=0f;
						}
						
						// Advance width:
						spaceCount+=1;
						
					}
					
				}
				
				if(wordWidth>LongestWord){
					LongestWord=wordWidth;
				}
				
				TotalAdvanceWidth=width;
				SpaceCount=spaceCount;
				
			}
			
			min=LongestWord * text.FontSize;
			max=(TotalAdvanceWidth * text.FontSize)+(SpaceCount * text.WordSpacing);
			
		}
		
	}
	
}