using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.IO;
using System.Management.Automation.Language;

namespace Bozho.PowerShell {

	/// <summary>
	/// A base class for file system color matchers.
	/// </summary>
	internal abstract class FileSystemColorMatch {

		/// <summary>
		/// Factory method for supported color matchers.
		/// </summary>
		public static FileSystemColorMatch CreateFileSystemColorMatch(ConsoleColor consoleColor, object colorMatch) {
			Type colorMatchType = colorMatch.GetType();

			if(colorMatchType.IsArray && (colorMatchType.GetElementType().IsAssignableFrom(typeof(string)))) return new FileExtensionMatch(consoleColor, ((Array)colorMatch).Cast<string>().ToArray());

			if(colorMatchType.IsAssignableFrom(typeof(Regex))) return new FileSystemRegexNameMatch(consoleColor, (Regex)colorMatch);

			if(colorMatchType.IsAssignableFrom(typeof(ScriptBlock))) return new FileSystemScriptMatch(consoleColor, (ScriptBlock)colorMatch);

			if (colorMatch is FileAttributes) return new FileSystemAttributeMatch(consoleColor, (FileAttributes)colorMatch);

			throw new ArgumentException("Unsupported file system color match type!", "colorMatch");
		}

		public FileSystemColorMatch(ConsoleColor consoleColor) {
			ConsoleColor = consoleColor;
		}

		/// <summary>
		/// Derived classes implement matching logic appropriate for the matcher type.
		/// 
		/// Output color will be selectd for the first matcher that returns true.
		/// </summary>
		public abstract bool IsMatch(FileSystemInfo fs);

		/// <summary>
		/// Output color for this match.
		/// </summary>
		public readonly ConsoleColor ConsoleColor;

		/// <summary>
		/// Match object. Derived classes should return a clone, 
		/// to prevent direct modification of reference type matchers.
		/// </summary>
		public abstract object Match { get; }
	}


	/// <summary>
	/// Output color matcher that matches on file extensions.
	/// 
	/// Does not match directories.
	/// </summary>
	internal class FileExtensionMatch : FileSystemColorMatch {

		readonly HashSet<string> mExtensions;

		public FileExtensionMatch(ConsoleColor consoleColor, string[] extensions) : base(consoleColor) {
			// The benefit of using a hashset is deduplication of entries and fast lookups
			mExtensions = new HashSet<string>(from extension in extensions
											  // FileSystemInfo.Extension includes a leading . if a file has an extension, 
											  // otherwise the Extension is an empty string; format our HashSet accordingly.
											  let ext = String.IsNullOrEmpty(extension) ? "" : String.Format(".{0}", extension)
											  select ext);
		}

		public override bool IsMatch(FileSystemInfo fs) {
			if ((fs.Attributes & FileAttributes.Directory) != 0) return false;
			return mExtensions.Contains(fs.Extension);
		}

		public override object Match {
			// reformat extension strings back to the input format (i.e. remove the leading .)
			get { return mExtensions.Select(ext => String.IsNullOrEmpty(ext) ? "" : ext.Substring(1)).ToArray(); }
		}
	}


	/// <summary>
	/// Output color matcher that matches on file/directory attributes.
	/// 
	/// Multiple attributes can be specified by ORing them together.
	/// </summary>
	internal class FileSystemAttributeMatch : FileSystemColorMatch {

		FileAttributes mFileAttributes;

		public FileSystemAttributeMatch(ConsoleColor consoleColor, FileAttributes fileAttributes) : base(consoleColor) {
			mFileAttributes = fileAttributes;
		}

		public override bool IsMatch(FileSystemInfo fs) {
			return (fs.Attributes & mFileAttributes) != 0;
		}

		public override object Match {
			get { return mFileAttributes; }
		}
	}


	/// <summary>
	/// Output color matcher that matches on file/directory name regular expression match.
	/// 
	/// Since regular expressions can be instantiated directly in PS, users are free to specify
	/// any regex options when defining the matching expression.
	/// </summary>
	internal class FileSystemRegexNameMatch : FileSystemColorMatch {
		readonly Regex mNameRegex;

		public FileSystemRegexNameMatch(ConsoleColor consoleColor, Regex nameRegex) : base(consoleColor) {
			mNameRegex = nameRegex;
		}

		public override bool IsMatch(FileSystemInfo fs) {
			return mNameRegex.IsMatch(fs.Name);
		}

		public override object Match {
			// clone the regex
			get { return new Regex(mNameRegex.ToString(), mNameRegex.Options); }
		}
	}


	/// <summary>
	/// Output color matcher that matches on PS script return bool value.
	/// 
	/// The match script must accept exactly one parameter of FileSystemInfo type and return a single bool value.
	/// </summary>
	internal class FileSystemScriptMatch : FileSystemColorMatch {

		ScriptBlock mMatchScript;

		public FileSystemScriptMatch(ConsoleColor consoleColor, ScriptBlock matchScript) : base(consoleColor) {
			ScriptBlockAst scriptBlockAst = (ScriptBlockAst)matchScript.Ast;
			if (scriptBlockAst.ParamBlock.Parameters.Count != 1) throw new ArgumentException("Match script blocks must accept exactly one parameter of FileInfo or DirectoryInfo type.");

			mMatchScript = matchScript;
		}

		public override bool IsMatch(FileSystemInfo fs) {
			var result = mMatchScript.Invoke(fs);
			if (result.Count != 1 || !(result[0].BaseObject is bool)) throw new ArgumentException("Match scripts must return a single true/false result.");
			return (bool)result[0].BaseObject;
		}

		public override object Match {
			// clone the script
			get { return ScriptBlock.Create(mMatchScript.Ast.ToString()); }
		}
	}
}
