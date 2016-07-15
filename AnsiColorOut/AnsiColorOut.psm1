# set accelerators
$accel = [psobject].Assembly.GetType('System.Management.Automation.TypeAccelerators')
$accel::Add("AnsiColorOut", [Bozho.PowerShell.AnsiColorOut])

# add type extensions and custom formats
Update-TypeData -AppendPath $PSScriptRoot\types.ps1xml
Update-FormatData -AppendPath $PSScriptRoot\FileSystem.format.ps1xml


