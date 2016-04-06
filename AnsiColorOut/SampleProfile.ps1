
$fileSystemColors = @(
	#@{
	#	Match = { param($f) $f.PSIsContainer };
	#	Color = [System.ConsoleColor]::Gray
	#},
	@{
		Match = [System.IO.FileAttributes]::Directory;
		Color = [System.ConsoleColor]::Gray
	},
	# archive/compressed
	@{
		Match = @("7z", "arj", "bz", "bz2", "cab", "gz", "iso", "lha", "lzh", "lzma", "rar", "tar", "taz", "tgz", "tbz2", "uu", "z", "zip");
		Color = [System.ConsoleColor]::Yellow
	},
	# executables/scripts
	@{
		Match = @("bat", "btm", "cmd", "com", "cpl", "exe", "lnk", "msi", "pl", "ps1", "psm1", "psd1", "py", "pyw", "vbs", "ws", "wsf");
		Color = [System.ConsoleColor]::Red
	},
	# libraries, fonts, data files, etc.
	@{
		Match = @("ax", "cpl", "dat", "dll", "drv", "fnt", "fon", "fot", "ttf", "sys", "vxd", "386", "hdl", "fdl", "ldl");
		Color = [System.ConsoleColor]::DarkRed
	},
	# source code and related
	@{
		Match = @("asm", "bas", "c", "cpp", "cs", "def", "h", "idl", "mak", "pas", "rc");
		Color = [System.ConsoleColor]::Magenta
	},
	# text files/documents/help
	@{
		Match = @("1st", "ans", "asc", "chm", "csv", "doc", "docx", "hlp", "lst", "man", "md", "me", "pdf", "prn", "ps", "txt", "wri", "xls", "xlsx");
		Color = [System.ConsoleColor]::Blue
	},
	# audio files
	@{
		Match = @("cda", "flac", "mid", "mod", "mp3", "wav", "ogg", "s3m", "wav", "xm");
		Color = [System.ConsoleColor]::Cyan
	},
	# images/video
	@{
		Match = @("ani", "asf", "asx", "avi", "bmp", "cur", "dng", "flv", "gif", "hdr", "ico", "iff", "jpeg", "jpg", "lbm", "mkv", "mov", "mp4", "mpeg", "mpg", "orf", "pcd", "pcx", "pic", "png", "raw", "rle", "rm", "tga", "tif", "tiff", "wmv");
		Color = [System.ConsoleColor]::DarkCyan
	},
	# "web" files
	@{
		Match = @("asax", "asp", "aspx", "htm", "html", "mht", "php", "xml", "xsl");
		Color = [System.ConsoleColor]::DarkYellow
	},
	# config and similar files
	@{
		Match = @("cfg", "conf", "ctl", "dos", "grp", "ini", "inf", "pif", "reg");
		Color = [System.ConsoleColor]::Green
	},
	# log/output files
	@{
		Match = @("log", "err", "out");
		Color = [System.ConsoleColor]::DarkGreen
	},
	# temporary/intermediate files
	@{
		Match = @("bak", "bsc", "exp", "idb", "ilk", "tmp");
		Color = [System.ConsoleColor]::DarkGray
	}
	@{
		Match = [regex] "^\..*";
		Color = [System.ConsoleColor]::DarkGray
	}
)


Set-FileSystemColors -Colors $fileSystemColors



function d {

	$totalFileSize = 0
	$fileCount = 0
	$directoryCount = 0
	Get-ChildItem @args -Force | 
	% {
		if ($_.PSIsContainer) {
			++$directoryCount
		}
		else {
			++$fileCount;
			$totalFileSize += $_.Length; 
		}
		$_ 
	} |
	Format-Custom -View AnsiColorView | 
	less -rEX

	Write-Host ([String]::Format("{0,20:N0} bytes in {1} files and {2} dirs`n", $totalFileSize, $fileCount, $directoryCount))
}

