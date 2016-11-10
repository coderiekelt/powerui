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
using Nitro;


namespace PowerUI{
	
	/// <summary>
	/// Manages the currently focused element for this document.
	/// </summary>
	
	public partial class HtmlDocument{
		
		/// <summary>Current focused element casted to a HtmlElement (generally you should use activeElement instead).</summary>
		public HtmlElement htmlActiveElement{
			get{
				return activeElement as HtmlElement ;
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element above.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-up="anElementID" to override which element will be focused next.</summary>
		public void MoveFocusUp(){
			if(htmlActiveElement==null){
				return;
			}
			
			// Grab the element above:
			HtmlElement element=htmlActiveElement.GetFocusableAbove();
			
			if(element!=null){
				// Focus it:
				element.focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element below.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-down="anElementID" to override which element will be focused next.</summary>
		public void MoveFocusDown(){
			if(htmlActiveElement==null){
				return;
			}
			
			// Grab the element below:
			HtmlElement element=htmlActiveElement.GetFocusableBelow();
			
			if(element!=null){
				// Focus it:
				element.focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element to the left.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-left="anElementID" to override which element will be focused next.</summary>
		public void MoveFocusLeft(){
			if(htmlActiveElement==null){
				return;
			}
			
			// Grab the element to the left:
			HtmlElement element=htmlActiveElement.GetFocusableLeft();
			
			if(element!=null){
				// Focus it:
				element.focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element to the right.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-right="anElementID" to override which element will be focused next.</summary>
		public void MoveFocusRight(){
			if(htmlActiveElement==null){
				return;
			}
			
			// Grab the element to the right:
			HtmlElement element=htmlActiveElement.GetFocusableRight();
			
			if(element!=null){
				// Focus it:
				element.focus();
			}
		}
		
		/// <summary>If there is an element focused, this will click it (mouse down and up are triggered).</summary>
		public void ClickFocused(){
			
			HtmlElement focused=htmlActiveElement;
			
			if(focused==null){
				return;
			}
			
			// Click it:
			focused.click();
			
		}
		
		/// <summary>Moves the focus to the next element as defined by tabindex. If there is no currently focused element,
		/// a tabindex of 0 will be searched for. If that's not found, then the first focusable element in the DOM will be used.</summary>
		public void TabNext(){
			
			// - From the current focused element, we check all elements after it to see which
			//   has the 'closest' tabIndex to currentIndex.
			// - If there are no immediate matches, we wrap around the DOM and continue searching
			// - If there is no current focused element, we start at very top and don't wrap at all.
			
			// These track the current best found element.
			int bestSoFar=int.MaxValue;
			HtmlElement best=null;
			
			// Get the current focused element:
			HtmlElement focused=htmlActiveElement;
			
			if(focused==null){
				
				// Haven't got one - hunt for anything:
				UI.document.body.SearchChildTabIndex(0,ref bestSoFar,ref best);
				
			}else{
				
				// Get the current index:
				int currentIndex=(focused==null)?0:focused.tabIndex;
				
				// Hunt after focused, then before it.
				if(!focused.SearchTabIndexAfter(currentIndex,ref bestSoFar,ref best)){
					
					// No perfect match found after - wrap around and try before:
					focused.SearchTabIndexBefore(currentIndex,ref bestSoFar,ref best);
				
				}
				
			}
			
			if(best!=null){
				// Focus it now:
				best.focus();
			}
			
		}
		
	}
	
}