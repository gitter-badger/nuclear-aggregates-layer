Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\versioning.psm1 -DisableNameChecking

Task Set-BuildNumber {
	$version = Get-Version
	
	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[buildNumber '$($version.SemanticVersion)']"
	}
}

Task Update-AssemblyInfo {

	Update-AssemblyInfo @(
		(Join-Path $global:Context.Dir.Solution 'AssemblyInfo.Version.cs')
		(Join-Path $global:Context.Dir.Solution '..\..\BLCore\Source\AssemblyInfo.Version.cs')
		(Join-Path $global:Context.Dir.Solution '..\..\BL\Source\AssemblyInfo.Version.cs')
		(Join-Path $global:Context.Dir.Solution '..\..\BLFlex\Source\AssemblyInfo.Version.cs')
		(Join-Path $global:Context.Dir.Solution '..\..\Platform\Source\AssemblyInfo.Version.cs')
		(Join-Path $global:Context.Dir.Solution '..\..\BLQuerying\Source\AssemblyInfo.Version.cs')
	)
}