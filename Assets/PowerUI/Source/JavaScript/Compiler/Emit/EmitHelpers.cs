using System;
using JavaScript;


namespace JavaScript.Compiler
{
	/// <summary>
	/// Outputs IL for misc tasks.
	/// </summary>
	internal static class EmitHelpers
	{
		/// <summary>
		/// Emits undefined.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		public static void EmitUndefined(ILGenerator generator)
		{
			generator.LoadField(ReflectionHelpers.Undefined_Value);
		}
		
		/// <summary>
		/// Emits a default value of the given type.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		/// <param name="type"> The type of value to generate. </param>
		public static void EmitDefaultValue(ILGenerator generator, Type type)
		{
			var temp = generator.CreateTemporaryVariable(type);
			generator.LoadAddressOfVariable(temp);
			generator.InitObject(temp.Type);
			generator.LoadVariable(temp);
			generator.ReleaseTemporaryVariable(temp);
		}
		
		/// <summary>
		/// Emits a JavaScriptException.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		/// <param name="name"> The type of error to generate. </param>
		/// <param name="message"> The error message. </param>
		public static void EmitThrow(ILGenerator generator, string name, string message)
		{
			EmitThrow(generator, name, message, null, null, 0);
		}

		/// <summary>
		/// Emits a JavaScriptException.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		/// <param name="name"> The type of error to generate. </param>
		/// <param name="message"> The error message. </param>
		/// <param name="optimizationInfo"> Information about the line number, function and path. </param>
		public static void EmitThrow(ILGenerator generator, string name, string message, OptimizationInfo optimizationInfo)
		{
			EmitThrow(generator, name, message, optimizationInfo.SourcePath, optimizationInfo.FunctionName, 1);
		}

		/// <summary>
		/// Emits a JavaScriptException.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		/// <param name="name"> The type of error to generate. </param>
		/// <param name="message"> The error message. </param>
		/// <param name="path"> The path of the javascript source file that is currently executing. </param>
		/// <param name="function"> The name of the currently executing function. </param>
		/// <param name="line"> The line number of the statement that is currently executing. </param>
		public static void EmitThrow(ILGenerator generator, string name, string message, string path, string function, int line)
		{
			EmitHelpers.LoadEngine(generator);
			generator.LoadString(name);
			generator.LoadString(message);
			generator.LoadInt32(line);
			generator.LoadStringOrNull(path);
			generator.LoadStringOrNull(function);
			generator.NewObject(ReflectionHelpers.JavaScriptException_Constructor_Error);
			generator.Throw();
		}

		/// <summary>
		/// Emits the given value.  Only possible for certain types.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		/// <param name="value"> The value to emit. </param>
		public static void EmitValue(ILGenerator generator, object value)
		{
			if (value == null)
				generator.LoadNull();
			else if ( value is JavaScript.Undefined )
			{
				EmitHelpers.EmitUndefined(generator);
			}
			else if ( value is System.Reflection.MethodBase )
			{
				generator.LoadToken(value as System.Reflection.MethodBase);
				generator.Call(ReflectionHelpers.MethodBase_GetMethod);
			}
			else if ( value is MethodGroup)
			{
				throw new NotImplementedException("Can't emit methodGroups here");
			}
			else
			{
				if(value is bool){
					generator.LoadBoolean((bool)value);
				}else if(value is byte){
					generator.LoadInt32((byte)value);
				}else if(value is char){
					generator.LoadInt32((char)value);
				}else if(value is double){
					generator.LoadDouble((double)value);
				}else if(value is short){
					generator.LoadInt32((short)value);
				}else if(value is int){
					generator.LoadInt32((int)value);
				}else if(value is long){
					generator.LoadInt64((long)value);
				}else if(value is sbyte){
					generator.LoadInt32((sbyte)value);
				}else if(value is float){
					generator.LoadDouble((float)value);
				}else if(value is string){
					generator.LoadString((string)value);
				}else if(value is ushort){
					generator.LoadInt32((ushort)value);
				}else if(value is uint){
					generator.LoadInt32((uint)value);
				}else if(value is ulong){
					generator.LoadInt64((ulong)value);
				}else{
					throw new NotImplementedException(string.Format("Cannot emit the value '{0}' (a "+value.GetType()+")", value));
				}
			}
		}



		//	 LOAD METHOD PARAMETERS
		//_________________________________________________________________________________________

		/// <summary>
		/// Pushes a reference to the ScriptEngine onto the stack.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		public static void LoadEngine(ILGenerator generator)
		{
			
			// Get the static ref to the engine:
			generator.LoadField(generator.Runtime.ModuleInfo.GlobalEngine);
			
		}
		
		/// <summary>
		/// Pushes a reference to the ScriptEngine or WebAssembly module onto the stack.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		public static void LoadRuntime(ILGenerator generator){
			
			// Get the static ref to the runtime:
			generator.LoadField(generator.Runtime.ModuleInfo.GlobalRuntime);
			
		}
		
		/// <summary>
		/// Pushes an aligned address onto the stack relative to the module memory.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		public static void LoadAddress(ILGenerator generator,ulong offset){
			
			// Load the void*:
			generator.LoadField(generator.Runtime.ModuleInfo.Memory);
			
			/*
			// Add the offset (on platforms with the JIT, 
			// this gets optimized out as the above field is static readonly):
			if(generator.X64){
				generator.LoadInt64(offset);
			}else{
				generator.LoadInt32((uint)offset);
			}
			
			generator.Add();
			*/
			
		}
		
		/// <summary>
		/// Pushes an unaligned address onto the stack relative to the module memory.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		public static void LoadAddress(ILGenerator generator,ulong offset,byte alignment){
			
			// Load the void*:
			generator.LoadField(generator.Runtime.ModuleInfo.Memory);
			
			// Add the offset (on platforms with the JIT, 
			// this gets optimized out as the above field is static readonly):
			if(generator.X64){
				generator.LoadInt64(offset);
			}else{
				generator.LoadInt32((uint)offset);
			}
			
			generator.Add();
			generator.Unaligned(alignment);
		}
		
		/// <summary>
		/// Pushes a particular proto to the stack.
		/// </summary>
		public static void LoadPrototype(ILGenerator generator,Type forType){
			
			// Load proto set:
			LoadPrototypes(generator);
			
			// Put the type on the stack:
			generator.LoadToken(forType);
			
			// Call get now:
			generator.Call(ReflectionHelpers.PrototypeLookup_Get);
			
		}
		
		/// <summary>
		/// Pushes a reference to the prototype set onto the stack.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		public static void LoadPrototypes(ILGenerator generator)
		{
			// Get the static ref to the engine:
			generator.LoadField(generator.Runtime.ModuleInfo.GlobalPrototypes);
		}
		
	}

}
