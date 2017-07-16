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


namespace JavaScript{
	
	/// <summary>
	/// The default and very permissive security domain.
	/// </summary>
	
	public class AllSecurityDomain:SecurityDomain{
		
		public AllSecurityDomain(){
			// Anything in the UI is ok:
			AddReference(".PowerUI");
			// Any http is also ok:
			AddReference(".PowerUI.Http");
			// First dot tells it to use 'this' assembly and the PowerUI namespace.
			// Any Css is ok:
			AddReference(".Css");
			// JSON too:
			AddReference(".Json");
			// Dom:
			AddReference(".Dom");
			// Windows and PowerSlide:
			AddReference(".Windows");
			AddReference(".PowerSlide");
			// Include the Unity classes by default:
			AddReference("UnityEngine.UnityEngine");
			// Include collections by default:
			AddReference("mscorlib.System.Collections");
			// Include generic classes by default:
			AddReference("mscorlib.System.Collections.Generic");
			
			AllowEverything();
		}
		
	}

}