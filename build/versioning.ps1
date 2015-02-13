Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\versioning.psm1" -DisableNameChecking

Task Set-BuildNumber {
	$version = Get-Version
	
	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[buildNumber '$($version.SemanticVersion)']"
	}
}

Task Update-AssemblyInfo {
	$globalDir = Join-Path $global:Context.Dir.Solution '..\..'
	$assemblyInfos = Get-ChildItem $globalDir -Filter 'AssemblyInfo.Version.cs' -Recurse
	Update-AssemblyInfo $assemblyInfos
}