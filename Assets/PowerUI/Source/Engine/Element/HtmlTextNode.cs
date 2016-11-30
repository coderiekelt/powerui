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
				return (parentNode as IRenderableNode).ComputedStyle;
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
		
		/// <summary>Part of shrink-to-fit. Computes the maximum and minimum possible width for an element.</summary>
		public void GetWidthBounds(out float min,out float max){
			
			min=0f;
			max=0f;
			
			// Get computed style:
			ComputedStyle cs=ComputedStyle;
			
			// Overflow-wrap mode (only active for 'break-word' which is just '1'):
			bool overflowWrapActive=( cs.ResolveInt(Css.Properties.OverflowWrap.GlobalProperty) == 1 );
			
			if(!overflowWrapActive){
				// Min required
			}
			
			#warning text width bounds
			// We need the longest single word (min) and the length of the line if it was totally continuous (max).
			
		}
		
	}
	
	public class TextRenderableData : RenderableData{
		
		public TextRenderableData(HtmlTextNode node):base(node){}
		
		public override void Reflow(Renderman renderer){
			
			// Clear the blocks:
			LayoutBox box=null;
			FirstBox=null;
			LastBox=null;
			
			// Get the text renderer (or create it):
			Css.TextRenderingProperty text=RequireTextProperty();
			
			// Get computed style:
			ComputedStyle cs=computedStyle;
			
			// Get the box meta:
			LineBoxMeta boxMeta=renderer.TopOfStack;
			
			// Get the font face:
			text.FontToDraw=boxMeta.FontFace;
			
			// Colour too:
			Color fontColour=cs.Resolve(Css.Properties.ColorProperty.GlobalProperty).GetColour(this,Css.Properties.ColorProperty.GlobalProperty);
			
			// Colour:
			text.BaseColour=fontColour;
			
			// Font size update:
			float fontSize=boxMeta.FontSize;
			text.FontSize=fontSize;
			
			// Get the baseline offset:
			float baseline=fontSize * text.FontToDraw.Descender;
			
			// Spacing:
			float wordSpacing=cs.ResolveDecimal(Css.Properties.WordSpacing.GlobalProperty);
			float letterSpacing=cs.ResolveDecimal(Css.Properties.LetterSpacing.GlobalProperty);
			
			// If word spacing is not 'normal', remove 1em from it (Note that letter spacing is always additive):
			if(wordSpacing==-1f){
				wordSpacing=0f;
			}else{
				wordSpacing-=fontSize;
			}
			
			text.WordSpacing=wordSpacing;
			text.LetterSpacing=letterSpacing;
			
			// Decoration:
			int decoration=cs.ResolveInt(Css.Properties.TextDecorationLine.GlobalProperty);
			
			if(decoration!=0){
				
				// Got a line!
				if(text.TextLine==null){
					text.TextLine=new TextDecorationInfo(decoration);
				}
				
				// Get the colour:
				Css.Value lineColour=cs.Resolve(Css.Properties.TextDecorationColor.GlobalProperty);
				
				if(lineColour==null || lineColour is Css.Keywords.Initial || lineColour is Css.Keywords.CurrentColor){
					
					// No override:
					text.TextLine.ColourOverride=false;
					
				}else{
					
					// Set the colour:
					text.TextLine.SetColour(lineColour.GetColour(this,Css.Properties.TextDecorationColor.GlobalProperty));
					
				}
			
			}
			
			// Get the white-space mode:
			int whiteSpaceMode=cs.WhiteSpaceX;
			
			// Step 1. Check if the text is 'dirty'.
			// If it is, that means we'll need to rebuild the TextRenderingProperty's Glyph array.
			
			if(text.Dirty){
				
				// Setup text now:
				// (Resets text.Characters based on all the text related CSS properties like variant etc).
				text.LoadCharacters((Node as HtmlTextNode).characterData_,this,whiteSpaceMode);
				
			}
			
			if(text.Characters==null || text.AllEmpty){
				return;
			}
			
			// Overflow-wrap mode (only active for 'break-word' which is just '1'):
			bool overflowWrapActive=( cs.ResolveInt(Css.Properties.OverflowWrap.GlobalProperty) == 1 );
			
			// Compute our line boxes based on text.Characters and the available space.
			// Safely ignore direction here because either way the selected characters are the same.
			// Note that things like first-letter are also considered.
			
			// Get the top of the stack:
			LineBoxMeta lbm=renderer.TopOfStack;
			float wordWidth=0f;
			float boxWidth=0f;
			bool wrappable=((whiteSpaceMode & WhiteSpaceMode.Wrappable)!=0);
			int i=0;
			int latestBreakpoint=-1;
			
			while(i<text.Characters.Length){
				
				// Get the glyph:
				InfiniText.Glyph glyph=text.Characters[i];
				
				if(glyph==null){
					// Skip!
					i++;
					continue;
				}
				
				// The glyph's width is..
				float width=(glyph.AdvanceWidth * fontSize)+letterSpacing;
				
				if(box==null){
					
					// The box always has an inner height of 'font size':
					if(renderer.FirstLetter!=null){
						
						// Clear FL immediately (so it can't go recursive):
						SparkInformerNode firstLetter=renderer.FirstLetter;
						renderer.FirstLetter=null;
						
						// Update its internal text node:
						HtmlTextNode textNode=firstLetter.firstChild as HtmlTextNode;
						
						// Note that we have to do it this way as the node might
						// change the *font*.
						textNode.characterData_=((char)glyph.Charcode)+"";
						
						// Ask it to reflow right now (must ask the node so it correctly takes the style into account):
						firstLetter.RenderData.Reflow(renderer);
						
						i++;
						continue;
						
					}
					
					// Create the box now:
					box=new LayoutBox();
					box.PositionMode=PositionMode.Static;
					box.DisplayMode=DisplayMode.Inline;
					box.Baseline=baseline;
					box.TextStart=i;
					
					if(FirstBox==null){
						FirstBox=box;
						LastBox=box;
					}else{
						// add to this element:
						LastBox.NextInElement=box;
						LastBox=box;
					}
					
					box.InnerHeight=fontSize;
					boxWidth=0f;
					wordWidth=0f;
				}
				
				// Got a space?
				bool space=(glyph.Charcode==(int)' ');
				
				if(space){
					
					// Advance width:
					width+=wordSpacing;
					
					// Lock in the previous text:
					latestBreakpoint=i;
					boxWidth+=wordWidth+width;
					wordWidth=0f;
					
				}else{
					
					// Advance word width now:
					wordWidth+=width;
					
				}
				
				// Are we breaking this word?
				int breakMode=(glyph.Charcode==(int)'\n') ? 1 : 0;
				
				// Word wrapping next:
				if(breakMode==0 && wrappable && i!=box.TextStart){
					
					// Test if we can break here:
					breakMode=lbm.GetLineSpace(wordWidth,boxWidth);
					
					// Return to the previous space if we should.
					if(breakMode==2 && overflowWrapActive){
						
						// The word doesn't fit at all (2) and we're supposed to break it.
						boxWidth+=wordWidth-width;
						wordWidth=0f;
					
					}else if(breakMode!=0){
						
						if(latestBreakpoint==-1){
							
							// Isn't a previous space!
							
							if(breakMode==2){
								
								// Instead, we'll try and break a parent.
								// This typically happens with inline elements 
								// which are right on the end of the host line.
								lbm.TryBreakParent();
								
							}
							
							// Don't break the node:
							breakMode=0;
							
						}else{
							i=latestBreakpoint+1;
						}
						
					}
					
				}
				
				if(breakMode!=0){
					
					// We're breaking!
					box.InnerWidth=boxWidth;
					box.TextEnd=i;
					latestBreakpoint=i;
					
					// If the previous glyph is a space, update EndSpaceSize:
					if(space){
						// Update ending spaces:
						box.EndSpaceSize=width;
					}
					
					// Ensure dimensions are set:
					box.SetDimensions(false,false);
					
					// Add the box to the line:
					lbm.AddToLine(box);
					
					// Update dim's:
					lbm.AdvancePen(box);
					
					// Newline:
					lbm.CompleteLine(true,true);
					
					// Clear:
					box=null;
					boxWidth=0f;
					
					if(glyph.Charcode!=(int)'\n'){
						// Process it again.
						continue;
					}
					
				}
				
				// Next character:
				i++;
				
			}
			
			if(box!=null){
				
				// Always apply inner width:
				box.InnerWidth=boxWidth+wordWidth;
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
	
}