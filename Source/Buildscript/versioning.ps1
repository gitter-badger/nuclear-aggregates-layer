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
	$assemblyInfos = Get-ChildItem $global:Context.Dir.Solution -Filter 'AssemblyInfo.Version.cs' -Recurse
	Update-AssemblyInfo $assemblyInfos
}