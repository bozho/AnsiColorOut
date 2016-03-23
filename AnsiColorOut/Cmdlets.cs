using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Bozho.PowerShell {

	[Cmdlet("Set", "FileSystemColors", HelpUri = "http://foo.bar")]
	public class SetFileSystemColors : PSCmdlet {

		object[] mColors;
		[Parameter(ParameterSetName = "", Position = 0, Mandatory = true)]
		public object[] Colors {
			get { return mColors; }
			set { mColors = value; }
		}

		protected override void EndProcessing() {
			AnsiColorOut.SetFileSystemColors(mColors);
		}
	}


	[Cmdlet("Get", "FileSystemColors", HelpUri = "http://foo.bar")]
	public class GetFileSystemColors : PSCmdlet {

		protected override void EndProcessing() {
			// The second parameter in WriteObject tells PS to enumerate the array,
			// sending one object at a time to the pipeline.
			WriteObject(AnsiColorOut.GetFileSystemColors(), true);
		}
	}


	[Cmdlet("Set", "FileInfoAttributePattern", HelpUri = "http://foo.bar")]
	public class SetFileInfoAttributePattern : PSCmdlet {

		string mAttributePattern;
		[Parameter(ParameterSetName = "", Position = 0)]
		public string AttributePattern {
			get { return mAttributePattern; }
			set { mAttributePattern = value; }
		}

		SwitchParameter mDefaultPattern;
		[Parameter(ParameterSetName = "")]
		public SwitchParameter DefaultPattern {
			get { return mDefaultPattern; }
			set { mDefaultPattern = value; }
		}

		protected override void EndProcessing() {
			FileSystemInfoExtensions.SetFileInfoAttributePattern(this);
		}
	}


	[Cmdlet("Get", "FileInfoAttributePattern", HelpUri = "http://foo.bar")]
	public class GetFileInfoAttributePattern : PSCmdlet {

		SwitchParameter mShowAll;
		[Parameter(ParameterSetName = "")]
		public SwitchParameter ShowAll {
			get { return mShowAll; }
			set { mShowAll = value; }
		}
		protected override void EndProcessing() {
			WriteObject(FileSystemInfoExtensions.GetFileInfoAttributePattern(this));
		}
	}
}
