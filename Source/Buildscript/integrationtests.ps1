Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Build-IntegrationTests {

	$projectFileName = Get-ProjectFileName '.\Tests' '2Gis.Erm.Tests.Integration.InProc'
	
	Transform-AppConfig $projectFileName
	
	$publishProfileName = $global:Context.EnvironmentName
	$packageFileName = Join-Path $global:Context.Dir.Artifacts 'integrationtests'

	Invoke-MSBuild @(
	"""$projectFileName"""
	"/p:PublishProfileName=$publishProfileName"
	"/p:OutDir=$packageFileName"
	)
}

Task Run-IntegrationTests -Depends Build-IntegrationTests {
	$packageFileName = Join-Path $global:Context.Dir.Artifacts 'integrationtests'
	$integrationTestsExePath = Join-Path $packageFileName '2Gis.Erm.Tests.Integration.InProc.exe'
	$arguments = @("-buildscript")
	
	& $integrationTestsExePath $arguments
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}