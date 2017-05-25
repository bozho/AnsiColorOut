# AnsiColorOut

## Introduction

AnsiColorOut is an attempt at a different approach to coloring PowerShell output. 

It uses custom formatting files used with `Format-XXX` cmdlets to write output with embedded ANSI 
color escape sequences and relies on PowerShell 5's built-in support for ANSI escape sequences, or
external utilities, like GNU less or [AnsiCon](https://github.com/adoxa/ansicon) to interpret them.

Currently, AnsiColorOut supports `FileInfo/DirectoryInfo` (e.g. Get-ChildItem output for drives)
and `Process` (e.g. Get-Process) formatting.

The module also expands `FileInfo` and `DirectoryInfo` types with the `ExtendedMode` property, 
which can show all suported file/directory attributes. `ExtendedMode` output format is 
configurable.



## Installation

Unzip the zip file to your modules directory.



## Configuration

### FileInfo/DirInfo

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

`Match` is an object used to match a file/dir to a console color. The first matching entry is used 
to determine the text color.

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


### Process

Process color output is configured using the `Set-ProcessColors` cmdlet: 

~~~
Set-ProcessColors -Colors $processColors
~~~

The input parameter is an array of Hashtables:
~~~
$processColors = @(
    @{
        Match = ...;
        Color = [System.ConsoleColor]::Gray
    },
    ...
)
~~~

`Match` is an object used to match a process to a console color. The first matching entry is used 
to determine the text color.

`Match` can be:

* One of the `[ProcessPriorityClass]` enum values
* A string corresponding to one of the `[ProcessPriorityClass]` enum value names

For example:

~~~
$processColors = @(
    @{
        Match = [System.Diagnostics.ProcessPriorityClass]::Idle
        Color = [System.ConsoleColor]::DarkMagenta
    }
    @{
        Match = "BelowNormal"
        Color = [System.ConsoleColor]::DarkGray
    }
    @{
        Match = "BelowNormal"
        Color = [System.ConsoleColor]::DarkCyan
    }
    @{
        Match = "Normal"
        Color = [System.ConsoleColor]::White
    }
    @{
        Match = "AboveNormal"
        Color = [System.ConsoleColor]::Green
    }
    @{
        Match = "High"
        Color = [System.ConsoleColor]::Yellow
    }
    @{
        Match = "RealTime"
        Color = [System.ConsoleColor]::Red
    }
)
~~~

For a sample configurations, please look at the `SampleProfile.ps1` file.


## Custom formatting files

The module is distributed with two custom formatting files: `FileSystem.format.ps1xml` and
`Process.format.ps1xml`. They define custom views for formatting output.

The files are automatically loaded when importing the module. The views are used in corresponding
Format-XXX cmdlet. E.g.

`Get-ChildItem C:\Windows | Format-Custom -View AnsiColorView`

You can also use them as templates for writing your own custom formatting files.


## Known issues and limitations

The color code strings may get line-wrapped if the console window is not wide enough, which will 
result in corrupt output.


## License

The MIT License (MIT)

Copyright (c) 2016-2017 Marko Bozikovic

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
