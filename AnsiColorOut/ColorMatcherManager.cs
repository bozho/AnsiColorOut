using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bozho.PowerShell {

	/// <summary>
	/// Color matcher manager class.
	/// </summary>
	public class ColorMatcherManager : IColorMatcherManager {

		readonly Dictionary<Type, IColorMatcher> mMatchers;

		public ColorMatcherManager() { mMatchers = new Dictionary<Type, IColorMatcher>(); }

		/// <summary>
		/// The method will register all matchers (classes implementing IColorMatcher&lt;&gt; interface)
		/// in the specified assembly using Reflection.
		/// </summary>
		public void RegisterMatchers(Assembly assembly) {

			// get all types implementing IColorMatcher interface
			IEnumerable<Type> colorMatcherTypes =
				from type in assembly.GetTypes()
				where !type.IsGenericType && type.IsAssignableToGenericType(typeof(IColorMatcher<>))
				select type;

			foreach(Type colorMatcherType in colorMatcherTypes) {
				// for each color matcher, get interface types
				IEnumerable<Type> colorMatcherInterfaces =
					from i in colorMatcherType.GetInterfaces()
					where i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IColorMatcher<>)
					select i;

				foreach(Type colorMatcherInterface in colorMatcherInterfaces) {
					// for each interface, extract the target type
					Type matchingType = colorMatcherInterface.GetGenericArguments().First();
					if(mMatchers.ContainsKey(matchingType)) continue;

					// if the target type is not already registered, create an instance of the matcher and register it.
					IColorMatcher matcher = (IColorMatcher)Activator.CreateInstance(colorMatcherType);
					mMatchers.Add(matchingType, matcher);
				}
			}
		}

		/// <summary>
		/// Retrieves a matcher based on the target object type. Throws if type is not found.
		/// </summary>
		public IColorMatcher GetMatcher(Type type) { return mMatchers[type]; }

		/// <summary>
		/// Tries to retrieve a matcher based on the target type.
		/// </summary>
		public bool TryGetMatcher(Type type, out IColorMatcher matcher) { return mMatchers.TryGetValue(type, out matcher); }
	}
}
