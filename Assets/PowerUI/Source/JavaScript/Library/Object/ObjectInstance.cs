using System;
using System.Collections.Generic;
using System.Reflection;


namespace JavaScript
{
	/// <summary>
	/// Provides functionality common to all objects.
	/// </summary>
	[JSProperties(Name="Object")]
	public partial class ObjectInstance
	{
		
		[JSProperties(Hidden=true)]
		public static readonly Type PrototypeFor=typeof(object);
		
		/*
		/// <summary>
		/// The constructor for this prototype.
		/// </summary>
		internal MethodBase Constructor{
			get{
				return this["constructor"] as MethodBase;
			}
		}
		
		/// <summary>
		/// The prototype being used by this object.
		/// </summary>
		public Prototype __proto__{
			get{
				return this.prototype;
			}
		}
		
		/// <summary>
		/// Gets the next object in the prototype chain.  There is no corresponding property in
		/// javascript (it is is *not* the same as the prototype property), instead use
		/// Object.getPrototypeOf().
		/// </summary>
		[JSProperties(Hidden=true)]
		public Prototype Prototype
		{
			get { return this.prototype; }
		}
		
		/// <summary>
		/// Gets or sets the value of a named property.
		/// </summary>
		/// <param name="propertyName"> The name of the property to get or set. </param>
		/// <returns> The property value, or <c>null</c> if the property doesn't exist. </returns>
		[JSProperties(Hidden=true)]
		public object this[string propertyName]
		{
			get { return GetPropertyValue(propertyName); }
			set { SetPropertyValue(propertyName, value, false); }
		}

		/// <summary>
		/// Gets or sets the value of an array-indexed property.
		/// </summary>
		/// <param name="index"> The index of the property to retrieve. </param>
		/// <returns> The property value, or <c>null</c> if the property doesn't exist. </returns>
		[JSProperties(Hidden=true)]
		public object this[uint index]
		{
			get { return GetPropertyValue(index); }
			set { SetPropertyValue(index, value, false); }
		}

		/// <summary>
		/// Gets or sets the value of an array-indexed property.
		/// </summary>
		/// <param name="index"> The index of the property to retrieve. </param>
		/// <returns> The property value, or <c>null</c> if the property doesn't exist. </returns>
		[JSProperties(Hidden=true)]
		public object this[int index]
		{
			get
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException("index");
				return GetPropertyValue((uint)index);
			}
			set
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException("index");
				SetPropertyValue((uint)index, value, false);
			}
		}
		
		/// <summary>
		/// Deletes the property with the given array index.
		/// </summary>
		/// <param name="index"> The array index of the property to delete. </param>
		/// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
		/// be set because the property was marked as non-configurable.  </param>
		/// <returns> <c>true</c> if the property was successfully deleted, or if the property did
		/// not exist; <c>false</c> if the property was marked as non-configurable and
		/// <paramref name="throwOnError"/> was <c>false</c>. </returns>
		[JSProperties(Hidden=true)]
		public virtual bool Delete(uint index, bool throwOnError)
		{
			string indexStr = index.ToString();

			// Retrieve the attributes for the property.
			var propertyInfo = GetPropertyIndexAndAttributes(indexStr);
			if (propertyInfo.Exists == false)
				return true;	// Property doesn't exist - delete succeeded!

			// Delete the property.
			if (properties==null || !properties.Remove(indexStr)){
				throw new InvalidOperationException("The property '"+indexStr+"' does not exist.");
			}
			
			InlineCacheKey++;
			return true;
		}
		
		/// <summary>
		/// Deletes the property with the given name.
		/// </summary>
		/// <param name="propertyName"> The name of the property to delete. </param>
		/// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
		/// be set because the property was marked as non-configurable.  </param>
		/// <returns> <c>true</c> if the property was successfully deleted, or if the property did
		/// not exist; <c>false</c> if the property was marked as non-configurable and
		/// <paramref name="throwOnError"/> was <c>false</c>. </returns>
		[JSProperties(Hidden=true)]
		public bool Delete(string propertyName, bool throwOnError)
		{
			// Check if the property is an indexed property.
			uint arrayIndex = JSArray.ParseArrayIndex(propertyName);
			if (arrayIndex != uint.MaxValue)
				return Delete(arrayIndex, throwOnError);

			// Retrieve the attributes for the property.
			var propertyInfo = GetPropertyIndexAndAttributes(propertyName);
			if (propertyInfo.Exists == false)
				return true;	// Property doesn't exist - delete succeeded!

			// Check if the property can be deleted.
			if (propertyInfo.IsConfigurable == false)
			{
				if (throwOnError == true)
					throw new JavaScriptException(this.Engine, "TypeError", string.Format("The property '{0}' cannot be deleted.", propertyName));
				return false;
			}

			// Delete the property.
			if (properties==null || !properties.Remove(propertyName)){
				throw new InvalidOperationException("The property '"+propertyName+"' does not exist.");
			}
			
			InlineCacheKey++;
			return true;
		}
		
		/// <summary>
		/// Calls the function with the given name.  The function must exist on this object or an
		/// exception will be thrown.
		/// </summary>
		/// <param name="functionName"> The name of the function to call. </param>
		/// <param name="parameters"> The parameters to pass to the function. </param>
		/// <returns> The result of calling the function. </returns>
		internal object CallMemberFunctionOn(object thisObj,string functionName, params object[] parameters)
		{
			var function = GetPropertyValue(functionName);
			if (function == null)
				throw new JavaScriptException(this.Engine, "TypeError", string.Format("Object {0} has no method '{1}'", this.ToString(), functionName));
			if ((function is FunctionInstance) == false)
				throw new JavaScriptException(this.Engine, "TypeError", string.Format("Property '{1}' of object {0} is not a function", this.ToString(), functionName));
			return ((FunctionInstance)function).CallLateBound(Engine,thisObj, parameters);
		}
		
		/// <summary>
		/// Calls the function with the given name.  The function must exist on this object or an
		/// exception will be thrown.
		/// </summary>
		/// <param name="functionName"> The name of the function to call. </param>
		/// <param name="parameters"> The parameters to pass to the function. </param>
		/// <returns> The result of calling the function. </returns>
		internal object CallMemberFunction(string functionName, params object[] parameters)
		{
			var function = GetPropertyValue(functionName);
			if (function == null)
				throw new JavaScriptException(this.Engine, "TypeError", string.Format("Object {0} has no method '{1}'", this.ToString(), functionName));
			if ((function is FunctionInstance) == false)
				throw new JavaScriptException(this.Engine, "TypeError", string.Format("Property '{1}' of object {0} is not a function", this.ToString(), functionName));
			return ((FunctionInstance)function).CallLateBound(Engine,this, parameters);
		}
		
		/// <summary>
		/// Calls the function with the given name.
		/// </summary>
		/// <param name="result"> The result of calling the function. </param>
		/// <param name="functionName"> The name of the function to call. </param>
		/// <param name="parameters"> The parameters to pass to the function. </param>
		/// <returns> <c>true</c> if the function was called successfully; <c>false</c> otherwise. </returns>
		internal bool TryCallMemberFunction(out object result, string functionName, params object[] parameters)
		{
			var function = GetPropertyValue(functionName);
			if ((function is FunctionInstance) == false)
			{
				result = null;
				return false;
			}
			result = ((FunctionInstance)function).CallLateBound(Engine,this, parameters);
			return true;
		}
		*/
		
		//	 JAVASCRIPT FUNCTIONS
		//_________________________________________________________________________________________

		/// <summary>
		/// Determines if a property with the given name exists on this object.
		/// </summary>
		/// <param name="engine"> The associated script engine. </param>
		/// <param name="thisObj"> The object that is being operated on. </param>
		/// <param name="propertyName"> The name of the property. </param>
		/// <returns> <c>true</c> if a property with the given name exists on this object,
		/// <c>false</c> otherwise. </returns>
		/// <remarks> Objects in the prototype chain are not considered. </remarks>
		public static bool HasOwnProperty(ScriptEngine engine, object thisObj, string propertyName)
		{
			Extendable eo=(thisObj as Extendable);
			
			if(eo!=null){
				// Check an extendable object:
				return eo.GetProperty(propertyName)!=Undefined.Value;
			}
			
			return engine.Prototypes.Get(thisObj.GetType())[propertyName]!=null;
		}

		/// <summary>
		/// Determines if this object is in the prototype chain of the given object.
		/// </summary>
		/// <param name="engine"> The associated script engine. </param>
		/// <param name="thisObj"> The object that is being operated on. </param>
		/// <param name="obj"> The object to check. </param>
		/// <returns> <c>true</c> if this object is in the prototype chain of the given object;
		/// <c>false</c> otherwise. </returns>
		public static bool IsPrototypeOf(ScriptEngine engine, object thisObj, object obj)
		{
			var obj2 = obj as Prototype;
			
			while (obj2!=null){
				if (obj2 == thisObj)
					return true;
				obj2 = obj2.BasePrototype;
			}
			
			return false;
		}

		/// <summary>
		/// Determines if a property with the given name exists on this object and is enumerable.
		/// </summary>
		/// <param name="engine"> The associated script engine. </param>
		/// <param name="thisObj"> The object that is being operated on. </param>
		/// <param name="propertyName"> The name of the property. </param>
		/// <returns> <c>true</c> if a property with the given name exists on this object and is
		/// enumerable, <c>false</c> otherwise. </returns>
		/// <remarks> Objects in the prototype chain are not considered. </remarks>
		public static bool PropertyIsEnumerable(ScriptEngine engine, object thisObj, string propertyName)
		{
			var property = engine.Prototypes.Get(thisObj.GetType()).GetProperty(propertyName);
			return property!=null && property.IsEnumerable;
		}

		/// <summary>
		/// Returns a locale-dependant string representing the current object.
		/// </summary>
		/// <returns> Returns a locale-dependant string representing the current object. </returns>
		public static string ToLocaleString(ScriptEngine engine, object thisObj)
		{
			// Get the prototype:
			Prototype proto=engine.Prototypes.Get(thisObj.GetType());
			
			return TypeConverter.ToString(proto.CallMemberFunctionOn(thisObj,"toString",false));
		}
		
		/// <summary>
		/// Returns a string representing the current object.
		/// </summary>
		/// <param name="engine"> The current script environment. </param>
		/// <param name="thisObj"> The value of the "this" keyword. </param>
		/// <returns> A string representing the current object. </returns>
		public static string ToString(ScriptEngine engine, object thisObj)
		{
			if (thisObj == null)
				return "[object Null]";
			if (thisObj == Undefined.Value)
				return "[object Undefined]";
			return "[object "+engine.Prototypes.Get(thisObj.GetType()).Name+"]";
		}
		
		/// <summary>
		/// Creates a new Object instance.
		/// </summary>
		public static Extendable OnConstruct(ScriptEngine engine)
		{
			return new Extendable(new object(),engine.Prototypes.ObjectPrototype);
		}
		
		//	 JAVASCRIPT FUNCTIONS
		//_________________________________________________________________________________________
		
		/// <summary>
		/// Retrieves the next object in the prototype chain for the given object.
		/// </summary>
		/// <param name="obj"> The object to retrieve the prototype from. </param>
		/// <returns> The next object in the prototype chain for the given object, or <c>null</c>
		/// if the object has no prototype chain. </returns>
		
		public static Prototype GetPrototypeOf(ScriptEngine engine,object obj)
		{
			return engine.Prototypes.Get(obj);
		}
		
		/// <summary>
		/// Gets an object that contains details of the property with the given name.
		/// </summary>
		/// <param name="obj"> The object to retrieve property details for. </param>
		/// <param name="propertyName"> The name of the property to retrieve details for. </param>
		/// <returns> An object containing some of the following properties: configurable,
		/// writable, enumerable, value, get and set. </returns>
		public static PropertyVariable GetOwnPropertyDescriptor(ScriptEngine engine,object obj, string propertyName)
		{
			Prototype proto=engine.Prototypes.Get(obj);
			return proto.GetProperty(propertyName);
		}
		
		/// <summary>
		/// Creates an array containing the names of all the properties on the object (even the
		/// non-enumerable ones).
		/// </summary>
		/// <param name="obj"> The object to retrieve the property names for. </param>
		/// <returns> An array containing the names of all the properties on the object. </returns>
		public static List<string> GetOwnPropertyNames(ScriptEngine engine,object obj){
			Prototype proto=engine.Prototypes.Get(obj);
			
			List<string> result = new List<string>();
			
			foreach (string property in proto.PropertyNames(obj)){
				result.Add(property);
			}
			
			return result;
		}

		/// <summary>
		/// Creates an object with the given prototype and, optionally, a set of properties.
		/// </summary>
		/// <param name="engine">  </param>
		/// <param name="prototype"> A reference to the next object in the prototype chain for the
		/// created object. </param>
		/// <param name="properties"> An object containing one or more property descriptors. </param>
		/// <returns> A new object instance. </returns>
		
		public static object Create(ScriptEngine engine, object prototype, object properties)
		{	
			Prototype proto=(prototype as Prototype);
			
			if(proto==null){
				// Get the prototype for the given object:
				proto=engine.Prototypes.Get(prototype);
			}
			
			if(proto==null){
				proto=engine.Prototypes.Get(typeof(object));
			}
			
			// Instance it:
			object result=proto.Instance();
			
			if (properties != null)
				DefineProperties(engine, result, properties);
			
			return result;
		}

		/// <summary>
		/// Modifies the value and attributes of a property.
		/// </summary>
		/// <param name="obj"> The object to define the property on. </param>
		/// <param name="propertyName"> The name of the property to modify. </param>
		/// <param name="attributes"> A property descriptor containing some of the following
		/// properties: configurable, writable, enumerable, value, get and set. </param>
		/// <returns> The object with the property. </returns>
		
		public static object DefineProperty(ScriptEngine engine, object obj, string propertyName, object attributes)
		{	
			Extendable eo=obj as Extendable;
			
			if(eo==null){
				throw new JavaScriptException(engine, "TypeError", "Cannot define properties on immutable objects.");
			}
			
			eo.SetProperty(propertyName, obj);
			return eo;
		}
		
		/// <summary>
		/// Modifies multiple properties on an object.
		/// </summary>
		/// <param name="obj"> The object to define the properties on. </param>
		/// <param name="properties"> An object containing one or more property descriptors. </param>
		/// <returns> The object with the properties. </returns>
		
		public static object DefineProperties(ScriptEngine engine, object obj, object properties){
			
			Extendable eo=obj as Extendable;
			
			if(eo==null){
				throw new JavaScriptException(engine, "TypeError", "Cannot define properties on immutable objects.");
			}
			
			Prototype props=engine.Prototypes.Get(properties);
			
			foreach(KeyValuePair<string,object> property in props.PropertyPairs(obj)){
				eo.SetProperty(property.Key, property.Value);
			}
			
			return obj;
			
		}

		/// <summary>
		/// Prevents the addition or deletion of any properties on the given object.
		/// </summary>
		/// <param name="obj"> The object to modify. </param>
		/// <returns> The object that was affected. </returns>
		
		public static object Seal(object obj){
			Extendable eo=(obj as Extendable);	
			if(eo!=null){
				eo.Frozen=true;
			}
			return obj;
		}
		
		/// <summary>
		/// Prevents the addition, deletion or modification of any properties on the given object.
		/// </summary>
		/// <param name="obj"> The object to modify. </param>
		/// <returns> The object that was affected. </returns>
		
		public static object Freeze(object obj){
			Extendable eo=(obj as Extendable);	
			if(eo!=null){
				eo.Frozen=true;
			}
			return obj;
		}
		
		/// <summary>
		/// Prevents the addition of any properties on the given object.
		/// </summary>
		/// <param name="obj"> The object to modify. </param>
		/// <returns> The object that was affected. </returns>
		
		public static object PreventExtensions(object obj){
			return Freeze(obj);
		}
		
		/// <summary>
		/// Determines if addition or deletion of any properties on the object is allowed.
		/// </summary>
		/// <param name="obj"> The object to check. </param>
		/// <returns> <c>true</c> if properties can be added or at least one property can be
		/// deleted; <c>false</c> otherwise. </returns>
		
		public static bool IsSealed(object obj){
			Extendable eo=(obj as Extendable);
			return (eo!=null && eo.Frozen);
		}
		
		/// <summary>
		/// Determines if addition, deletion or modification of any properties on the object is
		/// allowed.
		/// </summary>
		/// <param name="obj"> The object to check. </param>
		/// <returns> <c>true</c> if properties can be added or at least one property can be
		/// deleted or modified; <c>false</c> otherwise. </returns>
		public static bool IsFrozen(object obj){
			Extendable eo=(obj as Extendable);
			return (eo!=null && eo.Frozen);
		}
		
		/// <summary>
		/// Determines if addition of properties on the object is allowed.
		/// </summary>
		/// <param name="obj"> The object to check. </param>
		/// <returns> <c>true</c> if properties can be added to the object; <c>false</c> otherwise. </returns>
		
		[JSProperties(Name="isExtensible")]
		public static bool IsExtensibleG(object obj)
		{
			return obj is Extendable;
		}
		
		/// <summary>
		/// Creates an array containing the names of all the enumerable properties on the object.
		/// </summary>
		/// <param name="obj"> The object to retrieve the property names for. </param>
		/// <returns> An array containing the names of all the enumerable properties on the object. </returns>
		public static List<string> Keys(ScriptEngine engine, object obj)
		{
			return GetOwnPropertyNames(engine,obj);
		}

		/// <summary>
		/// Determines whether two values are the same value.  Note that this method considers NaN
		/// to be equal with itself and negative zero is considered different from positive zero.
		/// </summary>
		/// <param name="value1"> The first value to compare. </param>
		/// <param name="value2"> The second value to compare. </param>
		/// <returns> <c>true</c> if the values are the same.  </returns>
		
		public static bool Is(object value1, object value2)
		{
			return TypeComparer.SameValue(value1, value2);
		}
		
	}
}
