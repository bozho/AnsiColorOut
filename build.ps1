Add-Type -AssemblyName System.IO.Compression.FileSystem


$files = @(
"AnsiColorOut\bin\Release\AnsiColorOut.psm1",
"AnsiColorOut\bin\Release\AnsiColorOut.psd1",
"AnsiColorOut\bin\Release\Bozho.PowerShell.AnsiColorOut.dll",
"AnsiColorOut\bin\Release\*.ps1",
"AnsiColorOut\bin\Release\*.ps1xml",
"AnsiColorOut\bin\Release\en-US"
"README.md"
)


if(Test-Path "$env:ProgramFiles\MSBuild\14.0\Bin\MSBuild.exe") {
	$msbuild = "$env:ProgramFiles\MSBuild\14.0\Bin\MSBuild.exe"
}
else {
	$msbuild = "${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"

}


& "$msbuild" $PSScriptRoot\AnsiColorOut.sln /t:Rebuild /p:Configuration=Release

try {

	$tempDir = "$env:Temp\$([Guid]::NewGuid())"
	$stagingDir = New-Item "$tempDir\AnsiColorOut" -Type Directory -Force

	$files | % {
		Copy-Item $PSScriptRoot\$_ $stagingDir -Recurse
	}

	del $PSScriptRoot\AnsiColorOut.zip -ErrorAction Ignore
	[System.IO.Compression.ZipFile]::CreateFromDirectory($tempDir, "$PSScriptRoot\AnsiColorOut.zip")

}
finally {
	Remove-Item $tempDir -Force -Recurse
}
