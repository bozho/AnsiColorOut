using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Bozho.PowerShell {

	public class AnsiColorOut {

		const string HashtableColor = "Color";
		const string HashtableMatch = "Match";

		/// <summary>
		/// A lazily initialized FilSystemInfo color matcher. Used for better performance,
		/// to avoid calling into MatchManager for every GetFileInfoAnsiColor call.
		/// </summary>
		static readonly Lazy<IColorMatcher> sFileInfoColorMatcher;

		/// <summary>
		/// A lazily initialized System.Diagnostics.Process color matcher. Used for better 
		/// performance, to avoid calling into MatchManager for every GetFileInfoAnsiColor call.
		/// </summary>
		static readonly Lazy<IColorMatcher> sProcessColorMatcher;


		/// <summary>
		/// Foreground ANSI color escape strings.
		/// </summary>
		static readonly Dictionary<ConsoleColor, string> sForegroundAnsiColors;

		/// <summary>
		/// Background ANSI color escape strings.
		/// </summary>
		static readonly Dictionary<ConsoleColor, string> sBackgroundAnsiColors;

		#region ctor

		static AnsiColorOut() {

			sFileInfoColorMatcher = new Lazy<IColorMatcher>(() => MatcherManager.GetMatcher(typeof(FileSystemInfo)));

			sProcessColorMatcher = new Lazy<IColorMatcher>(() => MatcherManager.GetMatcher(typeof(Process)));

			sForegroundAnsiColors = new Dictionary<ConsoleColor, string> {
				{ ConsoleColor.DarkGray,    "\x1B[1;30m" },
				{ ConsoleColor.Black,       "\x1B[0;30m" },

				{ ConsoleColor.Red,         "\x1B[1;31m" },
				{ ConsoleColor.DarkRed,     "\x1B[0;31m" },

				{ ConsoleColor.Green,       "\x1B[1;32m" },
				{ ConsoleColor.DarkGreen,   "\x1B[0;32m" },

				{ ConsoleColor.Yellow,      "\x1B[1;33m" },
				{ ConsoleColor.DarkYellow,  "\x1B[0;33m" },

				{ ConsoleColor.Blue,        "\x1B[1;34m" },
				{ ConsoleColor.DarkBlue,    "\x1B[0;34m" },

				{ ConsoleColor.Magenta,     "\x1B[1;35m" },
				{ ConsoleColor.DarkMagenta, "\x1B[0;35m" },

				{ ConsoleColor.Cyan,        "\x1B[1;36m" },
				{ ConsoleColor.DarkCyan,    "\x1B[0;36m" },

				{ ConsoleColor.White,       "\x1B[1;37m" },
				{ ConsoleColor.Gray,        "\x1B[0;37m" },

			};

			sBackgroundAnsiColors = new Dictionary<ConsoleColor, string> {
				{ ConsoleColor.DarkGray,    "\x1B[1;40m" },
				{ ConsoleColor.Black,       "\x1B[0;40m" },

				{ ConsoleColor.Red,         "\x1B[1;41m" },
				{ ConsoleColor.DarkRed,     "\x1B[0;41m" },

				{ ConsoleColor.Green,       "\x1B[1;42m" },
				{ ConsoleColor.DarkGreen,   "\x1B[0;42m" },

				{ ConsoleColor.Yellow,      "\x1B[1;43m" },
				{ ConsoleColor.DarkYellow,  "\x1B[0;43m" },

				{ ConsoleColor.Blue,        "\x1B[1;44m" },
				{ ConsoleColor.DarkBlue,    "\x1B[0;44m" },

				{ ConsoleColor.Magenta,     "\x1B[1;45m" },
				{ ConsoleColor.DarkMagenta, "\x1B[0;45m" },

				{ ConsoleColor.Cyan,        "\x1B[1;46m" },
				{ ConsoleColor.DarkCyan,    "\x1B[0;46m" },

				{ ConsoleColor.White,       "\x1B[1;47m" },
				{ ConsoleColor.Gray,        "\x1B[0;47m" },

			};

			ForegroundColorReset = "\x1B[39m";
			BackgroundColorReset = "\x1B[49m";
			ColorReset = "\x1B[0m";
		}

		#endregion ctor


		#region public members

		public static string GetAnsiString(object target) {
			IColorMatcher colorMatcher;
			if(!MatcherManager.TryGetMatcher(target.GetType(), out colorMatcher)) return ColorReset;

			IColorMatch match = colorMatcher.GetMatch(target);
			return match == null ? ColorReset : sForegroundAnsiColors[match.ForegroundColor];
		}

		public static string GetFileInfoAnsiString(FileSystemInfo target) {
			IColorMatch match = sFileInfoColorMatcher.Value.GetMatch(target);
			return match == null ? ColorReset : sForegroundAnsiColors[match.ForegroundColor];
		}

		public static string GetProcessAnsiString(Process target) {
			IColorMatch match = sProcessColorMatcher.Value.GetMatch(target);
			return match == null ? ColorReset : sForegroundAnsiColors[match.ForegroundColor];
		}

		public static string GetForegroundAnsiColor(ConsoleColor color) { return sForegroundAnsiColors[color]; }

		public static string GetBackgroundAnsiColor(ConsoleColor color) { return sBackgroundAnsiColors[color]; }


		/// <summary>
		/// Global matcher manager.
		/// </summary>
		public static IColorMatcherManager MatcherManager { get; set; }

		/// <summary>
		/// Foreground color reset ANSI escape sequence.
		/// </summary>
		public static string ForegroundColorReset { get; }

		/// <summary>
		/// Background color reset ANSI escape sequence.
		/// </summary>
		public static string BackgroundColorReset { get; }

		/// <summary>
		/// Color reset ANSI escape sequence
		/// </summary>
		public static string ColorReset { get; }

		#endregion public members


		#region cmdlet support methods

		internal static void SetMatcherColors<T>(Hashtable[] matcherColors) {
			IColorMatcher<T> matcher = (IColorMatcher<T>)MatcherManager.GetMatcher(typeof(T));

			var matches = from matcherColor in matcherColors
				let foregroundColor = (ConsoleColor)matcherColor[HashtableColor]
				let match = matcherColor[HashtableMatch]
				select matcher.CreateMatch(foregroundColor, match);

			matcher.SetMatches(matches);
		}

		internal static Hashtable[] GetMatchColors<T>() {
			IColorMatcher<T> matcher = (IColorMatcher<T>)MatcherManager.GetMatcher(typeof(T));

			return (from match in matcher.GetMatches()
				select new Hashtable {
					[HashtableColor] = match.ForegroundColor,
					[HashtableMatch] = match.GetMatchData()
				}).ToArray();
		}

		#endregion cmdlet support methods
	}
}
