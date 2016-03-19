using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;
using System.Collections;

namespace Bozho.PowerShell {
	public class AnsiColorOut {

		static readonly AnsiColorOut sInstance;

		static readonly Dictionary<ConsoleColor, string> sForegroundAnsiColors;
		static readonly string sAnsiResetColor;

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

				{ConsoleColor.White, "\x1B[37;1m" },
				{ConsoleColor.Gray, "\x1B[37m" },

			};

			sAnsiResetColor = "\x1B[0m";
		}

		AnsiColorOut() {
			FileSystemColors = new List<FileSystemColorMatch>();
		}


		public static ConsoleColor? GetFileInfoColor(FileSystemInfo fs) {
			return sInstance.FileSystemColors.Where(fsc => fsc.IsMatch(fs)).Select(fsc => (ConsoleColor?)fsc.ConsoleColor).FirstOrDefault();
		}

		public static string GetFileInfoAnsiColor(FileSystemInfo fs) {
			ConsoleColor? consoleColor = GetFileInfoColor(fs);

			return consoleColor.HasValue ? sForegroundAnsiColors[consoleColor.Value] : sAnsiResetColor;
		}

		internal static void SetFileSystemColors(object[] fileSystemColors) {

			sInstance.FileSystemColors = (from fileSystemColor in fileSystemColors.Cast<Hashtable>()
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

			return (from fsColor in sInstance.FileSystemColors
					select (object)getMatchHashtable(fsColor.ConsoleColor, fsColor.Match)).ToArray();
		}

		public static string AnsiResetColor { get { return sAnsiResetColor; } }

		List<FileSystemColorMatch> FileSystemColors;
	}


	public static class FileSystemInfoExtensions {

		static readonly char[] sDefaultFileAttributePattern;
		static char[] sFileAttributePattern;
		static readonly Dictionary<char, FileAttributes> sFileAttributeChars;

		static FileSystemInfoExtensions() {
			sDefaultFileAttributePattern = new char[] { 'D', 'N', 'R', 'H', 'S', 'A', 'E', 'T', 'P', 'C', 'O', 'I', 'J', 'G', 'B' };
			sFileAttributePattern = sDefaultFileAttributePattern;

			sFileAttributeChars = new Dictionary<char, FileAttributes> {
				{ 'A', FileAttributes.Archive },
				{ 'C', FileAttributes.Compressed },
				{ 'D', FileAttributes.Directory },
				{ 'E', FileAttributes.Encrypted },
				{ 'H', FileAttributes.Hidden },
				{ 'G', FileAttributes.IntegrityStream },
				{ 'N', FileAttributes.Normal },
				{ 'B', FileAttributes.NoScrubData },
				{ 'I', FileAttributes.NotContentIndexed },
				{ 'O', FileAttributes.Offline },
				{ 'R', FileAttributes.ReadOnly },
				{ 'J', FileAttributes.ReparsePoint },
				{ 'P', FileAttributes.SparseFile },
				{ 'S', FileAttributes.System },
				{ 'T', FileAttributes.Temporary }
			};
		}

		public static string GetExtendedMode(PSObject fsObject) {
			FileSystemInfo fs = (FileSystemInfo)fsObject.BaseObject;

			StringBuilder extendedMode = new StringBuilder(sFileAttributePattern.Length);

			foreach (var attr in sFileAttributePattern) {
				if((fs.Attributes & sFileAttributeChars[attr]) != 0) {
					extendedMode.Append(attr);
				}
				else {
					extendedMode.Append('_');
				}
			}
			return extendedMode.ToString();
		}

		internal static void SetFileInfoAttributePattern(SetFileInfoAttributePattern setFileInfoAttributePattern) {

			if(setFileInfoAttributePattern.DefaultPattern.IsPresent) {
				sFileAttributePattern = sDefaultFileAttributePattern;
				return;
			}

			sFileAttributePattern = (from p in setFileInfoAttributePattern.AttributePattern.ToCharArray()
									  let attr = Char.ToUpper(p)
									  where sFileAttributeChars.ContainsKey(attr)
									  select attr).ToArray();
		}

		internal static string GetFileInfoAttributePattern(GetFileInfoAttributePattern getFileInfoAttributePattern) {
			StringBuilder output = new StringBuilder();
			sFileAttributePattern.Aggregate(output, (sb, c) => sb.Append(c), (sb) => sb);

			if(getFileInfoAttributePattern.ShowAll.IsPresent) {
				output.AppendLine().AppendLine();

				foreach (var fa in sFileAttributeChars) {
					output.AppendLine(String.Format("{0} - {1}", fa.Key, fa.Value.ToString()));
				}
				output.AppendLine();
			}

			return output.ToString();
		}
	}
}
