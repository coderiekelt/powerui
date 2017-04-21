using System;
using System.Collections.Generic;
using System.Text;

namespace JavaScript{

	/// <summary>
	/// A remote local is one which originated from some other scope.
	/// They're created when a local is pulled into an anonymous function.
	/// If that anonymous function updates the local's value then it's updated *everywhere* it was used.
	/// Thus an object is used at a slight extra cost.
	/// </summary>
	public class RemoteLocal<T>{
		
		/// <summary>The actual value.</summary>
		public T Value;
		
	}

}
