Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking

Task Build-IntegrationTests {

	$projectFileName = Get-ProjectFileName '.\Tests' '2Gis.Erm.Tests.Integration.InProc'
	$projectDir = Split-Path $projectFileName
	
	$configFileName = Join-Path $projectDir 'app.config'
	$configXml = Transform-Config $configFileName
	$buildFileName = Create-BuildFile $projectFileName -Properties @{ 'AppConfig' = 'app.transformed.config' } -CustomXmls $configXml
	Invoke-MSBuild $buildFileName

	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Integration Tests'
}

Task Run-IntegrationTests -Depends Build-IntegrationTests {
	$artifactName = Get-Artifacts 'Integration Tests'
	$integrationTestsExePath = Join-Path $artifactName '2Gis.Erm.Tests.Integration.InProc.exe'
	$arguments = @("-buildscript")
	
	& $integrationTestsExePath $arguments
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}