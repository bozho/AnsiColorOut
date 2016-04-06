using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Bozho.PowerShell {

	internal interface IColorMatch<TTarget> {
		/// <summary>
		/// Implementing classes implement matching logic appropriate for the matcher type.
		/// 
		/// Output color will be selected for the first matcher that returns true.
		/// </summary>
		bool IsMatch(TTarget target);

		/// <summary>
		/// Matcher object. Implementing classes should return a clone, 
		/// to prevent direct modification of reference type matchers.
		/// 
		/// Used to get a settings object.
		/// </summary>
		object GetMatcherCopy();

		/// <summary>
		/// Output foreground color for this match.
		/// </summary>
		ConsoleColor ConsoleColor { get; }
	}


	public class AnsiColorOut {

		const string HashtableColor = "Color";
		const string HashtableMatch = "Match";

		static List<IColorMatch<FileSystemInfo>> sFileSystemColors;

		static readonly Dictionary<ConsoleColor, string> sForegroundAnsiColors;
		static readonly Dictionary<ConsoleColor, string> sBackgroundAnsiColors;
		static readonly string sForegroundColorReset;
		static readonly string sBackgroundColorReset;
		static readonly string sColorReset;

		#region ctor

		static AnsiColorOut() {

			sFileSystemColors = new List<IColorMatch<FileSystemInfo>>();

			sForegroundAnsiColors = new Dictionary<ConsoleColor, string> {
				{ ConsoleColor.DarkGray, "\x1B[30;1m" },
				{ ConsoleColor.Black, "\x1B[30m" },

				{ ConsoleColor.Red, "\x1B[31;1m" },
				{ ConsoleColor.DarkRed, "\x1B[31m" },

				{ ConsoleColor.Green, "\x1B[32;1m" },
				{ ConsoleColor.DarkGreen, "\x1B[32m" },

				{ ConsoleColor.Yellow, "\x1B[33;1m" },
				{ ConsoleColor.DarkYellow, "\x1B[33m" },

				{ ConsoleColor.Blue, "\x1B[34;1m" },
				{ ConsoleColor.DarkBlue, "\x1B[34m" },

				{ ConsoleColor.Magenta, "\x1B[35;1m" },
				{ ConsoleColor.DarkMagenta, "\x1B[35m" },

				{ ConsoleColor.Cyan, "\x1B[36;1m" },
				{ ConsoleColor.DarkCyan, "\x1B[36m" },

				{ ConsoleColor.White, "\x1B[37;1m" },
				{ ConsoleColor.Gray, "\x1B[37m" },

			};

			sBackgroundAnsiColors = new Dictionary<ConsoleColor, string> {
				{ ConsoleColor.DarkGray, "\x1B[40;1m" },
				{ ConsoleColor.Black, "\x1B[40m" },

				{ ConsoleColor.Red, "\x1B[41;1m" },
				{ ConsoleColor.DarkRed, "\x1B[41m" },

				{ ConsoleColor.Green, "\x1B[42;1m" },
				{ ConsoleColor.DarkGreen, "\x1B[42m" },

				{ ConsoleColor.Yellow, "\x1B[43;1m" },
				{ ConsoleColor.DarkYellow, "\x1B[43m" },

				{ ConsoleColor.Blue, "\x1B[44;1m" },
				{ ConsoleColor.DarkBlue, "\x1B[44m" },

				{ ConsoleColor.Magenta, "\x1B[45;1m" },
				{ ConsoleColor.DarkMagenta, "\x1B[45m" },

				{ ConsoleColor.Cyan, "\x1B[46;1m" },
				{ ConsoleColor.DarkCyan, "\x1B[46m" },

				{ ConsoleColor.White, "\x1B[47;1m" },
				{ ConsoleColor.Gray, "\x1B[47m" },

			};

			sForegroundColorReset = "\x1B[39m";
			sBackgroundColorReset = "\x1B[49m";
			sColorReset = "\x1B[0m";
		}

		#endregion ctor


		#region public members

		public static ConsoleColor? GetFileInfoColor(FileSystemInfo fs) {
			return sFileSystemColors.Where(fsc => fsc.IsMatch(fs)).Select(fsc => (ConsoleColor?)fsc.ConsoleColor).FirstOrDefault();
		}


		public static string GetFileInfoAnsiColor(FileSystemInfo fs) {
			ConsoleColor? consoleColor = GetFileInfoColor(fs);

			return consoleColor.HasValue ? sForegroundAnsiColors[consoleColor.Value] : sColorReset;
		}

		public static string GetForegroundAnsiColor(ConsoleColor color) { return sForegroundAnsiColors[color]; }

		public static string GetBackgroundAnsiColor(ConsoleColor color) { return sBackgroundAnsiColors[color]; }

		public static string ForegroundColorReset { get { return sForegroundColorReset; } }

		public static string BackgroundColorReset { get { return sBackgroundColorReset; } }

		public static string ColorReset { get { return sColorReset; } }

		#endregion public members


		#region cmdlet support methods

		internal static void SetFileSystemColors(object[] fileSystemColors) {

			sFileSystemColors = (from fileSystemColor in fileSystemColors.Cast<Hashtable>()
										  let color = (ConsoleColor)fileSystemColor[HashtableColor]
										  let match = fileSystemColor[HashtableMatch]
										  select FileSystemColorMatch.CreateFileSystemColorMatch(color, match)).ToList();
		}


		internal static object[] GetFileSystemColors() {

			Func<ConsoleColor, object, Hashtable> getMatchHashtable = (color, match) => {
				Hashtable colorHash = new Hashtable();
				colorHash[HashtableColor] = color;
				colorHash[HashtableMatch] = match;
				return colorHash;
			};

			return (from fsColor in sFileSystemColors
					select (object)getMatchHashtable(fsColor.ConsoleColor, fsColor.GetMatcherCopy())).ToArray();
		}

		#endregion cmdlet support methods
	}
}
