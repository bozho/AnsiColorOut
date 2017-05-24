using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bozho.PowerShell {

	/// <summary>
	/// Color matcher for System.Diagnostics.Process targets.
	/// </summary>
	public class ProcessMatcher : ColorMatcher<Process> {
		public override IColorMatch<Process> CreateMatch(ConsoleColor foregroundColor, object colorMatch) {
			Type colorMatchType = colorMatch.GetType();

			if (colorMatch is ProcessPriorityClass) return new ProcessPriorityMatch(foregroundColor, (ProcessPriorityClass)colorMatch);

			if (colorMatchType.IsAssignableFrom(typeof(string))) return new ProcessPriorityMatch(foregroundColor, (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), (string)colorMatch, true));

			throw new ArgumentException("Unsupported process color match type!", nameof(colorMatch));
		}
	}


	/// <summary>
	/// A base class for System.Diagnostics.Process target matches.
	/// </summary>
	public abstract class ProcessMatch : IColorMatch<Process> {
		protected ProcessMatch(ConsoleColor foregroundColor) { ForegroundColor = foregroundColor; }

		public ConsoleColor ForegroundColor { get; }

		public abstract bool IsMatch(Process target);
		public abstract object GetMatchData();

		bool IColorMatch.IsMatch(object target) { return IsMatch((Process)target);}
	}


	/// <summary>
	/// Output color match that matches on process priority
	/// </summary>
	internal class ProcessPriorityMatch : ProcessMatch {

		static readonly Dictionary<ProcessPriorityClass, int> sPriorityMap;

		/// <summary>
		/// It would appear that Process.PriorityClass property is not always readable. We'll map 
		/// Process.BasePriority values the best we can and match on those.
		/// </summary>
		static ProcessPriorityMatch() {
			sPriorityMap = new Dictionary<ProcessPriorityClass, int> {
				{ProcessPriorityClass.Idle, 4},
				{ProcessPriorityClass.BelowNormal, 6},
				{ProcessPriorityClass.Normal, 8},
				{ProcessPriorityClass.AboveNormal, 10},
				{ProcessPriorityClass.High, 13},
				{ProcessPriorityClass.RealTime, 24}
			};
		}

		readonly int mBasePriority;

		public ProcessPriorityMatch(ConsoleColor consoleColor, ProcessPriorityClass priorityClass) : base(consoleColor) {
			mBasePriority = sPriorityMap[priorityClass];
		}

		public override bool IsMatch(Process process) { return process.BasePriority == mBasePriority; }

		public override object GetMatchData() { return sPriorityMap.Where(p => p.Value == mBasePriority).Select(p => p.Key).First(); }
		
	}
}
