﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Bozho.PowerShell {

	internal static class AnsiColorNouns {
		internal const string FileSystemColors = "FileSystemColors";
		internal const string ProcessColors = "ProcessColors";
		internal const string FileInfoAttributePattern = "FileInfoAttributePattern";
	}


	/// <summary>
	/// A base class for color matchers' Set-XXXColors cmdlets.
	/// </summary>
	public abstract class SetMatcherColors<T> : PSCmdlet {
		[Parameter(Position = 0, Mandatory = true)]
		public Hashtable[] Colors { get; set; }

		protected override void EndProcessing() { AnsiColorOut.SetMatcherColors<T>(Colors); }
	}


	/// <summary>
	/// A base class for color matchers' Get-XXXColors cmdlets.
	/// </summary>
	public abstract class GetMatcherColors<T> : PSCmdlet {
		protected override void EndProcessing() {
			// The second parameter in WriteObject tells PS to enumerate the array,
			// sending one object at a time to the pipeline.
			WriteObject(AnsiColorOut.GetMatchColors<T>(), true);
		}
	}


	/// <summary>
	/// <para type="synopsis">Configures color mappings for file and directory output.</para>
	/// <para type="description">Use this cmdlet to configure color mappings for file and directory output.</para>
	/// <para type="description">The input parameter is an array of hashtables. Each hashtable has two members: 
	/// Match and Color.</para>
	/// <para type="description">Match can be one of the following:
	///    * An array of strings, which are matched against extensions
	///    * A System.IO.FileAttributes (or a combination), which are matched against file/dir attributes
	///    * A regular expression, which is matched against file/dir name(just the name, not the full path)
	///    * A script block with a single System.IO.FileSystemInfo parameter returning bool.
	/// </para>
	/// <para type="description">When using Format-Custom to output files and directories 
	/// (e.g. using Get-ChildItems | Format-Custom), matching items will have ANSI foreground color codes embedded 
	/// in output strings. These codes can then be interpreted by ANSI-aware tools, like GNU less or AnsiCon.
	/// PowerShell 5 is able to interpret ANSI escape sequences natively.</para>
	/// 
	/// <example>
	/// <code>
	/// $fileSystemColors = @(
	/// # this entry will match directories
	/// @{
	///    Match = [System.IO.FileAttributes]::Directory;
	///    Color = [System.ConsoleColor]::Gray
	/// },
	/// # alternatively, we could've used a script block to match directories
	/// @{
	///    Match = { param($f) $f.PSIsContainer };
	///    Color = [System.ConsoleColor]::Gray
	/// },
	/// # this entry will match files with extensions log, err or out
	/// # (it won't get a chance to match directories, since they'll 
	/// # all be matched by the first entry)
	/// @{
	///    Match = @("log", "err", "out");
	///    Color = [System.ConsoleColor]::DarkGreen
	/// },
	/// # this entry will match all files with a leading . in the name
	/// @{
	///    Match = [regex] "^\..*";
	///    Color = [System.ConsoleColor]::DarkGray
	/// }
	/// )
	/// 
	/// Set-FileSystemColors -Colors $fileSystemColors
	/// </code>
	/// <para>A simple example demonstrating supported matchers</para>
	/// </example>
	/// </summary>
	[Cmdlet(VerbsCommon.Set, AnsiColorNouns.FileSystemColors)]
	public class SetFileSystemColors : SetMatcherColors<FileSystemInfo> { }


	/// <summary>
	/// <para type="synopsis">Gets a copy of currently configured color mappings for file and directory output.</para>
	/// <para type="description">This cmdlet will return a copy of currently configured color mappings for file 
	/// and directory output.</para>
	/// </summary>
	[Cmdlet(VerbsCommon.Get, AnsiColorNouns.FileSystemColors)]
	public class GetFileSystemColors : GetMatcherColors<FileSystemInfo> { }


	/// <summary>
	/// <para type="synopsis">Configures color mappings for process output.</para>
	/// <para type="description">Use this cmdlet to configure color mappings for process output.</para>
	/// <para type="description">The input parameter is an array of hashtables. Each hashtable has two members: 
	/// Match and Color.</para>
	/// <para type="description">Match can be one of the following:
	///    * ProcessPriorityClass (Idle, BelowNormal, Normal, AboveNormal, High, RealTime)
	/// </para>
	/// <para type="description">When using Format-XXX to output process data (e.g. using Get-Process | Format-XXX), 
	/// matching items will have ANSI foreground color codes embedded in output strings. These codes can then be 
	/// interpreted by ANSI-aware tools, like GNU less or AnsiCon. PowerShell 5 is able to interpret ANSI escape 
	/// sequences natively.</para>
	/// 
	/// <example>
	/// <code>
	/// $processColors = @(
	/// 	@{
	/// 		Match = "Idle"
	/// 		Color = [System.ConsoleColor]::DarkMagenta
	/// 	}
	/// 	@{
	/// 		Match = "BelowNormal"
	/// 		Color = [System.ConsoleColor]::DarkGray
	/// 	}
	/// 	@{
	/// 		Match = "BelowNormal"
	/// 		Color = [System.ConsoleColor]::DarkCyan
	/// 	}
	/// 	@{
	/// 		Match = "Normal"
	/// 		Color = [System.ConsoleColor]::White
	/// 	}
	/// 	@{
	/// 		Match = "AboveNormal"
	/// 		Color = [System.ConsoleColor]::Green
	/// 	}
	/// 	@{
	/// 		Match = "High"
	/// 		Color = [System.ConsoleColor]::Yellow
	/// 	}
	/// 	@{
	/// 		Match = "RealTime"
	/// 		Color = [System.ConsoleColor]::Red
	/// 	}
	/// )
	/// 
	/// Set-ProcessColors -Colors $processColors
	/// </code>
	/// <para>A simple example demonstrating supported matchers</para>
	/// </example>
	/// </summary>
	[Cmdlet(VerbsCommon.Set, AnsiColorNouns.ProcessColors)]
	public class SetProcessColors : SetMatcherColors<System.Diagnostics.Process> { }


	/// <summary>
	/// <para type="synopsis">Gets a copy of currently configured color mappings for process output.</para>
	/// <para type="description">This cmdlet will return a copy of currently configured color mappings for 
	/// process output.</para>
	/// </summary>
	[Cmdlet(VerbsCommon.Get, AnsiColorNouns.ProcessColors)]
	public class GetProcessColors : GetMatcherColors<FileSystemInfo> { }


	/// <summary>
	/// <para type="synopsis">Configures attribute string pattern for file and directory custom property, ExtendedMode.</para>
	/// <para type="description">Configures attribute string pattern for file and directory custom property, ExtendedMode. It can also be used to reset current pattern to the default one.</para>
	/// <para type="description">Currently supported attributes:
	///  A - Archive
	///  C - Compressed
	///  D - Directory
	///  E - Encrypted
	///  H - Hidden
	///  G - IntegrityStream
	///  N - Normal
	///  B - NoScrubData
	///  I - NotContentIndexed
	///  O - Offline
	///  R - ReadOnly
	///  J - ReparsePoint
	///  P - SparseFile
	///  S - System
	///  T - Temporary
	/// </para>
	/// 
	/// <example>
	/// <code>
	/// Set-FileInfoAttributePattern -AttributePattern DARHS
	/// </code>
	/// <para>Setting a custom parameter to indicated directories, archive, read=only, hidden and system attributes</para>
	/// </example>
	/// 
	/// <example>
	/// <code>
	/// Set-FileInfoAttributePattern -DefaultPattern
	/// </code>
	/// <para>Setting the default pattern</para>
	/// </example>
	/// </summary>
	[Cmdlet(VerbsCommon.Set, AnsiColorNouns.FileInfoAttributePattern)]
	public class SetFileInfoAttributePattern : PSCmdlet {

		string mAttributePattern;

		/// <summary>
		/// <para type="description">Used to specify custom attribute pattern for ExtendedMode property</para>
		/// </summary>
		[Parameter(Position = 0, Mandatory = true)]
		public string AttributePattern {
			get { return mAttributePattern; }
			set { mAttributePattern = value; }
		}

		SwitchParameter mDefaultPattern;

		/// <summary>
		/// <para type="description">Resets the ExtendedMode attribute pattern to the default one.</para>
		/// </summary>
		[Parameter(Position = 1)]
		public SwitchParameter DefaultPattern {
			get { return mDefaultPattern; }
			set { mDefaultPattern = value; }
		}

		protected override void EndProcessing() {
			FileSystemInfoExtensions.SetFileInfoAttributePattern(this);
		}
	}


	/// <summary>
	/// <para type="synopsis">Returns currently configured attribute string pattern for file and directory custom property, ExtendedMode.</para>
	/// <para type="description">Returns currently configured attribute string pattern for file and directory custom property, ExtendedMode.</para>
	/// <para type="description">Additionally, if the -ShowAll parameter is used, the cmdled will print out all supported attributes and their names.</para>
	/// <para type="description">Currently supported attributes:
	///  A - Archive
	///  C - Compressed
	///  D - Directory
	///  E - Encrypted
	///  H - Hidden
	///  G - IntegrityStream
	///  N - Normal
	///  B - NoScrubData
	///  I - NotContentIndexed
	///  O - Offline
	///  R - ReadOnly
	///  J - ReparsePoint
	///  P - SparseFile
	///  S - System
	///  T - Temporary
	/// </para>
	/// </summary>
	[Cmdlet(VerbsCommon.Get, AnsiColorNouns.FileInfoAttributePattern)]
	public class GetFileInfoAttributePattern : PSCmdlet {

		SwitchParameter mShowAll;
		/// <summary>
		/// <para type="description">If specified, the cmdled will print out all supported attributes and their names.</para>
		/// </summary>
		[Parameter(Position = 0)]
		public SwitchParameter ShowAll {
			get { return mShowAll; }
			set { mShowAll = value; }
		}
		protected override void EndProcessing() {
			WriteObject(FileSystemInfoExtensions.GetFileInfoAttributePattern(this));
		}
	}
}
