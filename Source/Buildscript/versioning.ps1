Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\versioning.psm1 -DisableNameChecking

Task Update-AssemblyInfo {

	$assemblyInfoPath1 = Join-Path $global:Context.Dir.Solution 'AssemblyInfo.Version.cs'
	Update-AssemblyInfo $assemblyInfoPath1 $global:Context.Version

	$assemblyInfoPath2 = Join-Path $global:Context.Dir.Solution '..\..\BLCore\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo $assemblyInfoPath2 $global:Context.Version

	$assemblyInfoPath3 = Join-Path $global:Context.Dir.Solution '..\..\BLFlex\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo $assemblyInfoPath3 $global:Context.Version

	$assemblyInfoPath4 = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo $assemblyInfoPath4 $global:Context.Version

	$assemblyInfoPath5 = Join-Path $global:Context.Dir.Solution '..\..\BLQuerying\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo $assemblyInfoPath5 $global:Context.Version
}