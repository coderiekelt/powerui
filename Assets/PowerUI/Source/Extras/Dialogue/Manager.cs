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


namespace Dialogue{
	
	/// <summary>
	/// A dialogue manager. Active dialogue operates in these and there is only ever one manager per Document.
	/// </summary>
	
	public class Manager{
		
		/// <summary>The actively displaying card.</summary>
		public Card currentCard;
		/// <summary>The actively displaying train.</summary>
		public Train currentTrain;
		
		
	}
	
}