Add-Type -AssemblyName System.IO.Compression.FileSystem


$files = @(
"AnsiColorOut\bin\Release\AnsiColorOut.psm1",
"AnsiColorOut\bin\Release\AnsiColorOut.psd1",
"AnsiColorOut\bin\Release\Bozho.PowerShell.AnsiColorOut.dll",
"AnsiColorOut\bin\Release\FileSystem.format.ps1xml",
"AnsiColorOut\bin\Release\types.ps1xml"
)


msbuild $PSScriptRoot\AnsiColorOut.sln /t:Rebuild /p:Configuration=Release

try {

	$stagingDir = New-Item "$env:Temp\$([Guid]::NewGuid())\AnsiColorOut" -Type Directory -Force

	$files | % {
		copy $PSScriptRoot\$_ $stagingDir
	}

	del $PSScriptRoot\AnsiColorOut.zip -ErrorAction Ignore
	[System.IO.Compression.ZipFile]::CreateFromDirectory($stagingDir, "$PSScriptRoot\AnsiColorOut.zip")

}
finally {
	Remove-Item $stagingDir -Force -Recurse
}
