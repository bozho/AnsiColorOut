# set accelerators
$accel = [psobject].Assembly.GetType('System.Management.Automation.TypeAccelerators')
$accel::Add("AnsiColorOut", [Bozho.PowerShell.AnsiColorOut])

$matcherManager = New-Object Bozho.PowerShell.ColorMatcherManager
$matcherManager.RegisterMatchers([Bozho.PowerShell.AnsiColorOut].Assembly)

[Bozho.PowerShell.AnsiColorOut]::MatcherManager = $matcherManager

# add type extensions and custom formats
Update-TypeData -AppendPath $PSScriptRoot\types.ps1xml
Update-FormatData -AppendPath $PSScriptRoot\FileSystem.format.ps1xml
Update-FormatData -AppendPath $PSScriptRoot\Process.format.ps1xml


