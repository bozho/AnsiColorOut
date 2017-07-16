# AnsiColorOut

## Introduction

AnsiColorOut is an attempt at a different approach to coloring PowerShell output. 

It uses custom formatting files used with `Format-XXX` cmdlets to write output with embedded ANSI 
color escape sequences and relies on PowerShell 5's built-in support for ANSI escape sequences, or
external utilities, like GNU less or [AnsiCon](https://github.com/adoxa/ansicon) to interpret them.

Currently, AnsiColorOut supports `FileInfo/DirectoryInfo` (e.g. `Get-ChildItem` output for drives)
and `Process` (e.g. `Get-Process`) formatting.

The module also expands `FileInfo` and `DirectoryInfo` types with the `ExtendedMode` property, 
which can show all suported file/directory attributes. `ExtendedMode` output format is 
configurable.


## Installation

Install the module from the PowerShell [Gallery](https://www.powershellgallery.com/packages/AnsiColorOut): 
```
Install-Module -Name AnsiColorOut
```

Download the file from the releases page and unzip the zip file to your modules directory.


## Changelog

You can find full change log [here](./CHANGELOG.md).


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

* An array of file extension strings (**without** leading `.`) - only matched against files
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

The files are automatically loaded when importing the module. The views are used in appropriate
Format-XXX cmdlets.

### Files/directories

`FileSystem.format.ps1xml` contains table, wide and custom views, all named "ansi".

`Get-ChildItem C:\Windows | Format-Table -View ansi`
`Get-ChildItem C:\Windows | Format-Wide -View ansi`
`Get-ChildItem C:\Windows | Format-Custom -View ansi`


### Process

`Process.format.ps1xml` contains **ADD VIEWS HERE**


## Known issues and limitations

The color code strings may get line-wrapped if the console window is not wide enough, which will 
result in corrupt output.

If you are piping Format-XXX output into something like `less -rEX`, custom table output may get 
corrupt, because custom formats print out ANSI escape sequence strings, which are not interpreted 
by PowerShell, but passed on to `less`.

It is recommended to use custom views in those cases.

At the time of this writing, it looks like Cygwin programs (e.g. less.exe) clear 
```ENABLE_VIRTUAL_TERMINAL_PROCESSING``` standard output flag when they run, which disables 
ANSI escape sequence processing in PowerShell. This module provides a helper method to re-enable 
the flag.

If you're using Cygwin utilities from PowerShell, you might want to add the following call to 
your custom prompt function (which will ensure that the flag is set whenever the prompt is
rendered):

```[Bozho.PowerShell.Console]::EnableVirtualTerminalProcessing()```



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
