using System;
using System.Collections.Generic;
using System.Linq;

namespace Bozho.PowerShell {

	/// <summary>
	/// Base color matcher class.
	/// </summary>
	/// <typeparam name="TTarget">Target object's type.</typeparam>
	public abstract class ColorMatcher<TTarget> : IColorMatcher<TTarget> {

		readonly List<IColorMatch<TTarget>> mMatches;

		protected ColorMatcher() {
			mMatches = new List<IColorMatch<TTarget>>();
		}

		/// <summary>
		/// Strongly typed GetMatch method.
		/// </summary>
		/// <param name="target"></param>
		/// <returns>IColorMatch matching the target object. Null if no match was found.</returns>
		public IColorMatch<TTarget> GetMatch(TTarget target) { return mMatches.FirstOrDefault(c => c.IsMatch(target)); }

		/// <summary>
		/// Sets matches for the matcher.
		/// </summary>
		public void SetMatches(IEnumerable<IColorMatch<TTarget>> matches) {
			mMatches.Clear();
			mMatches.AddRange(matches);
		}

		/// <summary>
		/// Retrieves currently registered matches.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IColorMatch<TTarget>> GetMatches() { return mMatches.AsEnumerable(); }

		/// <summary>
		/// A factory method for matches. Implementing classes for a target type will
		/// create instances of its subclasses based on the colorMatch type (and possibly
		/// other conditions)
		/// </summary>
		public abstract IColorMatch<TTarget> CreateMatch(ConsoleColor foregroundColor, object colorMatch);

		/// <summary>
		/// The method will attempt to find a match matching the target object, using 
		/// a collection of stored matches (using IColorMatch&lt;TTarget/^&gt;.IsMatch())
		/// </summary>
		/// <param name="target">Target object for matching</param>
		/// <returns>Matching IColorMatch, or null if none was found.</returns>
		IColorMatch IColorMatcher.GetMatch(object target) { return GetMatch((TTarget)target); }
	}
}