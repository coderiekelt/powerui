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

using Css;
using Dom;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A cursor element. Used internally by textarea's/ input elements.
	/// </summary>
	
	[Dom.TagName("cursor")]
	public class HtmlCursorElement:HtmlElement{
		
		/// <summary>The virtual cursor element draws after everything.</summary>
		public const int Priority=VirtualElements.AFTER_ZONE+10;
		
		/// <summary>Text index of the cursor.</summary>
		public int Index;
		/// <summary>True if the cursor should be relocated after the next update.</summary>
		public bool Locate;
		
		
		/// <summary>The host input element. Either a textarea or input.</summary>
		public HtmlElement Input{
			get{
				return parentElement as HtmlElement;
			}
		}
		
		/// <summary>Scrolls the input box if the given position is out of bounds.</summary>
		private void ScrollIfBeyond(ref Vector2 position){
			
			// Scroll input if the cursor is beyond the end of the box:
			ComputedStyle inputCS=Input.Style.Computed;
			
			LayoutBox input=inputCS.FirstBox;
			
			float boxSize=input.InnerWidth;
			float scrollLeft=input.Scroll.Left;
			
			float scrolledPosition=position.x-scrollLeft;
			
			if(scrolledPosition>boxSize){
				
				// Cursor is beyond the right edge.
				// Scroll it such that the cursor is positioned just on that right edge:
				scrollLeft+=(scrolledPosition-boxSize)+3f;
				
			}else if(scrolledPosition<0f){
				
				// Cursor is beyond the left edge.
				// Scroll it such that the cursor is positioned just on that left edge:
				scrollLeft+=scrolledPosition;
				
			}
			
			// Clamp the scrolling:
			if(scrollLeft<0f){
				scrollLeft=0f;
			}
			
			// Update scroll left:
			if(scrollLeft!=input.Scroll.Left){
				
				// For this pass:
				input.Scroll.Left=scrollLeft;
				
				// For future passes:
				inputCS.ChangeTagProperty("scroll-left",scrollLeft+"px");
			}
			
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			// Locate the cursor if we need to:
			if(Locate){
				Locate=false;
				
				HtmlTextNode htn=TextHolder;
				Vector2 position;
				
				if(htn==null){
					
					// Just at 0,0:
					position=Vector2.zero;
					
				}else{
					
					// Clip:
					if(Index>=htn.length){
						Index=htn.length;
					}
					
					// Get the position of the given letter:
					position=TextHolder.GetPosition(Index);
				
				}
				
				// Scroll it if position is out of range:
				ScrollIfBeyond(ref position);
				
				// Set it in for this pass:
				box.Position.Top=position.y;
				box.Position.Left=position.x;
				
				// Write it out:
				Style.Computed.ChangeTagProperty("left",position.x+"px");
				Style.Computed.ChangeTagProperty("top",position.y+"px");
				
			}
			
		}
		
		/// <summary>True if it successfully deleted a selection.</summary>
		public bool TryDeleteSelection(){
			
			if(TextHolder==null){
				return false;
			}
			
			// Got a selection?
			Selection s=htmlDocument.getSelection();
			
			if(s==null || s.focusNode!=TextHolder){	
				return false;
			}
			
			// Selected the text.
			
			// Delete it now:
			string value=Input.value;
			int start=s.anchorOffset;
			int end=s.focusOffset;
			value=value.Substring(0,start)+value.Substring(end,value.Length-end);
			
			// Apply now:
			Input.value=value;
			Move(start,false);
			
			// Remove all ranges:
			s.removeAllRanges();
			
			return true;
			
		}
		
		/// <summary>For text and password inputs, this relocates the cursor to the given index.</summary>
		/// <param name="index">The character index to move the cursor to, starting at 0.</param>
		/// <param name="immediate">True if the cursor should be moved right now.</param>
		public void Move(int index,bool immediate){
			
			if(index<0){
				index=0;
			}
			
			if(index==Index){
				return;
			}
			
			Index=index;
			
			Locate=true;

			if(immediate){
				// We have enough info to place the cursor already.
				// Request a layout.
				RequestLayout();
			}
			
			// Otherwise locating the cursor is delayed until after the new value has been rendered.
			// This is used immediately after we set the value.
		}
		
		/// <summary>The container holding the text.</summary>
		public HtmlTextNode TextHolder{
			get{
				return Input.firstChild as HtmlTextNode;
			}
		}
		
	}
	
}