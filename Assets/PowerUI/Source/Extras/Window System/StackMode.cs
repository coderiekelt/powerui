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


namespace Windows{
	
	/// <summary>
	/// How a window deals with other windows of the same type.
	/// E.g. if you have an inventory and a stats window that occupy the same space, one probably closes the other when opened.
	/// </summary>
	public enum StackMode{
		
		/// <summary>Directly stacks the window over the other. This is typical for 'floating' windows.</summary>
		Over,
		/// <summary>The newer window hides the older one. When the newer one is closed, the older one appears again.</summary>
		Hide,
		/// <summary>The newer window closes the older one.</summary>
		Close
		
	}
	
}