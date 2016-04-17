# AnsiColorOut

## Introduction

AnsiColorOut is an attempt at a different approach to coloring PowerShell output. 

It uses custom formatting (Format-Custom) to write output with embedded ANSI color escape codes
and relies on external utilities, like GNU less or [AnsiCon](https://github.com/adoxa/ansicon) to interpret them.

Currently, only `FileInfo` and `DirectoryInfo` support is implemented.

The module also expands `FileInfo` and `DirectoryInfo` types with the `ExtendedMode` property, which can show all suported
file/directory attributes. Its output format is configurable.



## Installation

Unzip the zip file to your modules directory.



## Configuration

File/dir color output is configured using the `Set-FileSystemColors` cmdlet: 

~~~
Set-FileSystemColors -Colors $fileSystemColors
~~~

The input parameter is an array of Hashtables:
~~~
$fileSystemColors = @(
    @{
        Match = ...;
        Color = [System.ConsoleColor]::Gray
    },
    ...
)
~~~

`Match` is an object used to match a file/dir to a console color. The first matching entry is used to determine the text color.

`Match` can be:

* An array of strings, which are matched against extensions
* A `System.IO.FileAttributes` (or a combination), which are matched against file/dir attributes
* A regular expression, which is matched against file/dir name (just the name, not the full path)
* A script block with a single `System.IO.FileSystemInfo` parameter returning `bool`.

For example:

~~~
$fileSystemColors = @(
    # this entry will match directories
    @{
        Match = [System.IO.FileAttributes]::Directory;
        Color = [System.ConsoleColor]::Gray
    },
    # alternatively, we could've used a script block to match directories
    @{
        Match = { param($f) $f.PSIsContainer };
        Color = [System.ConsoleColor]::Gray
    },
    # this entry will match files with extensions log, err or out
    # (it won't get a chance to match directories, since they'll 
    # all be matched by the first entry)
    @{
        Match = @("log", "err", "out");
        Color = [System.ConsoleColor]::DarkGreen
    },
    # this entry will match all files with a leading . in the name
    @{
        Match = [regex] "^\..*";
        Color = [System.ConsoleColor]::DarkGray
    }
)
~~~

For a sample configuration, please look at the `SampleProfile.ps1` file.



## Known issues and limitations

AnsiColorOut currently works by writing ANSI color codes to the output and relying on external utilities like GNU less
to interpret them. The color code strings may get line-wrapped if the console window is not wide enough, which will result 
in corrupt output.

