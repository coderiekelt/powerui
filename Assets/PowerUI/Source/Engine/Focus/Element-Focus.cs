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
using Css;
using Dom;
using Nitro;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Provides additional HtmlElement methods for managing focus.
	/// </summary>
	
	public partial class HtmlElement{
		
		/// <summary>True if this element is focusable.</summary>
		public bool focusable{
			get{
				return ( (IsFocusable || this["focusable"]!=null || this["tabindex"]!=null) && (this["disabled"]==null) );
			}
		}
		
		/// <summary>Gets the nearest focusable element above this.</summary>
		/// <returns>The nearest focusable element above. Null if there is none.</returns>
		public HtmlElement GetFocusableAbove(){
			
			// Has the element defined something specific?
			HtmlElement target=GetFocusableOverride("up");
			
			if(target!=null){
				// Yep, it did!
				return target;
			}
			
			// Distance of the nearest element (set when nearest is first set):
			float nearestDistance=0f;
			// The current nearest element:
			HtmlElement nearest=null;
			// Grab my computed style:
			ComputedStyle computed=Style.Computed;
			// Get hold of the iterator (so we can safely skip child elements):
			NodeIterator allElements=document.allNodes;
			
			// Grab my x position:
			float myX=computed.GetMidpointX();
			// Grab the midpoint of this element on Y:
			float myY=computed.GetMidpointY();
			
			
			// For each element in the dom that is focusable and above this..
			foreach(Node node in allElements){
				
				HtmlElement element=node as HtmlElement ;
				
				if(element==null){
					continue;
				}
				
				if(element!=this && element.IsAbove(computed) && element.focusable){
					// We have an element above.
					
					// Check if it is closer than the current result.
					// If it is, it's the current result.
					
					// Is it nearer?
					float distance=element.DistanceFromFast(myX,myY);
					
					// Next, weight the distance by it's verticalness - that's by how much above/below the element actually looks.
					float verticalness=element.VerticalDistanceRatio(myX,myY);
					
					if(verticalness<0.01f){
						verticalness=0.01f;
					}
					
					distance*=verticalness;
					
					// Is it the first we've found, or is it nearer?
					if(nearest==null || distance<nearestDistance){
						nearest=element;
						nearestDistance=distance;
					}
					
					// Make sure we don't now iterate its kids:
					allElements.SkipChildren=true;
				}
			}
			
			return nearest;
		}
		
		/// <summary>Gets the nearest focusable element below this.</summary>
		/// <returns>The nearest focusable element below. Null if there is none.</returns>
		public HtmlElement GetFocusableBelow(){
			
			// Has the element defined something specific?
			HtmlElement target=GetFocusableOverride("down");
			
			if(target!=null){
				// Yep, it did!
				return target;
			}
			
			// Distance of the nearest element (set when nearest is first set):
			float nearestDistance=0f;
			// The current nearest element:
			HtmlElement nearest=null;
			// Grab my computed style:
			ComputedStyle computed=Style.Computed;
			// Get hold of the iterator (so we can safely skip child elements):
			NodeIterator allElements=document.allNodes;
			
			// Grab my x position:
			float myX=computed.GetMidpointX();
			// Grab the midpoint of this element on Y:
			float myY=computed.GetMidpointY();
			
			
			// For each element in the dom that is focusable and below this..
			foreach(Node node in allElements){
				
				HtmlElement element=node as HtmlElement ;
				
				if(element==null){
					continue;
				}
				
				if(element!=this && element.IsBelow(computed) && element.focusable){
					// We have an element below.
					
					// Check if it is closer than the current result.
					// If it is, it's the current result.
					
					// Is it nearer?
					float distance=element.DistanceFromFast(myX,myY);
					
					// Next, weight the distance by it's verticalness - that's by how much above/below the element actually looks.
					float verticalness=element.VerticalDistanceRatio(myX,myY);
					
					if(verticalness<0.01f){
						verticalness=0.01f;
					}
					
					distance*=verticalness;
					
					// Is it the first we've found, or is it nearer?
					if(nearest==null || distance<nearestDistance){
						nearest=element;
						nearestDistance=distance;
					}
					
					// Make sure we don't now iterate its kids:
					allElements.SkipChildren=true;
				}
			}
			
			return nearest;
		}
		
		/// <summary>Gets the nearest focusable element left of this.</summary>
		/// <returns>The nearest focusable element to the left. Null if there is none.</returns>
		public HtmlElement GetFocusableLeft(){
			
			// Has the element defined something specific?
			HtmlElement target=GetFocusableOverride("left");
			
			if(target!=null){
				// Yep, it did!
				return target;
			}
			
			// Distance of the nearest element (set when nearest is first set):
			float nearestDistance=0f;
			// The current nearest element:
			HtmlElement nearest=null;
			// Grab my computed style:
			ComputedStyle computed=Style.Computed;
			// Get hold of the iterator (so we can safely skip child elements):
			NodeIterator allElements=document.allNodes;
			
			// Grab my x position:
			float myX=computed.GetMidpointX();
			// Grab the midpoint of this element on Y:
			float myY=computed.GetMidpointY();
			
			
			// For each element in the dom that is focusable and to the left of this..
			foreach(Node node in allElements){
				
				HtmlElement element=node as HtmlElement ;
				
				if(element==null){
					continue;
				}
				
				if(element!=this && element.IsLeftOf(computed) && element.focusable){
					// We have an element to our left.
					
					// Check if it is closer than the current result.
					// If it is, it's the current result.
					
					// Is it nearer?
					float distance=element.DistanceFromFast(myX,myY);
					
					// Next, weight the distance by it's horizontalness - that's how much to the right/left the element actually looks.
					float horizontalness=element.HorizontalDistanceRatio(myX,myY);
					
					if(horizontalness<0.01f){
						horizontalness=0.01f;
					}
					
					distance*=horizontalness;
					
					// Is it the first we've found, or is it nearer?
					if(nearest==null || distance<nearestDistance){
						nearest=element;
						nearestDistance=distance;
					}
					
					// Make sure we don't now iterate its kids:
					allElements.SkipChildren=true;
				}
			}
			
			return nearest;
		}
		
		/// <summary>Gets the nearest focusable element right of this.</summary>
		/// <returns>The nearest focusable element to the right. Null if there is none.</returns>
		public HtmlElement GetFocusableRight(){
			
			// Has the element defined something specific?
			HtmlElement target=GetFocusableOverride("right");
			
			if(target!=null){
				// Yep, it did!
				return target;
			}
			
			// Distance of the nearest element (set when nearest is first set):
			float nearestDistance=0f;
			// The current nearest element:
			HtmlElement nearest=null;
			// Grab my computed style:
			ComputedStyle computed=Style.Computed;
			// Get hold of the iterator (so we can safely skip child elements):
			NodeIterator allElements=document.allNodes;
			
			// Grab my x position:
			float myX=computed.GetMidpointX();
			// Grab the midpoint of this element on Y:
			float myY=computed.GetMidpointY();
			
			
			// For each element in the dom that is focusable and to the right of this..
			foreach(Node node in allElements){
				
				HtmlElement element=node as HtmlElement ;
				
				if(element==null){
					continue;
				}
				
				if(element!=this && element.IsRightOf(computed) && element.focusable){
					// We have an element to our right.
					
					// Check if it is closer than the current result.
					// If it is, it's the current result.
					
					// Is it nearer?
					float distance=element.DistanceFromFast(myX,myY);
					
					// Next, weight the distance by it's horizontalness - that's how much to the right/left the element actually looks.
					float horizontalness=element.HorizontalDistanceRatio(myX,myY);
					
					if(horizontalness<0.01f){
						horizontalness=0.01f;
					}
					
					distance*=horizontalness;
					
					// Is it the first we've found, or is it nearer?
					if(nearest==null || distance<nearestDistance){
						nearest=element;
						nearestDistance=distance;
					}
					
					// Make sure we don't now iterate its kids:
					allElements.SkipChildren=true;
				}
			}
			
			return nearest;
		}
		
		/// <summary>Checks if this element defines a specific focusable element by id in the given direction.
		/// E.g. it's defined focus-right, focus-left, focus-up, focus-down.</summary>
		/// <param name="direction">The direction to look for an override in.</param>
		/// <returns>The overriding element, if found. Null otherwise.</returns>
		private HtmlElement GetFocusableOverride(string direction){
			string definedTarget=this["focus-"+direction];
			
			if(definedTarget!=null){
				HtmlElement result=document.getElementById(definedTarget) as HtmlElement ;
				
				if(result==null){
					Dom.Log.Add("Warning: HtmlElement with id '"+definedTarget+"' was not found.");
				}else{
					return result;
				}
			}
			
			return null;
		}
		
		/// <summary>Used for tab focus. Gets the next available focusable element.</summary>
		/// <returns>The next available focusable element. Null if there is none.</returns>
		public HtmlElement GetFocusedNext(){
			return null;
		}
		
		/// <summary>Finds out the distance in pixels on the x and y axis the given point is away from this elements midpoint.</summary>
		/// <param name="x">The x coordinate to check from.</param>
		/// <param name="y">The y coordinate to check from.</param>
		/// <returns>The distance on each axis from the given point as a vector.</returns>
		public Vector2 AxisDistanceFrom(float x,float y){
			ComputedStyle computed=Style.Computed;
			
			x-=computed.GetMidpointX();
			y-=computed.GetMidpointY();
			
			if(x<0f){
				x=-x;
			}
			
			if(y<0f){
				y=-y;
			}
			
			return new Vector2(x,y);
		}
		
		/// <summary>Finds the distance on both axis of the given point from this elements midpoint. Then, it divides 
		/// the y result by the x result giving a ratio of 'horizontalness' of the distance. This is used by focus graphs, as it can
		/// be used to perceive how leftward or how rightward an element is.</summary>
		private float HorizontalDistanceRatio(float x,float y){
			ComputedStyle computed=Style.Computed;
			
			// Find the distance along both axis:
			x-=computed.GetMidpointX();
			y-=computed.GetMidpointY();
			
			if(x<0f){
				x=-x;
			}
			
			if(y<0f){
				y=-y;
			}
			
			if(x==0f){
				// Vertical line - it's not horizontal at all!
				return float.MaxValue;
			}
			
			return y/x;
		}
		
		/// <summary>Finds the distance on both axis of the given point from this elements midpoint. Then, it divides 
		/// the x result by the y result giving a ratio of 'verticalness' of the distance. This is used by focus graphs, as it can
		/// be used to perceive how upward or how downward an element is.</summary>
		private float VerticalDistanceRatio(float x,float y){
			ComputedStyle computed=Style.Computed;
			
			// Find the distance along both axis:
			x-=computed.GetMidpointX();
			y-=computed.GetMidpointY();
			
			if(x<0f){
				x=-x;
			}
			
			if(y<0f){
				y=-y;
			}
			
			if(y==0f){
				// Horizontal line - it's not vertical at all!
				return float.MaxValue;
			}
			
			return x/y;
		}
		
		/// <summary>Gets a relative 2D distance of this elements midpoint from the given point.
		/// The value returned is a fast distance used for comparison only. Use DistanceFrom for the correct distance.</summary>
		/// <param name="x">The x coordinate to check from.</param>
		/// <param name="y">The y coordinate to check from.</param>
		public float DistanceFromFast(float x,float y){
			ComputedStyle computed=Style.Computed;
			
			x-=computed.GetMidpointX();
			y-=computed.GetMidpointY();
			
			return ( (x*x) + (y*y) );
		}
		
		/// <summary>Gets the 2D distance of this elements midpoint from the given point.</summary>
		/// <param name="x">The x coordinate to check from.</param>
		/// <param name="y">The y coordinate to check from.</param>
		public float DistanceFrom(float x,float y){
			return Mathf.Sqrt(DistanceFromFast(x,y));
		}
		
		/// <summary>Checks if this element is to the left of the given style.</summary>
		/// <returns>True if this element is to the left of the given style.</returns>
		public bool IsLeftOf(ComputedStyle computed){
			// Check if my left edge is before (but not equal to) the given left edge.
			return (Style.Computed.OffsetLeft < computed.OffsetLeft);
		}
		
		/// <summary>Checks if this element is to the right of the given style.</summary>
		/// <returns>True if this element is to the right of the given style.</returns>
		public bool IsRightOf(ComputedStyle computed){
			// Check if my right edge is after (but not equal to) the given right edge.
			return (Style.Computed.OffsetLeft + Style.Computed.PixelWidth) > (computed.OffsetLeft + computed.PixelWidth);
		}
		
		/// <summary>Checks if this element is above the given style.</summary>
		/// <returns>True if this element is above the given style.</returns>
		public bool IsAbove(ComputedStyle computed){
			// Check if my top edge is less than (but not equal to) the given top edge.
			return (Style.Computed.OffsetTop < computed.OffsetTop);
		}
		
		/// <summary>Checks if this element is below the given style.</summary>
		/// <returns>True if this element is below the given style.</returns>
		public bool IsBelow(ComputedStyle computed){
			// Check if my bottom edge is greater than (but not equal to) the given bottom edge.
			return (Style.Computed.OffsetTop + Style.Computed.PixelHeight) > (computed.OffsetTop + computed.PixelHeight);
		}
		
		/// <summary>The next focusable child element. Entirely ignores tab index.</summary>
		public HtmlElement childFocusable{
			get{
				
				if(focusable){
					return this;
				}
				
				if(childNodes_==null){
					return null;
				}
				
				foreach(Node child in childNodes_){
					
					HtmlElement el=child as HtmlElement ;
					
					if(el==null){
						continue;
					}
					
					HtmlElement focus=el.childFocusable;
					
					if(focus!=null){
						return focus;
					}
					
				}
				
				return null;
				
			}
		}
		
		/// <summary>
		/// Searches for the closest tab index either after this element.
		/// E.g. if search is 1, it will look for the nearest element after this element
		/// with a tab index of 1 or more. BestSoFar is the closest tabIndex found (if any).
		/// An actual match results in this function halting and returning true.
		/// </summary>
		public bool SearchTabIndexBefore(int search,ref int bestSoFar,ref HtmlElement best){
			
			// Must start at the very root of the DOM.
			// Slight hack: set best to this to make it stop when it reaches it.
			best=this;
			htmlDocument.html.SearchChildTabIndex(search,ref bestSoFar,ref best);
			
			return (bestSoFar==search);
			
		}
		
		/// <summary>
		/// Searches for the closest tab index either after this element.
		/// E.g. if search is 1, it will look for the nearest element after this element
		/// with a tab index of 1 or more. BestSoFar is the closest tabIndex found (if any).
		/// An actual match results in this function halting and returning true.
		/// </summary>
		public bool SearchTabIndexAfter(int search,ref int bestSoFar,ref HtmlElement best){
			
			// Current parent:
			HtmlElement parent=ParentNode as HtmlElement ;
			HtmlElement currentRelative=this;
		
			// Go down the parent chain:
			while(parent!=null){
				
				if(parent.SearchTabIndexRelativeTo(currentRelative,false,search,ref bestSoFar,ref best)){
					
					// Found an ideal match - stop.
					return true;
					
				}
				
				currentRelative=parent;
				
				// Next parent:
				parent=parent.parentNode_ as HtmlElement ;
			}
			
			return false;
			
		}
		
		/// <summary>
		/// Searches for the closest tab index either before or after the given relative element.
		/// Note that relative is expected to be a child of this element.
		/// </summary>
		/// <param name='active'>True if you want to search before; false for after.</param>
		private bool SearchTabIndexRelativeTo(HtmlElement relativeTo,bool active,int search,ref int bestSoFar,ref HtmlElement best){
			
			if(childNodes_==null){
				return false;
			}
			
			foreach(Node child in childNodes_){
				
				if(child==relativeTo){
					
					// Found it! Flip the active state:
					active=!active;
					
				}else if(active){
					
					HtmlElement el=child as HtmlElement ;
					
					// Search in this child node:
					if(el!=null && el.SearchChildTabIndex(search,ref bestSoFar,ref best)){
						return true;
					}
					
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>Searches for the closest tab index to the given one. Returns true if it gets a match.</summary>
		public bool SearchChildTabIndex(int search,ref int bestSoFar,ref HtmlElement best){
			
			if(this==best){
				// Terminate
				return true;
			}
			
			if(focusable){
				
				// This element is focusable! Is it's tabIndex suitable?
				int index=tabIndex;
				
				if(index<bestSoFar && index>=search){
					
					// This is the best one so far.
					best=this;
					bestSoFar=index;
					
					if(index==search){
						
						// Direct match - stop!
						return true;
						
					}
					
				}
				
			}
			
			// Any kids?
			if(childNodes_==null){
				return false;
			}
			
			foreach(Node child in childNodes_){
				
				HtmlElement el=child as HtmlElement ;
				
				// Search in it:
				if(el!=null && el.SearchChildTabIndex(search,ref bestSoFar,ref best)){
					return true;
				}
				
			}
			
			return false;
			
		}
		
	}

}