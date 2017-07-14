using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Text.RegularExpressions;

namespace Bozho.PowerShell {

	/// <summary>
	/// Color matcher for FileSystemInfo targets.
	/// </summary>
	public class FileSystemInfoMatcher : ColorMatcher<FileSystemInfo> {
		public override IColorMatch<FileSystemInfo> CreateMatch(ConsoleColor foregroundColor, object colorMatch) {
			Type colorMatchType = colorMatch.GetType();

			if(colorMatchType.IsArray && (colorMatchType.GetElementType().IsAssignableFrom(typeof(string)))) return new FileExtensionMatch(foregroundColor, ((Array)colorMatch).Cast<string>().ToArray());

			if(colorMatchType.IsAssignableFrom(typeof(Regex))) return new FileRegexNameMatch(foregroundColor, (Regex)colorMatch);

			if(colorMatchType.IsAssignableFrom(typeof(ScriptBlock))) return new FileScriptMatch(foregroundColor, (ScriptBlock)colorMatch);

			if (colorMatch is FileAttributes) return new FileAttributeMatch(foregroundColor, (FileAttributes)colorMatch);

			throw new ArgumentException("Unsupported file system color match type!", nameof(colorMatch));
		}
	}


	/// <summary>
	/// A base class for FileSystemInfo target matches.
	/// </summary>
	public abstract class FileSystemInfoMatch : IColorMatch<FileSystemInfo> {

		protected FileSystemInfoMatch(ConsoleColor foregroundColor) { ForegroundColor = foregroundColor; }

		public ConsoleColor ForegroundColor { get; }

		public abstract bool IsMatch(FileSystemInfo target);
		public abstract object GetMatchData();

		bool IColorMatch.IsMatch(object target) { return IsMatch((FileSystemInfo)target);}
	}


	/// <summary>
	/// Output color match that matches on file extensions.
	/// 
	/// Does not match directories.
	/// </summary>
	internal class FileExtensionMatch : FileSystemInfoMatch {

		readonly HashSet<string> mExtensions;

		public FileExtensionMatch(ConsoleColor foregroundColor, IEnumerable<string> extensions) : base(foregroundColor) {
			// The benefit of using a hashset is deduplication of entries and fast lookups
			mExtensions = new HashSet<string>(from extension in extensions
											  // FileSystemInfo.Extension includes a leading . if a file has an extension, 
											  // otherwise the Extension is an empty string; format our HashSet accordingly.
											  let ext = string.IsNullOrEmpty(extension) ? "" : $".{extension}"
				select ext);
		}

		public override bool IsMatch(FileSystemInfo fs) {
			return (fs.Attributes & FileAttributes.Directory) == 0 && mExtensions.Contains(fs.Extension, StringComparer.CurrentCultureIgnoreCase);
		}

		public override object GetMatchData() {
			// reformat extension strings back to the input format (i.e. remove the leading .)
			return mExtensions.Select(ext => string.IsNullOrEmpty(ext) ? "" : ext.Substring(1)).ToArray();
		}
	}


	/// <summary>
	/// Output color matcher that matches on file/directory attributes.
	/// 
	/// Multiple attributes can be specified by ORing them together.
	/// </summary>
	internal class FileAttributeMatch : FileSystemInfoMatch {
		readonly FileAttributes mFileAttributes;

		public FileAttributeMatch(ConsoleColor consoleColor, FileAttributes fileAttributes) : base(consoleColor) {
			mFileAttributes = fileAttributes;
		}

		public override bool IsMatch(FileSystemInfo fs) {
			return (fs.Attributes & mFileAttributes) != 0;
		}

		public override object GetMatchData() {
			return mFileAttributes;
		}
	}


	/// <summary>
	/// Output color matcher that matches on file/directory name regular expression match.
	/// 
	/// Since regular expressions can be instantiated directly in PS, users are free to specify
	/// any regex options when defining the matching expression.
	/// </summary>
	internal class FileRegexNameMatch : FileSystemInfoMatch {
		readonly Regex mNameRegex;

		public FileRegexNameMatch(ConsoleColor consoleColor, Regex nameRegex) : base(consoleColor) {
			mNameRegex = nameRegex;
		}

		public override bool IsMatch(FileSystemInfo fs) {
			return mNameRegex.IsMatch(fs.Name);
		}

		public override object GetMatchData() {
			// clone the regex
			return new Regex(mNameRegex.ToString(), mNameRegex.Options);
		}
	}


	/// <summary>
	/// Output color matcher that matches on PS script return bool value.
	/// 
	/// The match script must accept exactly one parameter of FileSystemInfo type and return a single bool value.
	/// </summary>
	internal class FileScriptMatch : FileSystemInfoMatch {
		readonly ScriptBlock mMatchScript;

		public FileScriptMatch(ConsoleColor consoleColor, ScriptBlock matchScript) : base(consoleColor) {
			ScriptBlockAst scriptBlockAst = (ScriptBlockAst)matchScript.Ast;
			if (scriptBlockAst.ParamBlock.Parameters.Count != 1) throw new ArgumentException("Match script blocks must accept exactly one parameter of FileInfo or DirectoryInfo type.");

			mMatchScript = matchScript;
		}

		public override bool IsMatch(FileSystemInfo fs) {
			Collection<PSObject> result = mMatchScript.Invoke(fs);
			if (result.Count != 1 || !(result[0].BaseObject is bool)) throw new ArgumentException("Match scripts must return a single true/false result.");
			return (bool)result[0].BaseObject;
		}

		public override object GetMatchData() {
			// clone the script
			return ScriptBlock.Create(mMatchScript.Ast.ToString());
		}
	}

}
