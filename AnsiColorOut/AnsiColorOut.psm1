Update-TypeData -AppendPath $PSScriptRoot\types.ps1xml
Update-FormatData -AppendPath $PSScriptRoot\FileSystem.format.ps1xml



$fileSystemColors = @(
	#@{
	#	Match = { param($f) $f.PSIsContainer };
	#	Color = [System.ConsoleColor]::Gray
	#},
	@{
		Match = [System.IO.FileAttributes]::Directory;
		Color = [System.ConsoleColor]::Gray
	},
	@{
		Match = @("arj", "bz", "bz2", "cab", "gz", "lha", "lzh", "rar", "tar", "taz", "tgz", "tbz2", "uc?", "uu", "z", "zip");
		Color = [System.ConsoleColor]::Yellow
	},
	@{
		Match = @("bat", "btm", "cmd", "com", "exe", "lnk", "pl", "py", "vbs", "ps1", "psm1", "psd1");
		Color = [System.ConsoleColor]::Red
	},
	@{
		Match = @("ax", "cpl", "dat", "dll", "drv", "fnt", "fon", "fot", "ttf", "sys", "vxd", "386", "hdl", "fdl", "ldl");
		Color = [System.ConsoleColor]::DarkRed
	},
	@{
		Match = @("asm", "bas", "c", "cpp", "cs", "def", "h", "idl", "mak", "pas", "rc");
		Color = [System.ConsoleColor]::Magenta
	},
	@{
		Match = @("1st", "ans", "asc", "doc", "hlp", "lst", "man", "me", "pdf", "prn", "ps", "txt", "wri");
		Color = [System.ConsoleColor]::Blue
	},
	@{
		Match = @("cda", "mid", "mod", "mp3", "wav", "ogg", "s3m", "xm");
		Color = [System.ConsoleColor]::Cyan
	},
	@{
		Match = @("ani", "asf", "asx", "avi", "bmp", "cur", "gif", "ico", "iff", "jpeg", "jpg", "lbm", "mov", "mpeg", "mpg", "pcd", "pcx", "pic", "png", "rle", "rm", "tga", "tif", "tiff", "wmv", "mp4", "mkv", "flv");
		Color = [System.ConsoleColor]::DarkCyan
	},
	@{
		Match = @("asp", "htm", "html", "mht", "php", "xml");
		Color = [System.ConsoleColor]::DarkYellow
	},
	@{
		Match = @("cfg", "conf", "ctl", "dos", "grp", "ini", "inf", "pif", "reg");
		Color = [System.ConsoleColor]::Green
	},
	@{
		Match = @("log", "err", "out");
		Color = [System.ConsoleColor]::DarkGreen
	},
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


