Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\versioning.psm1 -DisableNameChecking

Task Update-AssemblyInfo {
	$assemblyInfoPath = Join-Path $global:Context.Dir.Solution 'AssemblyInfo.Version.cs'
	Update-AssemblyInfo -AssemblyInfoPath $assemblyInfoPath -Version $global:Context.Version
}