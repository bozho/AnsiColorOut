# set accelerators
$accel = [psobject].Assembly.GetType('System.Management.Automation.TypeAccelerators')
$accel::Add("AnsiColorOut", [Bozho.PowerShell.AnsiColorOut])

$matcherManager = New-Object Bozho.PowerShell.ColorMatcherManager
$matcherManager.RegisterMatchers([Bozho.PowerShell.AnsiColorOut].Assembly)

[Bozho.PowerShell.AnsiColorOut]::MatcherManager = $matcherManager

# add type extensions and custom formats
Get-ChildItem -Path "$PSScriptRoot\types\*.ps1xml" | % { Update-TypeData -AppendPath $_ }
Get-ChildItem -Path "$PSScriptRoot\formats\*.ps1xml" | % { Update-FormatData -AppendPath $_ }
