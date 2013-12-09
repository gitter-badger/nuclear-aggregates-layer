Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\versioning.psm1 -DisableNameChecking

Task Update-AssemblyInfo {

	$assemblyInfoPath1 = Join-Path $global:Context.Dir.Solution 'AssemblyInfo.Version.cs'
	Update-AssemblyInfo -AssemblyInfoPath $assemblyInfoPath1 -Version $global:Context.Version

	$assemblyInfoPath2 = Join-Path $global:Context.Dir.Solution '..\..\BL\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo -AssemblyInfoPath $assemblyInfoPath2 -Version $global:Context.Version

	$assemblyInfoPath3 = Join-Path $global:Context.Dir.Solution '..\..\BLFlex\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo -AssemblyInfoPath $assemblyInfoPath3 -Version $global:Context.Version

	$assemblyInfoPath4 = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo -AssemblyInfoPath $assemblyInfoPath4 -Version $global:Context.Version

	$assemblyInfoPath5 = Join-Path $global:Context.Dir.Solution '..\..\Research\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo -AssemblyInfoPath $assemblyInfoPath5 -Version $global:Context.Version
}