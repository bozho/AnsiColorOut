using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Bozho.PowerShell {

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

		#region public members

		public static string GetExtendedMode(PSObject fsObject) {
			FileSystemInfo fs = (FileSystemInfo)fsObject.BaseObject;

			StringBuilder extendedMode = new StringBuilder(sFileAttributePattern.Length);

			foreach (var attr in sFileAttributePattern) {
				if ((fs.Attributes & sFileAttributeChars[attr]) != 0) {
					extendedMode.Append(attr);
				}
				else {
					extendedMode.Append('_');
				}
			}
			return extendedMode.ToString();
		}

		#endregion public members


		#region cmdlet support methods

		internal static void SetFileInfoAttributePattern(SetFileInfoAttributePattern setFileInfoAttributePattern) {

			if (setFileInfoAttributePattern.DefaultPattern.IsPresent) {
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

			if (getFileInfoAttributePattern.ShowAll.IsPresent) {
				output.AppendLine().AppendLine();

				foreach (var fa in sFileAttributeChars) {
					output.AppendLine(String.Format("{0} - {1}", fa.Key, fa.Value.ToString()));
				}
				output.AppendLine();
			}

			return output.ToString();
		}

		#endregion cmdlet support methods

	}
}
