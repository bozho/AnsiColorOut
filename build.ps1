Add-Type -AssemblyName System.IO.Compression.FileSystem


$files = @(
"AnsiColorOut\bin\Release\AnsiColorOut.psm1",
"AnsiColorOut\bin\Release\AnsiColorOut.psd1",
"AnsiColorOut\bin\Release\Bozho.PowerShell.AnsiColorOut.dll",
"AnsiColorOut\bin\Release\FileSystem.format.ps1xml",
"AnsiColorOut\bin\Release\types.ps1xml",
"AnsiColorOut\help\en-US"

)


msbuild $PSScriptRoot\AnsiColorOut.sln /t:Rebuild /p:Configuration=Release

try {

	$tempDir = "$env:Temp\$([Guid]::NewGuid())"
	$stagingDir = New-Item "$tempDir\AnsiColorOut" -Type Directory -Force

	$files | % {
		copy $PSScriptRoot\$_ $stagingDir -Recurse
	}

	del $PSScriptRoot\AnsiColorOut.zip -ErrorAction Ignore
	[System.IO.Compression.ZipFile]::CreateFromDirectory($tempDir, "$PSScriptRoot\AnsiColorOut.zip")

}
finally {
	Remove-Item $tempDir -Force -Recurse
}
