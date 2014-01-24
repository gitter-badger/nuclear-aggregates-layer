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

#	Закомментировано, т.к. в версии 2.0-BreakingBad-Season2 нет компонента Research
#	$assemblyInfoPath5 = Join-Path $global:Context.Dir.Solution '..\..\Research\Source\AssemblyInfo.Version.cs'
#	Update-AssemblyInfo $assemblyInfoPath5 $global:Context.Version
}