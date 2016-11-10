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
		public float LineHeight_;
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
		
		/// <summary>The current x location of the renderer in screen pixels from the left.</summary>
		public virtual float PenX{
			get{
				return HostBlock.PenX_;
			}
			set{
				HostBlock.PenX_=value;
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
		
		/// <summary>The height of the current line being processed.</summary>
		public virtual float LineHeight{
			get{
				return LineHeight_;
			}
			set{
				LineHeight_=value;
			}
		}
		
		/// <summary>The point at which lines begin at.</summary>
		public virtual float LineStart{
			get{
				return HostBlock.LineStart_;
			}
			set{
				HostBlock.LineStart_=value;
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
		
		/// <summary>The position of the text baseline.</summary>
		public virtual float Baseline{
			get{
				return HostBlock.Baseline_;
			}
			set{
				HostBlock.Baseline_=value;
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
		internal void AddToLine(LayoutBox styleBox,RenderableData parentNode){
			
			if((styleBox.PositionMode & PositionMode.InFlow)==0){
				// Out of flow - margins only:
				styleBox.ParentOffsetLeft=PenX+styleBox.Margin.Left;
				styleBox.ParentOffsetTop=PenY+styleBox.Margin.Top;
				return;
			}
			
			// Don't call with inline elements - block or inline-block only.
			LayoutBox parentBox=(parentNode==null)?null:parentNode.FirstBox;
			
			if( (styleBox.DisplayMode==DisplayMode.Block && styleBox.FloatMode==FloatMode.None) ){
				
				// Break before a block element.
				CompleteLine();
				
			}
			
			styleBox.NextPacked=null;
			styleBox.NextOnLine=null;
			
			if(styleBox.FloatMode==FloatMode.Right){
				
				if(GoingLeftwards){
					styleBox.ParentOffsetLeft=LineStart;
					PenX+=styleBox.TotalWidth;
				}else{
					styleBox.ParentOffsetLeft=MaxX-styleBox.TotalWidth;
				}
				
				if(ActiveFloats==null){
					ActiveFloats=new List<LayoutBox>(1);
				}
				
				ActiveFloats.Add(styleBox);
				
			}else if(styleBox.FloatMode==FloatMode.Left){
				
				if(GoingLeftwards){
					styleBox.ParentOffsetLeft=MaxX-styleBox.TotalWidth;
				}else{
					styleBox.ParentOffsetLeft=LineStart;
					PenX+=styleBox.TotalWidth;
				}
				
				if(ActiveFloats==null){
					ActiveFloats=new List<LayoutBox>(1);
				}
				
				ActiveFloats.Add(styleBox);
				
			}else if(GoingLeftwards){
				PenX+=styleBox.Width+styleBox.Margin.Right;
				styleBox.ParentOffsetLeft=LineStart*2-PenX;
				PenX+=styleBox.Margin.Left;
			}else{
				PenX+=styleBox.Margin.Left;
				styleBox.ParentOffsetLeft=PenX;
				PenX+=styleBox.Width+styleBox.Margin.Right;
			}
			
			if(styleBox.FloatMode==FloatMode.Left){
				
				if(GoingLeftwards){
				
					// Reduce max:
					MaxX-=styleBox.Width;
					
				}else{
					
					// Push over where lines start at:
					LineStart+=styleBox.Width;
					
				}
				
			}else if(styleBox.FloatMode==FloatMode.Right){
				
				if(GoingLeftwards){
					
					// Push over where lines start at:
					LineStart+=styleBox.Width;
					
				}else{
					
					// Reduce max:
					MaxX-=styleBox.Width;
					
				}
				
			}else{
				
				float effectiveHeight=styleBox.TotalHeight;
				
				if(effectiveHeight>LineHeight){
					LineHeight=effectiveHeight;
				}
				
			}
			
			if(FirstPacked==null){
				FirstPacked=LastPacked=styleBox;
			}else{
				LastPacked=LastPacked.NextPacked=styleBox;
			}
			
			if(FirstOnLine==null){
				FirstOnLine=LastOnLine=styleBox;
			}else{
				
				if(styleBox.FloatMode==FloatMode.Left){
					
					// Push over all the elements before this on the line.
					LayoutBox currentLine=FirstOnLine;
					
					while(currentLine!=null){
						
						if(currentLine.FloatMode==FloatMode.None){
							// Move it:
							currentLine.ParentOffsetLeft+=styleBox.Width;
						}
						
						// Next one:
						currentLine=currentLine.NextOnLine;
						
					}
					
				}
				
				LastOnLine=LastOnLine.NextOnLine=styleBox;
			}
			
			if(styleBox.DisplayMode==DisplayMode.Block && styleBox.FloatMode==FloatMode.None){
				
				// A second break after the block too.
				CompleteLine();
				
			}
			
		}
		
		/// <summary>Lets the renderer know the current line doesn't fit anymore elements
		/// and has been finished.</summary>
		public void CompleteLine(){
			
			if(PenX>LargestLineWidth){
				LargestLineWidth=PenX;
			}
			
			// Get the line and clear it out. zone.CompleteLine may add new inline boxes to the next line:
			LayoutBox currentBox=FirstOnLine;
			FirstOnLine=null;
			LastOnLine=null;
			
			CompleteLine(true);
			
			// Next, apply their top offset.
			float lineHeight=LineHeight;
			
			while(currentBox!=null){
				// Calculate the offset to where the top left corner is (of the complete box, margin included):
				
				// Must be positioned such that the boxes sit on this lines baseline.
				// the baseline is by default at half the line-height but moves up whenever 
				// an inline-block element with padding/border/margin is added.
				
				float delta=lineHeight-(currentBox.Height+currentBox.Margin.Bottom);
				
				currentBox.ParentOffsetTop=PenY+delta;
				
				// Hop to the next one:
				currentBox=currentBox.NextOnLine;
			}
			
			// Move the pen down to the following line:
			PenY+=lineHeight;
			
			if(ActiveFloats!=null){
				
				// Are any now going to be "deactivated"?
				
				for(int i=ActiveFloats.Count-1;i>=0;i--){
					
					// Grab the style:
					LayoutBox activeFloat=ActiveFloats[i];
					
					// Is the current render point now higher than this floating object?
					// If so, we must reduce LineStart/ increase MaxX depending on which type of float it is.
					
					if(PenY>=(activeFloat.ParentOffsetTop + activeFloat.Height)){
						
						// Yep! Deactivate and reduce our size:
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
			
			LineHeight=0;
			Baseline=0;
			PenX=LineStart;
			
		}
		
		public void CompleteLine(bool breakLine){
			
			InlineBoxMeta inline=this as InlineBoxMeta;
			float penX=PenX;
			
			while(inline!=null){
				
				// Update line height:
				if(inline.LineHeight_>inline.Parent.LineHeight){
					inline.Parent.LineHeight=inline.LineHeight_;
				}
				
				// Apply valid width/height:
				LayoutBox box=inline.CurrentBox;
				
				box.InnerHeight=inline.LineHeight_;
				box.InnerWidth=penX-inline.StartPenX;
				box.SetDimensions(false,false);
				
				// Update content h/w:
				box.ContentHeight=box.InnerHeight;
				box.ContentWidth=box.InnerWidth;
				
				if(breakLine){
					
					// Time to create a new box!
					// (And add it to the inline element this represents)
					LayoutBox styleBox=new LayoutBox();
					styleBox.Border=box.Border;
					styleBox.Padding=box.Padding;
					styleBox.Margin=box.Margin;
					styleBox.DisplayMode=box.DisplayMode;
					
					inline.CurrentBox=styleBox;
					
					RenderableData rd=inline.RenderData;
					LayoutBox cb=inline.CurrentBox;
					cb.NextInElement=null;
					
					if(rd.FirstBox==null){
						rd.FirstBox=cb;
						rd.LastBox=cb;
					}else{
						rd.LastBox.NextInElement=cb;
						rd.LastBox=cb;
					}
					
					// Add to line next:
					styleBox.NextPacked=null;
					styleBox.NextOnLine=null;
					
					if(GoingLeftwards){
						styleBox.ParentOffsetLeft=LineStart;
					}else{
						styleBox.ParentOffsetLeft=LineStart;
					}
					
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
					
					// End of the element only - don't bother going up the DOM.
					break;
					
				}
				
				// Next:
				inline=inline.Parent as InlineBoxMeta;
				
			}
			
		}
		
	}
	
	public class BlockBoxMeta : LineBoxMeta{
		
		/// <summary>The current x location of the renderer in screen pixels from the left.</summary>
		internal float PenX_;
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		internal float PenY_;
		/// <summary>The point at which lines begin at.</summary>
		internal float LineStart_;
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		internal bool GoingLeftwards_;
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		internal float MaxX_;
		/// <summary>The position of the text baseline.</summary>
		internal float Baseline_;
		/// <summary>The length of the longest line so far.</summary>
		internal float LargestLineWidth_;
		
		/// <summary>The current x location of the renderer in screen pixels from the left.</summary>
		public override float PenX{
			get{
				return PenX_;
			}
			set{
				PenX_=value;
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
		
		/// <summary>The height of the current line being processed.</summary>
		public override float LineHeight{
			get{
				return LineHeight_;
			}
			set{
				LineHeight_=value;
			}
		}
		
		/// <summary>The point at which lines begin at.</summary>
		public override float LineStart{
			get{
				return LineStart_;
			}
			set{
				LineStart_=value;
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
		
		/// <summary>The position of the text baseline.</summary>
		public override float Baseline{
			get{
				return Baseline_;
			}
			set{
				Baseline_=value;
			}
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
		
		/// <summary>The starting X coordinate.</summary>
		public float StartPenX;
		/// <summary>The current inline box.</summary>
		internal LayoutBox CurrentBox;
		/// <summary>The next box in the hierarchy.</summary>
		public LineBoxMeta Parent;
		/// <summary>The inline element.</summary>
		public RenderableData RenderData;
		
		
		public InlineBoxMeta(BlockBoxMeta block,LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData){
			HostBlock=block;
			Parent=parent;
			StartPenX=block.PenX;
			CurrentBox=firstBox;
			RenderData=renderData;
		}
		
	}
	
}