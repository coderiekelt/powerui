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
using PowerUI;
using ContextMenus;
using System.Text;


namespace Windows{
	
	/// <summary>
	/// A no frills context menu which just lists out the options in a white box.
	/// </summary>
	
	[Dom.TagName("menulist")]
	public class MenuList : ContextMenuWindow{
		
		public override void BuildOption(StringBuilder builder,Option option){
			
			// - option.mouseRef provides the onclick and optindex attributes.
			//   It's equal to.. optindex='option.index' onclick='OptionList.ResolveOptionFromClick'
			
			// If you want to have a totally custom resolver, OptionList provides a few other helpers for you.
			
			// - Markup is whatever the user provided to display for the option.
			// - Option is a partial class; extend it if you want to pass additional stuff.
			
			// Just a basic div:
			builder.Append("<div "+option.mouseRef+">"+option.markup+"</div>");
			
		}
		
	}
	
}