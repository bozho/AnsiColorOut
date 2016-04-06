using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Bozho.PowerShell {

	internal static class AnsiColorNouns {
		internal const string FileSystemColors = "FileSystemColors";
		internal const string FileInfoAttributePattern = "FileInfoAttributePattern";
	}

	[Cmdlet(VerbsCommon.Set, AnsiColorNouns.FileSystemColors, HelpUri = "http://foo.bar")]
	public class SetFileSystemColors : PSCmdlet {

		object[] mColors;
		[Parameter(Position = 0, Mandatory = true)]
		public object[] Colors {
			get { return mColors; }
			set { mColors = value; }
		}

		protected override void EndProcessing() {
			AnsiColorOut.SetFileSystemColors(mColors);
		}
	}


	[Cmdlet(VerbsCommon.Get, AnsiColorNouns.FileSystemColors, HelpUri = "http://foo.bar")]
	public class GetFileSystemColors : PSCmdlet {

		protected override void EndProcessing() {
			// The second parameter in WriteObject tells PS to enumerate the array,
			// sending one object at a time to the pipeline.
			WriteObject(AnsiColorOut.GetFileSystemColors(), true);
		}
	}


	[Cmdlet(VerbsCommon.Set, AnsiColorNouns.FileInfoAttributePattern, HelpUri = "http://foo.bar")]
	public class SetFileInfoAttributePattern : PSCmdlet {

		string mAttributePattern;
		[Parameter(Position = 0, Mandatory = true)]
		public string AttributePattern {
			get { return mAttributePattern; }
			set { mAttributePattern = value; }
		}

		SwitchParameter mDefaultPattern;
		[Parameter(Position = 1)]
		public SwitchParameter DefaultPattern {
			get { return mDefaultPattern; }
			set { mDefaultPattern = value; }
		}

		protected override void EndProcessing() {
			FileSystemInfoExtensions.SetFileInfoAttributePattern(this);
		}
	}


	[Cmdlet(VerbsCommon.Get, AnsiColorNouns.FileInfoAttributePattern, HelpUri = "http://foo.bar")]
	public class GetFileInfoAttributePattern : PSCmdlet {

		SwitchParameter mShowAll;
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
