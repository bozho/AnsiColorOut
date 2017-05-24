using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bozho.PowerShell {

	/// <summary>
	/// Base interface for color matches. When trying to match a target object
	/// to a color, the module will iterate through a collection of matches for
	/// the target object's type and use the first match that returns true.
	/// </summary>
	public interface IColorMatch {
		/// <summary>
		/// The matching method. Implementing classes define a matching condition
		/// (e.g. file's extension, name regexp, process' priority).
		/// 
		/// The condition is then use to determine if a target object matches 
		/// the condition.
		/// </summary>
		/// <param name="target">Target object for matching</param>
		/// <returns>True if the object matches the condition. False otherwise.</returns>
		bool IsMatch(object target);

		/// <summary>
		/// The foreground console color associated with the match. When the module
		/// matches a target object, this color will be use to format console output.
		/// </summary>
		ConsoleColor ForegroundColor { get; }
	}


	/// <summary>
	/// Strongly typed IColorMatch interface. Used internally by matchers.
	/// </summary>
	/// <typeparam name="TTarget">Target object's type.</typeparam>
	public interface IColorMatch<TTarget> : IColorMatch {
		/// <summary>
		/// Strongly typed version of IsMatch method.
		/// </summary>
		/// <param name="target">Target object for matching</param>
		/// <returns>True if the object matches the condition. False otherwise.</returns>
		bool IsMatch(TTarget target);

		/// <summary>
		/// Returns a clone of match data (e.g. a list of file extensions used for matching).
		/// This method is used by Get-XXXXColors to retrieve current settings.
		/// 
		/// It is important for implementing classes to return a deep clone of the data, 
		/// in order to prevent modification of reference-typed data.
		/// </summary>
		/// <returns></returns>
		object GetMatchData();
	}


	/// <summary>
	/// Base interface for color matchers. When trying to match a target object
	/// to a color, the module will call this method to attempt to find a match for 
	/// the target object.
	/// </summary>
	public interface IColorMatcher {
		/// <summary>
		/// The method will attempt to find a match matching the target object, using 
		/// a collection of stored matches (using IColorMatch&lt;TTarget/^&gt;.IsMatch())
		/// </summary>
		/// <param name="target">Target object for matching</param>
		/// <returns>Matching IColorMatch, or null if none was found.</returns>
		IColorMatch GetMatch(object target);

	}


	/// <summary>
	/// Strongly type IColorMatcher interface.
	/// </summary>
	/// <typeparam name="TTarget">Target object's type.</typeparam>
	public interface IColorMatcher<TTarget> : IColorMatcher {
		/// <summary>
		/// Strongly typed GetMatch method.
		/// </summary>
		/// <param name="target"></param>
		/// <returns>IColorMatch matching the target object. Null if no match was found.</returns>
		IColorMatch<TTarget> GetMatch(TTarget target);

		/// <summary>
		/// Sets matches for the matcher.
		/// </summary>
		void SetMatches(IEnumerable<IColorMatch<TTarget>> matches);

		/// <summary>
		/// Gets the matches.
		/// </summary>
		IEnumerable<IColorMatch<TTarget>> GetMatches();

		/// <summary>
		/// A factory method for matches. Implementing classes for a target type will
		/// create instances of its subclasses based on the colorMatch type (and possibly
		/// other conditions)
		/// </summary>
		IColorMatch<TTarget> CreateMatch(ConsoleColor foregroundColor, object colorMatch);
	}


	/// <summary>
	/// Color matcher manager interface. The manager allows registering matchers, as well as
	/// retrieving them based on the target object type.
	/// </summary>
	public interface IColorMatcherManager {
		/// <summary>
		/// Retrieves a matcher based on the target object type. Throws if type is not found.
		/// </summary>
		IColorMatcher GetMatcher(Type type);

		/// <summary>
		/// Tries to retrieve a matcher based on the target type.
		/// </summary>
		bool TryGetMatcher(Type type, out IColorMatcher matcher);
	}
}
