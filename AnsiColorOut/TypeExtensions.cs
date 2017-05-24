using System;
using System.Linq;

namespace Bozho.PowerShell {

	/// <summary>
	/// Type type extension methdos.
	/// </summary>
	public static class TypeExtensions {
		/// <summary>
		/// Checks whether a type is assignable to an open generic type.
		/// </summary>
		public static bool IsAssignableToGenericType(this Type type, Type openGenericType) {
			if(!openGenericType.IsGenericType) throw new ArgumentException("Not a generic type", nameof(openGenericType));
			while(type != null) {
				if(type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType) return true;
				if(type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)) return true;
				type = type.BaseType;
			}

			return false;
		}
	}
}
