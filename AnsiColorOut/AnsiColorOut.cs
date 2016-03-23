using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Bozho.PowerShell {

	internal interface IAnsiColorMatch<TTarget> {
		/// <summary>
		/// Implementing classes implement matching logic appropriate for the matcher type.
		/// 
		/// Output color will be selectd for the first matcher that returns true.
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
		/// Output color for this match.
		/// </summary>
		ConsoleColor ConsoleColor { get; }
	}


	public class AnsiColorOut {

		static readonly AnsiColorOut sInstance;

		protected static readonly Dictionary<ConsoleColor, string> sForegroundAnsiColors;
		protected static readonly string sAnsiResetColor;

		#region ctor

		static AnsiColorOut() {
			sInstance = new AnsiColorOut();

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

			sAnsiResetColor = "\x1B[0m";
		}

		protected AnsiColorOut() {
			mFileSystemColors = new List<IAnsiColorMatch<FileSystemInfo>>();
		}

		#endregion ctor


		#region public members

		public static ConsoleColor? GetFileInfoColor(FileSystemInfo fs) {
			return sInstance.mFileSystemColors.Where(fsc => fsc.IsMatch(fs)).Select(fsc => (ConsoleColor?)fsc.ConsoleColor).FirstOrDefault();
		}


		public static string GetFileInfoAnsiColor(FileSystemInfo fs) {
			ConsoleColor? consoleColor = GetFileInfoColor(fs);

			return consoleColor.HasValue ? sForegroundAnsiColors[consoleColor.Value] : sAnsiResetColor;
		}


		public static string AnsiResetColor { get { return sAnsiResetColor; } }

		#endregion public members


		#region cmdlet support methods

		internal static void SetFileSystemColors(object[] fileSystemColors) {

			sInstance.mFileSystemColors = (from fileSystemColor in fileSystemColors.Cast<Hashtable>()
										  let color = (ConsoleColor)fileSystemColor["Color"]
										  let match = fileSystemColor["Match"]
										  select FileSystemColorMatch.CreateFileSystemColorMatch(color, match)).ToList();
		}


		internal static object[] GetFileSystemColors() {

			Func<ConsoleColor, object, Hashtable> getMatchHashtable = (color, match) => {
				Hashtable colorHash = new Hashtable();
				colorHash["Color"] = color;
				colorHash["Match"] = match;
				return colorHash;
			};

			return (from fsColor in sInstance.mFileSystemColors
					select (object)getMatchHashtable(fsColor.ConsoleColor, fsColor.GetMatcherCopy())).ToArray();
		}

		#endregion cmdlet support methods

		#region private fields

		List<IAnsiColorMatch<FileSystemInfo>> mFileSystemColors;

		#endregion private fields

	}
}
