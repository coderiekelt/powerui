using System;
using System.Collections;
using System.Collections.Generic;


namespace JavaScript{
	
	/// <summary>
	/// A dynamically extendable object.
	/// Used when attempting to runtime extend a 'built in' (or precompiled) object.
	/// </summary>
	public class Extendable{
		
		/// <summary>True if it's frozen.</summary>
		public bool Frozen;
		/// <summary>The object that is being extended.</summary>
		public object Extended;
		/// <summary>The prototype of the object being extended.</summary>
		private Prototype OriginalPrototype;
		/// <summary>Its additional properties.</summary>
		private Dictionary<string,object> Properties_;
		
		
		/// <summary>Creates an extendable version of the given object.</summary>
		public Extendable(object ext,Prototype hostProto){
			OriginalPrototype=hostProto;
			Extended=ext;
		}
		
		/// <summary>Gets a property. Undefined if it doesn't exist.</summary>
		public object GetProperty(string property){
			
			object v=null;
			
			if(Properties_==null || !Properties_.TryGetValue(property,out v)){
				
				// Try the proto:
				PropertyVariable pv=OriginalPrototype.GetProperty(property);
				
				if(pv!=null){
					
					// Read the value:
					return pv.GetValue(Extended);
					
				}
				
				// Undefined property:
				return Undefined.Value;
				
			}
			
			return v;
		}
		
		/// <summary>Sets a property.</summary>
		public void SetProperty(string property,object value){
		
			// Check the prototype first:
			PropertyVariable pv=OriginalPrototype.GetProperty(property);
			
			if(pv!=null){
				
				pv.SetValue(Extended,value);
				return;
				
			}
			
			if(Properties_==null){
				Properties_=new Dictionary<string,object>();
			}
			
			Properties_[property]=value;
		}
		
		public IEnumerable<KeyValuePair<string,object>> PropertyPairs{
			get{
				if(Properties_!=null){
					
					foreach(KeyValuePair<string,object> kvp in Properties_){
							
						yield return kvp;
						
					}
					
				}
				
				foreach(KeyValuePair<string,object> o in OriginalPrototype.PropertyPairs(Extended)){
					yield return o;
				}
			}
		}
		
		public IEnumerable<string> PropertyNames{
			get{
				if(Properties_!=null){
					
					foreach(KeyValuePair<string,object> kvp in Properties_){
							
						yield return kvp.Key;
						
					}
					
				}
				
				foreach(string o in OriginalPrototype.PropertyNames(Extended)){
					yield return o;
				}
			}
		}
		
		public IEnumerable<object> PropertyValues{
			get{
				if(Properties_!=null){
					foreach(KeyValuePair<string,object> kvp in Properties_){
						yield return kvp.Value;
					}
				}
				
				foreach(object o in OriginalPrototype.PropertyValues(Extended)){
					yield return o;
				}
			}
		}
		
		/// <summary>(Will be) called by the delete keyword.</summary>
		public void Delete(string property){
			
			// Todo! Call this.
			
			// If it's on the base prototype then we add 'undefined' to the properties set.
			PropertyVariable pv=OriginalPrototype.GetProperty(property);
			
			if(pv!=null){
				
				if(Properties_==null){
					Properties_=new Dictionary<string,object>();
				}
				
				Properties_[property]=Undefined.Value;
				return;
				
			}
			
			// Otherwise, remove from the properties set:
			if(Properties_!=null){
				
				// Remove the property:
				Properties_.Remove(property);
				
				if(Properties_.Count==0){
					Properties_=null;
				}
				
			}
			
		}
		
		public override int GetHashCode(){
			return Extended.GetHashCode();
		}
		
		public override bool Equals(object x){
			
			// Compare references:
			if(x==this || x==Extended){
				return true;
			}
			
			return false;
			
		}
		
	}
	
}