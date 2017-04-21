using System;
using System.Collections.Generic;
using System.Reflection;
using JavaScript.Compiler;


namespace JavaScript
{

	/// <summary>
	/// Represents the information stored about a property in the class schema.
	/// </summary>
	public class PropertyVariable : Variable
	{
		/// <param name="index"> The index of the property in the
		/// <see cref="ObjectInstance.InlinePropertyValues"/> array. </param>
		/// <param name="attributes"> The property attributes.  These attributes describe how the
		/// property can be modified. </param>
		public PropertyVariable(Prototype proto,string name, object value, PropertyAttributes attributes)
			: base(name)
		{
			Prototype = proto;
			Attributes = attributes;
			
			if(value!=null){
				ConstantValue = value;
				Type_ = value.GetType();
				Resolve();
			}
			
		}
		
		public PropertyVariable(Prototype proto,string name, PropertyAttributes attributes, Type valueType)
			: base(name)
		{
			Prototype = proto;
			Attributes = attributes;
			
			Type_ = valueType;
			Resolve();
			
		}
		
		/// <summary>
		/// The type of the index value to use. E.g. the type of 'index' in this[index].
		/// Typically int32.
		/// </summary>
		public Type FirstIndexType{
			get{
				
				// Get the parameters:
				ParameterInfo[] parameters=Property.GetMethod.GetParameters();
				
				// It's the first one after our special 'engine' and 'thisObj' params (if they're set):
				int offset=0;
				
				if(parameters[0].ParameterType==typeof(ScriptEngine)){
					offset++;
				}
				
				if(parameters[offset].Name=="thisObj"){
					offset++;
				}
				
				return parameters[offset].ParameterType;
				
			}
		}
		
		/// <summary>
		/// True if this is a reference to a type.
		/// </summary>
		public bool TypeReference;
		
		/// <summary>
		/// The prototype this belongs to.
		/// </summary>
		public Prototype Prototype;
		
		/// <summary>
		/// The field this is imported from, if any.
		/// </summary>
		public FieldInfo ImportedFrom;
		
		/// <summary>
		/// The default value as a method, if it is one. Null otherwise.
		/// </summary>
		public MethodBase Method
		{
			get{
				return ConstantValue as MethodBase;
			}
		}
		
		/// <summary>
		/// The FieldInfo, if it is one.
		/// </summary>
		public FieldInfo GetField(){
			return Field_;
		}
		
		/// <summary>
		/// The property if it is one.
		/// </summary>
		private VirtualProperty Property;
		
		/// <summary>
		/// The field if it is one.
		/// </summary>
		private FieldInfo Field_;
		
		/// <summary>
		/// The type of this property.
		/// </summary>
		private Type Type_;
		
		/// <summary>
		/// The value type of this property.
		/// </summary>
		public override Type Type
		{
			get{
				return Type_;
			}
			set{
				
				if(value==Type_){
					return;
				}
				
				if(Property!=null){
					// Can't change a fixed property type.
					return;
				}
				
				if(Type_==null || Type_==typeof(JavaScript.Undefined)){
					Type_=value;
				}else if(Type_==typeof(object)){
					// Already collapsed.
					return;
				#if NETFX_CORE
				}else if(!Type_.GetTypeInfo().IsAssignableFrom(value.GetTypeInfo())){
				#else
				}else if(!Type_.IsAssignableFrom(value)){
				#endif
					// The variable has collapsed.
					Type_=typeof(object);
				}
				
			}
		}
		
		/// <summary>True if the accessor variable needs to be emitted too.</summary>
		public bool HasAccessor{
			get{
				if(Property==null && Field_==null)
				{
					Resolve();
				}
				
				// Note that if neither is set, this is a constant (includes method references).
				return ((Property!=null && Property.RequiresThis) || (Field_!=null && !Field_.IsStatic));
			}
		}
		
		/// <summary>True if it has an 'engine' parameter. Applies with get/set methods only.</summary>
		public bool HasEngine{
			get{
				if(Property==null && Field_==null)
				{
					Resolve();
				}
				
				return (Property!=null && Property.RequiresEngine);
			}
		}
		
		/// <summary>Is this a static property?</summary>
		private bool IsStatic{
			get{
				if(Property==null && Field_==null)
				{
					Resolve();
				}
				
				// Either the property or field is static:
				return ((Property!=null && Property.GetMethod.IsStatic) || (Field_!=null && Field_.IsStatic));
			}
		}
		
		/// <summary>
		///	Determines if this is a field or method.
		/// </summary>
		private void Resolve(){
			
			// Property=null;
			// Field_=null;
			
			#if NETFX_CORE
			if(typeof(PropertyInfo).GetTypeInfo().IsAssignableFrom(Type_.GetTypeInfo()))
			#else
			if(typeof(PropertyInfo).IsAssignableFrom(Type_))
			#endif
			{
				
				// It's a property!
				PropertyInfo pI=(PropertyInfo)ConstantValue;
				
				Property=new VirtualProperty( pI );
				Type_=pI.PropertyType;
			
			#if NETFX_CORE
			}else if(typeof(VirtualProperty).GetTypeInfo().IsAssignableFrom(Type_.GetTypeInfo()))
			#else
			}else if(typeof(VirtualProperty).IsAssignableFrom(Type_))
			#endif
			{
				
				// It's a property!
				Property=(VirtualProperty)ConstantValue;
				Type_=Property.PropertyType;
				
			}
			#if NETFX_CORE
			else if(typeof(FieldInfo).GetTypeInfo().IsAssignableFrom(Type_.GetTypeInfo()))
			#else
			else if(typeof(FieldInfo).IsAssignableFrom(Type_))
			#endif
			{
				
				// It's a field!
				Field_=(FieldInfo)ConstantValue;
				Type_=Field_.FieldType;
			}
			#if NETFX_CORE
			else if(typeof(MethodBase).GetTypeInfo().IsAssignableFrom(Type_.GetTypeInfo()) || Type_==typeof(MethodGroup))
			#else
			else if(typeof(MethodBase).IsAssignableFrom(Type_) || Type_==typeof(MethodGroup))
			#endif
			{
				// It's a method - do nothing here; the prototype will store the reference for us.
			}
			else if(Prototype.Builder!=null && Type_!=null)
			{
				FieldAttributes attribs=FieldAttributes.Public;
				
				// Is it static?
				if(Prototype.IsStatic){
					attribs |= FieldAttributes.Static;
				}
				
				// Declare a new field to store it.
				Field_=Prototype.Builder.DefineField(Name,Type_,attribs);
				
			}
			
		}
		
		/// <summary>
		/// Sets the value of this property on the given object.
		/// Note that the object must be an instance of 'this' prototype.
		/// </summary>
		public void SetValue(object thisObj,object newValue){
			
			if(ImportedFrom!=null){
				// 'This' comes from the imported field:
				thisObj=ImportedFrom.GetValue(null);
			}
			
			if(Property==null && Field_==null)
			{
				Type=newValue.GetType();
				Resolve();
			}
			
			if(Property!=null)
			{
				
				// Call the set method:
				Property.SetMethod.Invoke(thisObj,new object[]{newValue});
				return;
				
			}
			
			if(Field_!=null)
			{
				// Set the field (used by globals only):
				
				if(Field_.GetType()!=typeof(FieldInfo)){
					// This occurs when we've got a fieldbuilder.
					// In order to set it, we must re-resolve the field 
					// by obtaining it from the built type.
					ReloadField();
				}
				
				Field_.SetValue(thisObj,newValue);
				return;
			}
			
		}
		
		/// <summary>
		/// Gets the value of this property from the given object.
		/// Note that the object must be an instance of 'this' prototype.
		/// </summary>
		public object GetValue(object thisObj)
		{
			
			if(ImportedFrom!=null){
				// 'This' comes from the imported field:
				thisObj=ImportedFrom.GetValue(null);
			}
			
			if(Property==null && Field_==null)
			{
				Resolve();
			}
			
			if(Property!=null)
			{
				
				// Call the get method:
				return Property.GetMethod.Invoke(thisObj,null);
				
			}
			
			if(Field_!=null)
			{
				// Load the field (used by globals only):
				
				if(Field_.GetType()!=typeof(FieldInfo)){
					// This occurs when we've got a fieldbuilder.
					// In order to set it, we must re-resolve the field by obtaining it from the built type.
					ReloadField();
				}
				
				return Field_.GetValue(thisObj);
			}
			
			return null;
			
		}
		
		/// <summary>Reloads the Field from being a fieldBuilder to a full FieldInfo object.</summary>
		private void ReloadField(){
			
			// Grab the fieldInfo by name:
			Field_=Prototype.Type.GetField(Field_.Name);
			
		}
		
		/// <summary>Called when users of this property must recompile.</summary>
		private void RequireRecompile(){
		
		}
		
		/// <summary>Sets a new method. This triggers call sites to compile as jump tables.</summary>
		public void OverwriteMethod(MethodBase newMethod)
		{
			
			// Is it already a jump table?
			MethodGroup group=ConstantValue as MethodGroup;
			
			if(group==null)
			{
				// Create a group:
				group=new MethodGroup();
				
				// Mark it as a jump table:
				group.JumpTable=true;
				
				// Add the current value:
				group.Add(Method);
				
				// Change the value:
				ConstantValue=group;
			}
			
			// Add the method:
			group.Add(newMethod);
			
			// Which functions use it? Inform them of the collapse.
			RequireRecompile();
			
		}
		
		/// <summary>Force the variable to change type.
		/// Note that this is only ever used during prototype construction.
		/// Any other use will likely have unintended results.</summary>
		public void ForceChange(object value)
		{
			Field_=null;
			ConstantValue=value;
			Type_=value.GetType();
		}
		
		/// <summary>
		/// Gets a boolean value indicating whether the property value will be included during an
		/// enumeration.
		/// </summary>
		public bool IsEnumerable
		{
			get { return (this.Attributes & PropertyAttributes.Enumerable) != 0; }
		}

		/// <summary>
		/// Gets a boolean value indicating whether the property can be deleted.
		/// </summary>
		public bool IsConfigurable
		{
			get { return (this.Attributes & PropertyAttributes.Configurable) != 0; }
		}
		
		/// <summary>
		/// Gets a value that indicates whether the value is computed using accessor functions.
		/// </summary>
		public bool IsAccessor
		{
			get { return (this.Attributes & PropertyAttributes.IsAccessorProperty) != 0; }
		}
		
		
		internal override Type Get(ILGenerator generator)
		{
			
			if(Property==null && Field_==null)
			{
				Resolve();
			}
			
			if(Property!=null)
			{
				
				if(ImportedFrom!=null){
					// Load that:
					generator.LoadField(ImportedFrom);
				}
				
				// Call the get method:
				generator.Call(Property.GetMethod);
				return Type_;
				
			}
			
			if(Field_!=null)
			{
				
				if(ImportedFrom!=null){
					// Load that:
					generator.LoadField(ImportedFrom);
				}
				
				// Load the field:
				generator.LoadField(Field_);
			}
			else if(IsConstant)
			{
				// Emit the default constant value:
				EmitHelpers.EmitValue(generator,ConstantValue);
			}
			else
			{
				// Emit undefined:
				EmitHelpers.EmitUndefined(generator);
			}
			
			return Type_;
			
		}
		
		internal override void Set(ILGenerator generator, OptimizationInfo optimizationInfo,bool rIU, Type valueType, SetValueMethod value)
		{
			
			// Resolve if needed:
			if(Field_==null && Property==null){
				Type=valueType;
				Resolve();
			}else{
				
				// Update type, potentially forcing the variable to change type
				// (unless it's a fixed property, in which case the value being set changes type instead):
				if(Property==null){
					
					// Fixed property - var changes type:
					Type=valueType;
					
				}
				
			}
			
			ILLocalVariable localVar=null;
			
			if(ImportedFrom!=null){
				// Load that:
				generator.LoadField(ImportedFrom);
			}
			
			// Is the return value of this set in use?
			// E.g. var x=obj.x=14;
			if(rIU){
				
				// It's in use and 'obj.x' is not static.
				
				// Output the value twice:
				value(true);
				
				// If it's not static then we now have [obj][value][value] on the stack.
				// Calling the set method would fail (as it'll operate on the duplicated value).
				// So, we have to pop one off and re-add it after.
				
				if(!IsStatic){
					
					// Note that if it's static, no 'obj' reference goes onto the stack anyway.
					
					// At this point, we have our target object followed by two copies of the value.
					// For the following Call/StoreField to work correctly, we must store the 2nd copy into a local.
					localVar=generator.DeclareVariable(valueType);
					
					// Store into the local:
					generator.StoreVariable(localVar);
					
				}
				
			}else{
				
				// Output the value just once:
				value(false);
				
			}
			
			if(Type_!=valueType){
				
				// Convert if needed:
				EmitConversion.Convert(generator,valueType, Type_,optimizationInfo);
			}
			
			if(Property!=null)
			{
				
				// Call the set method:
				generator.Call(Property.SetMethod);
				
			}
			else
			{
				
				// Load the field:
				generator.StoreField(Field_);
				
			}
			
			if(localVar!=null){
				
				// Reload the 2nd copy of the value:
				generator.LoadVariable(localVar);
				
			}
			
		}
		
	}

}
