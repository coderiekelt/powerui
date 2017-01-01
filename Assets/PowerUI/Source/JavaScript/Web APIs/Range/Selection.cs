//--------------------------------------
//			   PowerUI
//
//		For documentation or 
//	if you have any issues, visit
//		powerUI.kulestar.com
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Dom;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// The rather unusual text selection API.
	/// </summary>
	
	public partial class Selection{
		
		/// <suummary>All ranges (oddly, it's required to be exactly 1, but addRange exists)</summary>
		public List<Range> Ranges=new List<Range>();
		
		/// <summary>Returns the Node in which the selection ends.</summary>
		public Node focusNode{
			get{
				if(Ranges.Count==0){
					return null;
				}
				
				return Ranges[0].endContainer;
			}
		}
		
		/// <summary>Returns the number of ranges in the selection</summary>
		public int rangeCount{
			get{
				return Ranges.Count;
			}
		}
		
		/// <summary>Returns the Node in which the selection starts.</summary>
		public Node anchorNode{
			get{
				if(Ranges.Count==0){
					return null;
				}
				
				return Ranges[0].startContainer;
			}
		}
		
		/// <summary>Returns a number representing the offset of the selection's anchor within the anchorNode.
		/// If anchorNode is a text node, this is the number of characters within anchorNode preceding the anchor.
		/// If anchorNode is an element, this is the number of child nodes of the anchorNode preceding the anchor.</summary>
		public int anchorOffset{
			get{
				if(Ranges.Count==0){
					return 0;
				}
				
				return Ranges[0].startOffset;
			}
		}
		
		/// <summary>Returns a number representing the offset of the selection's anchor within the focusNode.
		/// If focusNode is a text node, this is the number of characters within focusNode preceding the focus.
		/// If focusNode is an element, this is the number of child nodes of the focusNode preceding the focus.</summary>
		public int focusOffset{
			get{
				if(Ranges.Count==0){
					return 0;
				}
				
				return Ranges[0].endOffset;
			}
		}
		
		/// <summary>Returns a Boolean indicating whether the selection's start and end points are at the same position.</summary>
		public bool isCollapsed{
			get{
				if(Ranges.Count==0){
					return true;
				}
				
				return Ranges[0].isCollapsed;
			}
		}
		
		/// <summary>Removes a range from the selection.</summary>
		public void removeRange(Range r){
			
			if(Ranges.Remove(r)){
				UpdateSelection(false,r);
			}
			
		}
		
		/// <summary>Adds a range which selects all the kids of the given node.</summary>
		public void selectAllChildren(Node node){
			
			// Create range and add it:
			Range r=new Range();
			r.endContainer=node;
			r.startContainer=node;
			r.endOffset=node.childCount;
			
			addRange(r);
			
		}
		
		/// <summary>Selects/ Deselects a whole range.</summary>
		internal void UpdateSelection(bool select,Range r){
			
			if(r.startContainer==null){
				return;
			}
			
			// Get as text node:
			HtmlTextNode htn=(r.startContainer as HtmlTextNode);
			
			if(htn==null){
				// Can't select this at the moment.
				Dom.Log.Add("Note: Attempted to select something that isn't text. If you want this to work, let us know!");
				return;
			}
			
			// Text selection from r.startOffset -> r.endOffset
			
			// Get the selection renderer:
			SelectionRenderingProperty srp=htn.RenderData.GetProperty(
				typeof(SelectionRenderingProperty)
			) as SelectionRenderingProperty;
			
			if(select){
				
				// Create if it doesn't exist:
				if(srp==null){
					
					// Create:
					srp=new SelectionRenderingProperty(htn.RenderData);
					srp.Text=htn.RenderData.Text;
					
					// Add it:
					htn.RenderData.AddOrReplaceProperty(srp,typeof(SelectionRenderingProperty));
					
				}
				
				// Update range:
				if(r.endOffset<r.startOffset){
					
					// Dragged upwards:
					srp.StartIndex=r.endOffset;
					srp.EndIndex=r.startOffset;
					
				}else{
					
					srp.StartIndex=r.startOffset;
					srp.EndIndex=r.endOffset;
					
				}
				
				// Layout:
				srp.RequestLayout();
				
			}else if(srp!=null){
				
				// Remove:
				htn.RenderData.AddOrReplaceProperty(null,typeof(SelectionRenderingProperty));
				
				// Layout:
				srp.RequestLayout();
				
			}
			
		}
		
		/// <summary>Removes all ranges from the selection.</summary>
		public void removeAllRanges(){
			
			for(int i=0;i<Ranges.Count;i++){
				UpdateSelection(false,Ranges[i]);
			}
			
			Ranges.Clear();
		}
		
		/// <summary>Adds a range to the selection.</summary>
		public void addRange(Range r){
			Ranges.Add(r);
			UpdateSelection(true,r);
		}
		
		/// <summary>A range object representing one of the ranges currently selected.</summary>
		public Range getRangeAt(int index){
			
			if(index<0 || index>=Ranges.Count){
				return null;
			}
			
			return Ranges[index];
		}
		
		/// <summary>The currently selected text.</summary>
		public override string ToString(){
			
			if(Ranges.Count==0){
				return "";
			}
			
			return Ranges[0].ToString();
		}
		
		/// <summary>Collapses the selection to the start of the range.</summary>
		public void collapseToStart(){
			collapse(true);
		}
		
		/// <summary>Collapses the selection to the end of the range.</summary>
		public void collapseToEnd(){
			collapse(false);
		}
		
		/// <summary>Collapses the selection to the start or end of the range.</summary>
		public void collapse(bool toStart){
			
			for(int i=0;i<Ranges.Count;i++){
				Range r=Ranges[i];
				UpdateSelection(false,r);
				r.collapse(toStart);
				UpdateSelection(true,r);
			}
			
		}
		
		/// <summary>Deletes the selection's content from the document.</summary>
		public void deleteFromDocument(){
			
			for(int i=0;i<Ranges.Count;i++){
				Range r=Ranges[i];
				UpdateSelection(false,r);
				r.deleteContents();
			}
			
			Ranges.Clear();
			
		}
		
		/// <summary>Indicates if a certain node is part of the selection.</summary>
		public bool containsNode(Node n){
			
			for(int i=0;i<Ranges.Count;i++){
				
				if( Ranges[i].contains(n) ){
					return true;
				}
				
			}
			
			return false;
			
		}
		
	}
	
	public partial class Window{
		
		/// <summary>The current selection. Use the standard getSelection() method instead.</summary>
		internal Selection CurrentSelection;
		
		/// <summary>Creates a range.</summary>
		public Selection getSelection(){
			
			if(CurrentSelection==null){
				CurrentSelection=new Selection();
			}
			
			return CurrentSelection;
		}
		
	}
	
	public partial class HtmlDocument{
		
		/// <summary>Clears any selection.</summary>
		internal void clearSelection(){
			
			Selection s=window.CurrentSelection;
			
			if(s!=null && s.Ranges.Count>0){
				s.removeAllRanges();
			}
			
		}
		
		/// <summary>The current selection.</summary>
		public Selection getSelection(){
			return window.getSelection();
		}
		
	}
	
}