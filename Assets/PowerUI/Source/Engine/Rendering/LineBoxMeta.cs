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
using System.Collections;
using System.Collections.Generic;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// Stores the information used whilst laying out boxes during a reflow.
	/// <summary>

	public class LineBoxMeta{
		
		/// <summary>The "host" block box.</summary>
		public BlockBoxMeta HostBlock;
		/// <summary>The height of the current line being processed.</summary>
		public float LineHeight;
		/// <summary>The current font size.</summary>
		public float FontSize_;
		/// <summary>A linked list of elements on a line are kept. This is the last element on the current line.</summary>
		internal LayoutBox LastOnLine;
		/// <summary>A linked list of elements on a line are kept. This is the first element on the current line.</summary>
		internal LayoutBox FirstOnLine;
		/// <summary>The last child element of an element to be packed onto any line. Tracked for valign.</summary>
		internal LayoutBox LastPacked;
		/// <summary>The first child element of an element to be packed onto any line. Tracked for valign.</summary>
		internal LayoutBox FirstPacked;
		/// <summary>The value of the CSS line-height property.</summary>
		public float CssLineHeight_=float.MinValue;
		/// <summary>The set of active floated elements for the current line being rendered.</summary>
		internal List<LayoutBox> ActiveFloats;
		/// <summary>The current x location of the renderer in screen pixels from the left.</summary>
		internal float PenX;
		/// <summary>The point at which lines begin at.</summary>
		public float LineStart;
		/// <summary>The position of the text baseline.</summary>
		public float Baseline;
		/// <summary>The current box being worked on.</summary>
		internal LayoutBox CurrentBox;
		/// <summary>The next box in the hierarchy.</summary>
		public LineBoxMeta Parent;
		/// <summary>The inline element.</summary>
		public RenderableData RenderData;
		
		
		public LineBoxMeta(LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData){
			
			Parent=parent;
			CurrentBox=firstBox;
			RenderData=renderData;
			
		}
		
		/// <summary>The value of the CSS line-height property.</summary>
		public float CssLineHeight{
			get{
				
				if(CssLineHeight_==float.MinValue){
					// Parent:
					return HostBlock.CssLineHeight_;
				}
				
				return CssLineHeight_;
			}
			set{
				CssLineHeight_=value;
			}
		}
		
		/// <summary>The value of the CSS font-size property.</summary>
		public float FontSize{
			get{
				
				if(FontSize_==float.MinValue){
					// Parent:
					return HostBlock.FontSize_;
				}
				
				return FontSize_;
			}
			set{
				FontSize_=value;
			}
		}
		
		/// <summary>The length of the longest line so far.</summary>
		public virtual float LargestLineWidth{
			get{
				return HostBlock.LargestLineWidth_;
			}
			set{
				HostBlock.LargestLineWidth_=value;
			}
		}
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		public virtual float PenY{
			get{
				return HostBlock.PenY_;
			}
			set{
				HostBlock.PenY_=value;
			}
		}
		
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		public virtual bool GoingLeftwards{
			get{
				return HostBlock.GoingLeftwards_;
			}
			set{
				HostBlock.GoingLeftwards_=value;
			}
		}
		
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		public virtual float MaxX{
			get{
				return HostBlock.MaxX_;
			}
			set{
				HostBlock.MaxX_=value;
			}
		}
		
		/// <summary>The current font family in use.</summary>
		internal Css.Units.FontFamilyUnit FontFamily_;
		
		/// <summary>The current font family in use.</summary>
		internal virtual Css.Units.FontFamilyUnit FontFamily{
			get{
				
				if(FontFamily_==null){
					// Use the block:
					return HostBlock.FontFamily;
				}
				
				return FontFamily_;
				
			}
			set{
				FontFamily_=value;
			}
		}
		
		
		/// <summary>Adds the given style to the current line.</summary>
		/// <param name="style">The style to add.</param>
		internal void AddToLine(LayoutBox styleBox){
			
			if((styleBox.PositionMode & PositionMode.InFlow)==0){
				// Out of flow - margins only:
				styleBox.ParentOffsetLeft=PenX+styleBox.Margin.Left;
				styleBox.ParentOffsetTop=PenY+styleBox.Margin.Top;
				return;
			}
			
			// Make sure it's safe:
			styleBox.NextPacked=null;
			styleBox.NextOnLine=null;
			
			if(styleBox.FloatMode==FloatMode.None){
				
				// In flow - add to line
				
				if(FirstPacked==null){
					FirstPacked=LastPacked=styleBox;
				}else{
					LastPacked=LastPacked.NextPacked=styleBox;
				}
				
				if(FirstOnLine==null){
					FirstOnLine=LastOnLine=styleBox;
				}else{
					LastOnLine=LastOnLine.NextOnLine=styleBox;
				}
				
			}else{
				
				// Using float - add to active floaters:
				
				if(ActiveFloats==null){
					ActiveFloats=new List<LayoutBox>(1);
				}
				
				ActiveFloats.Add(styleBox);
				
			}
			
		}
		
		/// <summary>Completes a line, optionally breaking it.</summary>
		public void CompleteLine(bool breakLine,bool topOfStack){
			
			float lineHeight=LineHeight;
			
			if(breakLine || topOfStack){
				
				// Vertically align all elements on the current line and reset it:
				LayoutBox currentBox=FirstOnLine;
				FirstOnLine=null;
				LastOnLine=null;
				
				while(currentBox!=null){
					// Calculate the offset to where the top left corner is (of the complete box, margin included):
					
					// Must be positioned such that the boxes sit on this lines baseline.
					// the baseline is by default at half the line-height but moves up whenever 
					// an inline-block element with padding/border/margin is added.
					
					float delta=(currentBox.Height+currentBox.Margin.Bottom);
					
					if(currentBox.DisplayMode==DisplayMode.Inline){
						
						// Must also move it down by padding and border:
						delta-=currentBox.Border.Bottom + currentBox.Padding.Bottom;
						
					}
					
					delta=lineHeight-delta;
					
					if(currentBox.TEST){
						Dom.Log.Add("SECONDARY LINE!",delta);
					}
					
					currentBox.ParentOffsetTop=PenY+delta;
					
					// Hop to the next one:
					currentBox=currentBox.NextOnLine;
				}
				
			}
			
			// Recurse down to the nearest block root element.
			
			if(this is BlockBoxMeta){
				
				// Done recursing downwards - we're at the block!
				
				if(breakLine || topOfStack){
					
					// Move the pen down to the following line:
					PenY+=lineHeight;
					
					if(ActiveFloats!=null){
						
						// Are any now cleared?
						
						for(int i=ActiveFloats.Count-1;i>=0;i--){
							
							// Grab the style:
							LayoutBox activeFloat=ActiveFloats[i];
							
							// Is the current render point now higher than this floating object?
							// If so, we must reduce LineStart/ increase MaxX depending on which type of float it is.
							
							if(PenY>=(activeFloat.ParentOffsetTop + activeFloat.Height)){
								
								// Yep! Cleared. Reduce our size:
								if(activeFloat.FloatMode==FloatMode.Right){
									
									if(GoingLeftwards){
										
										// Decrease LineStart:
										LineStart-=activeFloat.Width;
										
									}else{
										
										// Increase max x:
										MaxX+=activeFloat.Width;
										
									}
									
								}else{
									
									if(GoingLeftwards){
										
										// Increase max x:
										MaxX+=activeFloat.Width;
										
									}else{
										
										// Decrease LineStart:
										LineStart-=activeFloat.Width;
										
									}
									
								}
								
								// Remove it as an active float:
								ActiveFloats.RemoveAt(i);
								
							}
							
						}
						
					}
					
				}
				
			}else{
				
				// Update line height:
				if(lineHeight>Parent.LineHeight){
					Parent.LineHeight=lineHeight;
				}
				
				// Apply valid width/height:
				LayoutBox box=CurrentBox;
				
				box.InnerHeight=lineHeight;
				box.InnerWidth=PenX-LineStart;
				box.SetDimensions(false,false);
				
				// Update content w/h:
				box.ContentHeight=box.InnerHeight;
				box.ContentWidth=box.InnerWidth;
				
				// Update dim's:
				Parent.AdvancePen(box);
				
				if(breakLine){
					
					// Linebreak the parent:
					Parent.CompleteLine(breakLine,false);
					
					// Create a new box!
					// (And add it to the parent)
					LayoutBox styleBox=new LayoutBox();
					styleBox.Border=box.Border;
					styleBox.Padding=box.Padding;
					styleBox.Margin=box.Margin;
					styleBox.DisplayMode=box.DisplayMode;
					
					CurrentBox=styleBox;
					
					styleBox.NextInElement=null;
					
					// Add to the inline element's render data:
					RenderData.LastBox.NextInElement=styleBox;
					RenderData.LastBox=styleBox;
					
					styleBox.TEST=true;
					
					// Add to line next:
					Parent.AddToLine(styleBox);
					
				}
				
			}
			
			if(breakLine){
				
				// Finally, reset the pen (this is after the recursion call, so we've cleared floats etc):
				if(this is InlineBoxMeta){
					LineStart=HostBlock.LineStart;
				}
				
				PenX=LineStart;
				
				LineHeight=0;
				Baseline=0;
				
			}
			
		}
		
		/// <summary>Advances the pen now.</summary>
		public void AdvancePen(LayoutBox styleBox){
			
			if(styleBox.FloatMode==FloatMode.Right){
				
				// Float right
				if(GoingLeftwards){
					styleBox.ParentOffsetLeft=LineStart;
					PenX+=styleBox.TotalWidth;
				}else{
					styleBox.ParentOffsetLeft=MaxX-styleBox.TotalWidth;
				}
				
				if(GoingLeftwards){
					
					// Push over where lines start at:
					LineStart+=styleBox.Width;
					
				}else{
					
					// Reduce max:
					MaxX-=styleBox.Width;
					
				}
				
			}else if(styleBox.FloatMode==FloatMode.Left){
				
				// Float left
				if(GoingLeftwards){
					styleBox.ParentOffsetLeft=MaxX-styleBox.TotalWidth;
				}else{
					styleBox.ParentOffsetLeft=LineStart;
					PenX+=styleBox.TotalWidth;
				}
				
				if(GoingLeftwards){
				
					// Reduce max:
					MaxX-=styleBox.Width;
					
				}else{
					
					// Push over where lines start at:
					LineStart+=styleBox.Width;
					
				}
				
				// If it's not the first on the line then..
				if(styleBox!=FirstOnLine){
					
					// Push over all the elements before this on the line.
					LayoutBox currentLine=FirstOnLine;
					
					while(currentLine!=styleBox && currentLine!=null){
						
						if(currentLine.FloatMode==FloatMode.None){
							// Move it:
							currentLine.ParentOffsetLeft+=styleBox.Width;
						}
						
						// Next one:
						currentLine=currentLine.NextOnLine;
						
					}
					
				}
				
			}else if(GoingLeftwards){
				PenX+=styleBox.Width+styleBox.Margin.Right;
				styleBox.ParentOffsetLeft=LineStart*2-PenX;
				PenX+=styleBox.Margin.Left;
				
				float effectiveHeight=styleBox.TotalHeight;
				
				if(effectiveHeight>LineHeight){
					LineHeight=effectiveHeight;
				}
				
			}else{
				PenX+=styleBox.Margin.Left;
				styleBox.ParentOffsetLeft=PenX;
				PenX+=styleBox.Width+styleBox.Margin.Right;
				
				// If it's inline then don't use total height.
				// If it's a word then we don't check it at all.
				float effectiveHeight;
				
				if(styleBox.DisplayMode==DisplayMode.Inline){
					effectiveHeight=styleBox.InnerHeight;
				}else{
					effectiveHeight=styleBox.TotalHeight;
				}
				
				if(effectiveHeight>LineHeight){
					LineHeight=effectiveHeight;
				}
				
			}
			
		}
		
	}
	
	public class BlockBoxMeta : LineBoxMeta{
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		internal float PenY_;
		/// <summary>The point at which lines begin at.</summary>
		internal float LineStart_;
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		internal bool GoingLeftwards_;
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		internal float MaxX_;
		/// <summary>The length of the longest line so far.</summary>
		public float LargestLineWidth_;
		
		
		public BlockBoxMeta(LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData):base(parent,firstBox,renderData){
			
			Parent=parent;
			CurrentBox=firstBox;
			RenderData=renderData;
			
		}
		
		/// <summary>The length of the longest line so far.</summary>
		public override float LargestLineWidth{
			get{
				return LargestLineWidth_;
			}
			set{
				LargestLineWidth_=value;
			}
		}
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		public override float PenY{
			get{
				return PenY_;
			}
			set{
				PenY_=value;
			}
		}
		
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		public override bool GoingLeftwards{
			get{
				return GoingLeftwards_;
			}
			set{
				GoingLeftwards_=value;
			}
		}
		
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		public override float MaxX{
			get{
				return MaxX_;
			}
			set{
				MaxX_=value;
			}
		}
		
		/// <summary>The current font family in use.</summary>
		internal override Css.Units.FontFamilyUnit FontFamily{
			get{
				
				if(FontFamily_==null){
					// Use the default font:
					return Css.Units.FontFamilyUnit.DefaultUnit;
				}
				
				return FontFamily_;
				
			}
			set{
				FontFamily_=value;
			}
		}
		
	}
	
	public class InlineBoxMeta : LineBoxMeta{
		
		public InlineBoxMeta(BlockBoxMeta block,LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData):base(parent,firstBox,renderData){
			HostBlock=block;
			PenX=parent.PenX + firstBox.InlineStyleOffsetLeft;
			LineStart=PenX;
		}
		
	}
	
}