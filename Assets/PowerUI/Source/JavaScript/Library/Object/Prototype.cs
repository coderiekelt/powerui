using System;
using System.Collections.Generic;
using System.Reflection;
using JavaScript.Compiler;


namespace JavaScript
{
	/// <summary>
	/// Provides functionality common to all JavaScript prototypes.
	/// </summary>
	public partial class Prototype
	{
		
		/// <summary>True if a generic property is available at runtime.</summary>
		public static bool HasProperty(ScriptEngine engine,object thisObj,string property){
			
			// Exists?
			if(thisObj==null){
				throw new NullReferenceException("Attempted to read property '"+property+"' from a null reference.");
			}
			
			// Is it extendable?
			Extendable eo=thisObj as Extendable;
			
			if(eo!=null){
				return eo.GetProperty(property)!=Undefined.Value;
			}
			
			// Get the property:
			return (engine.Prototypes.Get(thisObj.GetType()).GetProperty(property)!=null);
			
		}
		
		/// <summary>Pulls a generic property from the given object at runtime.</summary>
		public static object GetPropertyValue(ScriptEngine engine,object thisObj,string property){
			
			// Exists?
			if(thisObj==null){
				throw new NullReferenceException("Attempted to read property '"+property+"' from a null reference.");
			}
			
			// Is it extendable?
			Extendable eo=thisObj as Extendable;
			
			if(eo!=null){
				return eo.GetProperty(property);
			}
			
			// Get the prototype:
			Prototype proto=engine.Prototypes.Get(thisObj.GetType());
			
			// Get the property:
			PropertyVariable pv=proto.GetProperty(property);
			
			if(pv==null){
				// Undefined:
				return JavaScript.Undefined.Value;
			}
			
			// Get the value:
			return pv.GetValue(thisObj);
			
		}
		
		/// <summary>Sets a generic property on the given object at runtime. Note that this can't create a property.</summary>
		public static void SetPropertyValue(ScriptEngine engine,object thisObj,string property,object value){
			
			// Exists?
			if(thisObj==null){
				throw new NullReferenceException("Attempted to set property '"+property+"' on a null reference.");
			}
			
			// Is it extendable?
			Extendable eo=thisObj as Extendable;
			
			if(eo!=null){
				eo.SetProperty(property,value);
				return;
			}
			
			// Get the prototype:
			Prototype proto=engine.Prototypes.Get(thisObj.GetType());
			
			// Get the property:
			PropertyVariable pv=proto.GetProperty(property);
			
			if(pv==null){
				// Undefined:
				throw new JavaScriptException(engine,"TypeError","Can't set dynamic properties on generic objects.");
			}
			
			// Set the value:
			pv.SetValue(thisObj,value);
			
		}
		
		/// <summary>True if this prototype is static (only occurs with the global scope itself).</summary>
		public bool IsStatic;
		/// <summary>The type name.</summary>
		public string Name;
		/// <summary>Host script engine.</summary>
		public ScriptEngine Engine;
		/// <summary>The inherited prototype.</summary>
		public Prototype BasePrototype;
		/// <summary>
		/// The type builder for this prototype.
		/// </summary>
		public ILTypeBuilder Builder;
		/// <summary>The prototype for the constructor of this instance prototype.</summary>
		public Prototype ConstructorPrototype;
		
		/// <summary>
		/// Used to define the root prototype only.
		/// </summary>
		public Prototype(ScriptEngine engine){
			Engine = engine;
			_Type=typeof(object);
		}
		
		public Prototype(ScriptEngine engine,string name,Prototype baseProto,Type type)
		{
			Engine = engine;
			BasePrototype = baseProto;
			
			if(name==null)
			{
				name=type.Name;
			}
			
			Name=name;
			_Type=type;
			
		}
		
		public Prototype(ScriptEngine engine,string name,Prototype baseProto,bool isStatic,Type type)
		{
			
			Engine = engine;
			BasePrototype = baseProto;
			IsStatic=isStatic;
			Name=name;
			
			var moduleInfo=engine.ModuleInfo;
			
			if(type == null){
				
				Builder=moduleInfo.AllocateType(ref Name, (baseProto!=null) ? baseProto.Type : null, IsStatic);
				
				if(!isStatic){
					
					// Define the default ctr and obtain it:
					AddConstructor( Builder.DefineConstructor() );
					
				}
				
			}else{
				_Type = type;
			}
			
		}
		
		/// <summary>The generated type.</summary>
		private Type _Type;
		
		/// <summary>The type.</summary>
		public Type Type
		{
			get
			{
				
				if(_Type==null)
				{
					return Builder.Type;
				}
				
				return _Type;
			}
		}
		
		/// <summary>Adds the given method as this prototypes constructor.
		/// It's either a MethodBase or a MethodGroup.</summary>
		internal void AddConstructor(object method)
		{
			AddProperty("constructor",method,PropertyAttributes.NonEnumerable);
		}
		
		/// <summary>Generates the type now.</summary>
		public void Complete()
		{
			if(Builder==null)
			{
				return;
			}
			
			_Type=Builder.Close();
			Builder=null;
		}
		
		/// <summary>Invokes OnConstruct directly.</summary>
		public object ConstructInstance(object[] paramSet){
			
			MethodBase resMethod=(OnConstruct as MethodBase);
			System.Reflection.ConstructorInfo ctr=resMethod as System.Reflection.ConstructorInfo;
			
			if(ctr!=null)
			{
				// Actual constructor call:
				return ctr.Invoke(paramSet);
			}
			
			// Ordinary method:
			return resMethod.Invoke(null,paramSet);
			
		}
		
		/// <summary>
		/// Instances something of this type.
		/// </summary>
		public object Instance()
		{
			if(Builder!=null){
				Complete();
			}
			return Activator.CreateInstance(_Type);
		}
		
		/// <summary>
		/// Derive a new prototype from this prototype.
		/// </summary>
		public Prototype Derive()
		{
			
			// Create the new one:
			Prototype proto=new Prototype(Engine,null,null,IsStatic,null);
			
			// Define an internal derived property:
			proto.AddProperty("JS__Internal__Derived",Type,PropertyAttributes.NonEnumerable);
			
			return proto;
			
		}
		
		/// <summary>
		/// Creates a new inheriting prototype from this prototype.
		/// </summary>
		public Prototype Inherit()
		{
			return new Prototype(Engine,null,this,IsStatic,null);
		}
		
		/// <summary>
		/// The constructor for this prototype.
		/// </summary>
		internal ConstructorInfo TypeConstructor{
			get{
				return Constructor as ConstructorInfo;
			}
		}
		
		/// <summary>
		/// The constructor for this prototype.
		/// This is always either a MethodBase or MethodGroup object.
		/// </summary>
		public object Constructor{
			get{
				
				// Get the constructor:
				PropertyVariable property=GetProperty("constructor");
				
				if(property==null)
				{
					if(Engine.Prototypes.ObjectPrototype == this)
					{
						// Object should always have a constructor but just in case!
						// We don't want this to go infinity on us.
						return null;
					}
					
					// Standard global constructor:
					return Engine.Prototypes.ObjectPrototype.Constructor;
				}
				
				return property.ConstantValue;
			}
		}
		
		public PropertyVariable this[string name]{
			get{
				return GetProperty(name);
			}
		}
		
		/// <summary>Enumerates all the values in the given object.</summary>
		public IEnumerable<object> PropertyValues(object obj){
			
			// Got a custom one?
			PropertyVariable pv=GetProperty("PropertyValues");
			
			if(pv==null){
				
				foreach(KeyValuePair<string,PropertyVariable> kvp in Properties){
					
					if(kvp.Value.IsEnumerable){
						
						// Read the value:
						object value=kvp.Value.GetValue(obj);
						
						yield return value;
					}
					
				}
				
			}else{
				
				// Get the value:
				IEnumerable<object> set=pv.GetValue(obj) as IEnumerable<object>;
				
				// Iterate it:
				foreach(object s in set){
					
					yield return s;
					
				}
				
			}
			
		}
		
		/// <summary>Enumerates all properties and accounts for any custom ones if the object defines it.</summary>
		public IEnumerable<string> PropertyNames(object obj){
			
			// Got a custom one?
			PropertyVariable pv=GetProperty("PropertyNames");
			
			if(pv==null){
				
				foreach(KeyValuePair<string,PropertyVariable> kvp in Properties){
					
					if(kvp.Value.IsEnumerable){
						yield return kvp.Key;
					}
					
				}
				
			}else{
				
				// Get the value:
				IEnumerable<string> set=pv.GetValue(obj) as IEnumerable<string>;
				
				// Iterate it:
				foreach(string str in set){
					
					yield return str;
					
				}
				
			}
			
		}
		
		/// <summary>Enumerates all properties and accounts for any custom ones if the object defines it.</summary>
		public IEnumerable<KeyValuePair<string,object>> PropertyPairs(object obj){
			
			// Got a custom one?
			PropertyVariable pv=GetProperty("PropertyPairs");
			
			if(pv==null){
				
				foreach(KeyValuePair<string,PropertyVariable> kvp in Properties){
					
					if(kvp.Value.IsEnumerable){
						// Read the value:
						object value=kvp.Value.GetValue(obj);
						
						yield return new KeyValuePair<string,object>(kvp.Key,value);
					}
					
				}
				
			}else{
				
				// Get the value:
				IEnumerable<KeyValuePair<string,object>> set=pv.GetValue(obj) as IEnumerable<KeyValuePair<string,object>>;
				
				// Iterate it:
				foreach(KeyValuePair<string,object> v in set){
					
					yield return v;
					
				}
				
			}
			
		}
		
		/// <summary>Copies this prototypes values into the given one.</summary>
		public void Merge(Prototype into){
			
			foreach(KeyValuePair<string,PropertyVariable> kvp in Properties){
				
				// Get initial value if there is one:
				object value=kvp.Value.ConstantValue;
				
				if(value==null){
				
					// Add it by type only:
					into.AddProperty(kvp.Key,kvp.Value.Attributes,kvp.Value.Type);
					
				}else{
					
					// Add by value:
					into.AddProperty(kvp.Key,value,kvp.Value.Attributes);
					
				}
				
			}
			
		}
		
		// Properties
		public Dictionary<string, PropertyVariable> Properties;
		
		/// <summary>
		/// Gets the number of properties defined in this object.
		/// </summary>
		[JSProperties(Hidden=true)]
		public int PropertyCount
		{
			get
			{
				if (Properties == null)
				{
					return 0;
				}
				return Properties.Count;
			}
		}
		
		/// <summary>
		/// Used by for..in
		/// </summary>
		public PropertyVariable PropertyNames(){
			
			return GetProperty("___properties");
			
		}
		
		/// <summary>
		/// Used for this[indexerType]
		/// </summary>
		public PropertyVariable Indexer(Type indexerType){
			
			return GetProperty("___item_"+indexerType.Name);
			
		}
		
		/// <summary>
		/// Gets the zero-based index of the property with the given name and the attributes
		/// associated with the property.
		/// </summary>
		/// <param name="name"> The name of the property. </param>
		/// <returns> A structure containing the zero-based index of the property, or <c>-1</c> if a property with the
		/// given name does not exist. </returns>
		public PropertyVariable GetProperty(string name)
		{
			if(Properties==null)
				return null;
			
			PropertyVariable propertyInfo;
			Properties.TryGetValue(name, out propertyInfo);
			return propertyInfo;
		}
		
		/// <summary>
		/// Calls the function with the given name.  The function must exist on this object or an
		/// exception will be thrown.
		/// </summary>
		/// <param name="functionName"> The name of the function to call. </param>
		/// <param name="parameters"> The parameters to pass to the function. </param>
		/// <returns> The result of calling the function. </returns>
		public object CallMemberFunctionOn(object thisObj,string functionName,bool optional, params object[] parameters)
		{
			PropertyVariable function = GetProperty(functionName);
			if (function == null){
				
				if(optional){
					return null;
				}
				
				throw new JavaScriptException(this.Engine, "TypeError", "Object "+ToString()+" has no method '"+functionName+"'");
			}
			
			MethodBase method = function.ConstantValue as MethodBase;
			
			if(method!=null)
			{
				// Invoke it:
				return method.Invoke(thisObj,parameters);
			}
			
			MethodGroup set = function.ConstantValue as MethodGroup;
			
			if(set!=null)
			{
				// Invoke it:
				return set.Invoke(thisObj,parameters);
			}
			
			if(function.Type == typeof(JSFunction)){
				// Try getting the value:
				JSFunction d = function.GetValue(thisObj) as JSFunction;
				
				if(d!=null){
					
					// Invoke it now:
					return d(parameters);
					
				}
				
			}
			
			if(optional){
				return null;
			}
			
			throw new JavaScriptException(this.Engine, "TypeError", "Property '"+functionName+"' of object "+ToString()+" is not a function");
			
		}
		
		/// <summary>True if the given name is on this prototype or inherited ones.</summary>
		public bool HasProperty(string property){
			
			if(Properties!=null){
				if(Properties.ContainsKey(property)){
					return true;
				}
			}
			
			if(BasePrototype!=null){
				return BasePrototype.HasProperty(property);
			}
			
			// Not found.
			return false;
			
		}
		
		/// <summary>True if the given name is on this prototype (and not inherited ones).</summary>
		public bool HasOwnProperty(string property){
			
			if(Properties==null){
				return false;
			}
			
			return Properties.ContainsKey(property);
			
		}
		
		/// <summary>
		/// Gets an enumerable list of every property name and value associated with this object.
		/// Includes properties in the prototype chain.
		/// </summary>
		[JSProperties(Hidden=true)]
		public IEnumerator<string> GetIterator()
		{
			
			Prototype current=this;
			
			while(current!=null){
				
				// Enumerate named properties.
				foreach (KeyValuePair<string,PropertyVariable> pair in current.Properties){
					
					// Can we 'see' this property?
					if(pair.Value.IsEnumerable){
						
						yield return pair.Key;
					
					}
					
				}
				
				current=current.BasePrototype;
				
			}
			
		}
		
		/// <summary>
		/// Gets an enumerable list of every property name and value associated with this object.
		/// Does not include properties in the prototype chain.
		/// </summary>
		[JSProperties(Hidden=true)]
		public IEnumerator<string> GetOwnIterator()
		{
			// Enumerate named properties.
			foreach (KeyValuePair<string,PropertyVariable> pair in Properties){
				
				// Can we 'see' this property?
				if(pair.Value.IsEnumerable){
					
					yield return pair.Key;
				
				}
				
			}
			
		}
		
		/// <summary>
		/// Adds a property to this object.
		/// </summary>
		/// <param name="name"> The name of the property to add. </param>
		/// <param name="attributes"> The property attributes. </param>
		public PropertyVariable AddProperty(string name, object value, PropertyAttributes attributes)
		{
			
			if(Properties==null)
			{
				Properties = new Dictionary<string, PropertyVariable>();
			}
			
			PropertyVariable sp=new PropertyVariable(this, name, value, attributes);
			Properties[name]=sp;
			
			return sp;
		}
		
		/// <summary>
		/// Adds a property to this object, typically generating a field of the given type.
		/// </summary>
		/// <param name="name"> The name of the property to add. </param>
		/// <param name="attributes"> The property attributes. </param>
		public PropertyVariable AddProperty(string name, PropertyAttributes attributes,Type valueType)
		{
			
			if(Properties==null)
			{
				Properties = new Dictionary<string, PropertyVariable>();
			}
			
			PropertyVariable sp=new PropertyVariable(this, name, attributes,valueType);
			Properties[name]=sp;
			
			return sp;
		}
		
		//	 ATTRIBUTE-BASED PROTOTYPE POPULATION
		//_________________________________________________________________________________________
		
		/// <summary>
		/// Populate with everything from the given type.
		/// </summary>
		public void PopulatePrototype(Type type,bool ctrs)
		{
			PopulateFunctions(type,ctrs);
			PopulateFields(type,0);
		}
		
		/// <summary>
		/// Populate with only the static/ constructors.
		/// </summary>
		public void PopulateConstructorPrototype(Type type)
		{
			PopulateStaticFunctions(type,true);
			PopulateFields(type,2);
		}
		
		/// <summary>
		/// Populate with the non-static members.
		/// </summary>
		public void PopulateInstancePrototype(Type type)
		{
			PopulateNonStaticFunctions(type,false);
			PopulateFields(type,1);
		}
		
		/// <summary>
		/// Populates the object with functions by searching a .NET type for methods.
		/// Should be called only once at startup.  Also automatically
		/// populates properties too.
		/// </summary>
		/// <param name="type"> The type to search for methods. </param>
		/// <param name="ctrs"> Populate constructors too.</param>
		private void PopulateFunctions(Type type,bool ctrs)
		{
			PopulateFunctions(type, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly,ctrs,0);
		}
		
		/// <summary>
		/// Used by the prototype for constructors.
		/// E.g. String.aStaticFunction is a function on the String constructor prototype.
		/// </summary>
		/// <param name="type"> The type to search for methods. </param>
		/// <param name="ctrs"> Populate constructors too.</param>
		private void PopulateStaticFunctions(Type type,bool ctrs)
		{
			PopulateFunctions(type, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,ctrs,2);
		}
		
		/// <summary>
		/// Used by instances which have a constructor.
		/// Essentially culls out the functions which were claimed by the constructor.
		/// </summary>
		/// <param name="type"> The type to search for methods. </param>
		/// <param name="ctrs"> Populate constructors too.</param>
		private void PopulateNonStaticFunctions(Type type,bool ctrs)
		{
			PopulateFunctions(type, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly,ctrs,1);
		}
		
		/// <summary>
		///  The value of the special .make property. Found on constructor objects only.
		/// </summary>
		public object OnConstruct
		{
			get{
				// Get the property:
				PropertyVariable property=GetProperty(".make");
				
				if(property==null)
				{
					return null;
				}
				
				return property.ConstantValue;
			}
		}
		
		/// <summary>
		///  The value of the special .call property. Found on constructor objects only.
		/// </summary>
		public object OnCall
		{
			get{
				// Get the property:
				PropertyVariable property=GetProperty(".call");
				
				if(property==null)
				{
					return null;
				}
				
				return property.ConstantValue;
			}
		}
		
		/// <summary>
		/// Adds the given method to the prototype.
		/// </summary>
		private void AddMethod(MethodBase method,Type type,int staticMode)
		{
			
			// Get the properties attribute if it's got one:
			#if NETFX_CORE
			var attribute = (JSProperties)method.GetCustomAttribute(typeof(JSProperties));
			#else
			var attribute = (JSProperties)Attribute.GetCustomAttribute(method, typeof(JSProperties));
			#endif
			if (attribute != null && attribute.Hidden)
				return;

			// Determine the name of the method.
			string name;
			if (attribute!=null && attribute.Name != null)
				name = attribute.Name;
			else
				name = method.Name;
			
			// Reject properties (but not constructors):
			if(method.IsSpecialName && name!=".ctor"){
				return;
			}
			
			ParameterInfo[] mParams=null;
			
			if(method.IsStatic && staticMode!=0)
			{
				// If it's static and it does not have a 'thisObj' property
				// then it's a constructor level static function, IF the object has a constructor at all.
				mParams=method.GetParameters();
				
				if(mParams.Length>0)
				{
					
					if(mParams[0].Name=="thisObj" || (mParams.Length>1 && mParams[1].Name=="thisObj"))
					{
						// Not actually static.
						if(staticMode==2)
						{
							return;
						}
					}else if(staticMode==1)
					{
						return;
					}
					
				}
			}
			
			bool enumerable=true;
			
			if(name=="OnConstruct" || name==".ctor")
			{
				name=".make";
				enumerable=false;
			}
			else if(name=="OnCall")
			{
				name=".call";
				enumerable=false;
			}
			
			// Special case for get_ or set_ methods - we want them to act like properties.
			// Chop off get_ or set_ here (so the actual name entering the proto is correct)
			// then when the user attempts to use it like a property, it checks then.
			int actLikeProperty=0;
			
			if(name.StartsWith("get_")){
				name=name.Substring(4);
				actLikeProperty=1;
			}else if(name.StartsWith("set_")){
				name=name.Substring(4);
				actLikeProperty=2;
			}
			
			// For methods, auto == lowercase.
			if(attribute==null)
			{
				
				name=char.ToLower(name[0])+name.Substring(1);
				
			}else
			{
				
				// For methods, auto == lowercase
				if(attribute.FirstCharacter==CaseMode.Upper)
				{
					name=char.ToUpper(name[0])+name.Substring(1);
				}else if(attribute.FirstCharacter==CaseMode.Lower || attribute.FirstCharacter==CaseMode.Auto)
				{
					name=char.ToLower(name[0])+name.Substring(1);
				}
				
			}
			
			if(actLikeProperty!=0){
				
				if(name=="_Item"){
					
					// Rename! Actually got an indexer here.
					name="___item";
					
					if(mParams==null){
						mParams=method.GetParameters();
					}
					
					// Skip thisObj and engine if they're set:
					for(int i=0;i<mParams.Length;i++){
						
						ParameterInfo p=mParams[i];
						
						if(i<2){
							
							// Skip special parameters.
							
							if(p.Name=="thisObj" || (i==0 && p.ParameterType==typeof(ScriptEngine))){
								continue;
							}
							
						}
						
						// Append the type name:
						name+="_"+p.ParameterType.Name;
						
					}
					
				}else if(name=="_Properties"){
					
					// Rename! Got the property enumerator here.
					name="___properties";
					
				}
				
			}
			
			// Check property attributes.
			PropertyAttributes descriptorAttributes = PropertyAttributes.Sealed;
			if (attribute!=null && attribute.IsEnumerable && enumerable)
				descriptorAttributes |=PropertyAttributes.Enumerable;
			if (attribute==null || attribute.IsConfigurable)
				descriptorAttributes |= PropertyAttributes.Configurable;
			if (attribute==null || attribute.IsWritable)
				descriptorAttributes |= PropertyAttributes.Writable;
				
			// Already defined this property?
			PropertyVariable property=GetProperty(name);
			
			MethodInfo disambig=null;
			
			if(attribute!=null && attribute.Disambiguation!=null){
				// This method is ambiguous. 
				// We've got a special (static) disambiguation method:
				disambig=Type.GetMethod(attribute.Disambiguation);
			}
			
			if(property==null)
			{
				// Add as a single method, optionally as a virtual property:
				object propertyValue;
				
				if(actLikeProperty==0){
					
					if(disambig!=null){
						// Group of 1 is required 
						// (we know more methods will be getting added).
						MethodGroup group=new MethodGroup();
						
						// Apply disambig method:
						group.Disambiguation = disambig;
						
						// Set the attributes:
						group.PropertyAttributes = descriptorAttributes;
						
						// Add the method:
						group.Add(method);
						
						propertyValue=group;
					}else{
						// Straight apply the method:
						propertyValue=method;
					}
					
				}else{
					
					// Create a virtual property:
					VirtualProperty vProperty=new VirtualProperty();
					
					// Apply the method:
					if(actLikeProperty==1){
						vProperty.GetMethod=method;
						vProperty.LoadMeta();
					}else{
						vProperty.SetMethod=method;
					}
					
					// Apply virtual property as the value:
					propertyValue=vProperty;
					
				}
				
				property=AddProperty(name,propertyValue,descriptorAttributes);
				
			}
			else if(actLikeProperty!=0){
				
				// Got the other method in a property.
				VirtualProperty vProperty=property.ConstantValue as VirtualProperty;
				
				// Apply the method:
				if(actLikeProperty==1){
					vProperty.GetMethod=method;
					vProperty.LoadMeta();
				}else{
					vProperty.SetMethod=method;
				}
				
			}else{
				// Already a method set?
				MethodGroup group=property.ConstantValue as MethodGroup;
				
				if(group==null)
				{
					
					// Create a method group and add the current method to it:
					group=new MethodGroup();
					
					// Set the attributes:
					group.PropertyAttributes = descriptorAttributes;
					
					// Add the method:
					group.Add(property.ConstantValue as MethodBase);
					
					// Force the property to change type:
					property.ForceChange(group);
					
				}
				
				if(disambig!=null){
					
					// Apply disambig method:
					group.Disambiguation = disambig;
					
				}
				
				// Add this method to the group:
				group.Add(method);
				
				if (group.PropertyAttributes != descriptorAttributes)
					throw new InvalidOperationException("Inconsistant property attributes detected on "+method+".");
				
			}
			
		}
		
		/// <summary>
		/// Populates the object with functions by searching a .NET type for methods.
		/// Should be called only once at startup.  Also automatically
		/// populates properties too.
		/// </summary>
		/// <param name="type"> The type to search for methods. </param>
		/// <param name="bindingFlags"> The binding flags to use to search for properties and methods. </param>
		private void PopulateFunctions(Type type, BindingFlags bindingFlags,bool constructors,int staticMode)
		{
			if (type == null)
				type = this.GetType();
			
			// Group the methods on the given type by name.
			var methods = type.GetMethods(bindingFlags);
			
			foreach (var method in methods)
			{
				AddMethod(method,type,staticMode);
			}
			
			if(constructors)
			{
				var set = type.GetConstructors();
				
				foreach (var ctr in set)
				{
					AddMethod(ctr,type,staticMode);
				}
			}
			
			PropertyInfo[] properties = type.GetProperties(bindingFlags);
			foreach (PropertyInfo prop in properties)
			{
				#if NETFX_CORE
				var attribute = prop.GetCustomAttribute(typeof(JSProperties), false) as JSProperties;
				#else
				var attribute = Attribute.GetCustomAttribute(prop, typeof(JSProperties), false) as JSProperties;
				#endif
				
				if (attribute != null && attribute.Hidden)
					continue;
			
				// The property name.
				string name;
				if (attribute!=null && attribute.Name != null)
					name = attribute.Name;
				else
					name = prop.Name;
				
				// For properties, auto == lowercase.
				if(attribute==null)
				{
					
					name=char.ToLower(name[0])+name.Substring(1);
					
				}else
				{
					
					// For properties, auto == lowercase
					if(attribute.FirstCharacter==CaseMode.Upper)
					{
						name=char.ToUpper(name[0])+name.Substring(1);
					}else if(attribute.FirstCharacter==CaseMode.Lower || attribute.FirstCharacter==CaseMode.Auto)
					{
						name=char.ToLower(name[0])+name.Substring(1);
					}
					
				}
				
				var getMethod = prop.GetGetMethod(true);
				
				if(getMethod.IsStatic)
				{
					// If it's static then it's a constructor level function (if there is a constructor).
					
					if(staticMode==1)
					{
						// We only want non-static ones.
						continue;
					}
				}else if(staticMode==2)
				{
					// We only want static ones.
					continue;
				}
				
				// Handle this[indexer] next:
				ParameterInfo[] indexParams=prop.GetIndexParameters();
				
				if(indexParams!=null && indexParams.Length>0){
					// Add to the name:
					
					name="___item";
					
					for(int i=0;i<indexParams.Length;i++){
						name+="_"+indexParams[i].ParameterType.Name;
					}
					
				}
				
				// Define the property.
				AddProperty(name, prop, PropertyAttributes.FullAccess);
			}
		}

		/// <summary>
		/// Populates the object with properties by searching a .NET type for fields.
		/// Should be called only once at startup.
		/// </summary>
		private void PopulateFields()
		{
			PopulateFields(null,0);
		}
		
		/// <summary>
		/// Populates the object with properties by searching a .NET type for fields.
		/// Should be called only once at startup.
		/// </summary>
		/// <param name="type"> The type to search for fields. </param>
		private void PopulateFields(Type type,int staticMode)
		{
			if (type == null){
				type = this.GetType();
			}
			
			// Get the js properties for the type:
			#if NETFX_CORE
			JSProperties typeAttribute = (JSProperties)type.GetTypeInfo().GetCustomAttribute(typeof(JSProperties));
			#else
			JSProperties typeAttribute = (JSProperties)Attribute.GetCustomAttribute(type, typeof(JSProperties));
			#endif
			
			CaseMode typeCaseMode=CaseMode.Auto;
			
			if(typeAttribute!=null){
				typeCaseMode=typeAttribute.FirstFieldCharacter;
			}
			
			// Find all fields with [JSProperties]
			foreach (var field in type.GetFields()){
				
				#if NETFX_CORE
				var attribute = (JSProperties)field.GetCustomAttribute(typeof(JSProperties));
				#else
				var attribute = (JSProperties)Attribute.GetCustomAttribute(field, typeof(JSProperties));
				#endif
				
				if (attribute != null && attribute.Hidden){
					continue;
				}
				
				CaseMode caseMode=CaseMode.Auto;
				
				if(attribute!=null){
					// Got settings; apply case mode:
					caseMode=attribute.FirstCharacter;
				}
				
				if(caseMode==CaseMode.Auto){
					caseMode=typeCaseMode;
				}
				
				string name;
				if (attribute!=null && attribute.Name != null){
					name = attribute.Name;
				}else{
					name = field.Name;
				}
				
				if(field.IsStatic){
					// If it's static then it's a constructor level function (if there is a constructor).
					
					if(staticMode==1){
						// We only want non-static ones.
						continue;
					}
				}else if(staticMode==2){
					// We only want static ones.
					continue;
				}
				
				// auto == static/const fields are as-is;
				// instance fields are lowercased.
				if(caseMode==CaseMode.Auto){
					if(field.IsStatic){
						caseMode=CaseMode.Unchanged;
					}else{
						caseMode=CaseMode.Lower;
					}
				}
				
				if(caseMode==CaseMode.Upper){
					name=char.ToUpper(name[0])+name.Substring(1);
				}else if(caseMode==CaseMode.Lower){
					name=char.ToLower(name[0])+name.Substring(1);
				}
				
				if(field.IsLiteral && !field.IsInitOnly){
					AddProperty(name,field.GetValue(null),PropertyAttributes.FullAccess);
				}else{
					AddProperty(name,field,PropertyAttributes.FullAccess);
				}
			}
		}
		
	}
}
