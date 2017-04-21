using System;
using System.Reflection;


namespace JavaScript.Compiler
{
	/// <summary>
	/// Methods related to the Type enum.
	/// </summary>
	internal static class PrimitiveTypeUtilities
	{
		
		/// <summary>
		/// Checks if the given primitive type is numeric.
		/// </summary>
		/// <param name="type"> The primitive type to check. </param>
		/// <returns> <c>true</c> if the given primitive type is numeric; <c>false</c> otherwise. </returns>
		public static bool IsNumeric(Type type)
		{
			return type == typeof(double) || type == typeof(int) || type == typeof(uint);
		}

		/// <summary>
		/// Checks if the given primitive type is a string type.
		/// </summary>
		/// <param name="type"> The primitive type to check. </param>
		/// <returns> <c>true</c> if the given primitive type is a string type; <c>false</c>
		/// otherwise. </returns>
		public static bool IsString(Type type)
		{
			return type == typeof(string) || type == typeof(ConcatenatedString);
		}

		/// <summary>
		/// Checks if the given primitive type is a value type.
		/// </summary>
		/// <param name="type"> The primitive type to check. </param>
		/// <returns> <c>true</c> if the given primitive type is a value type; <c>false</c> otherwise. </returns>
		public static bool IsValueType(Type type)
		{
			#if NETFX_CORE
			return type.GetTypeInfo().IsValueType;
			#else
			return type.IsValueType;
			#endif
		}

		/// <summary>
		/// Gets a type that can hold values of both the given types.
		/// </summary>
		/// <param name="a"> The first of the two types to find the LCD for. </param>
		/// <param name="b"> The second of the two types to find the LCD for. </param>
		/// <returns> A type that can hold values of both the given types. </returns>
		public static Type GetCommonType(Type a, Type b)
		{
			// If the types are the same, then trivially that type will do.
			if (a == b)
				return a;

			// If both types are numeric, return the number type.
			if (IsNumeric(a) && IsNumeric(b))
				return typeof(double);

			// Otherwise, fall back on the generic Any type.
			return typeof(object);
		}
	}

}
