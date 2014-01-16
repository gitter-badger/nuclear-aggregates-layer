Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Build-IntegrationTests {

	$projectFileName = Get-ProjectFileName '.\Tests' '2Gis.Erm.Tests.Integration.InProc'
	
	try {
		Transform-AppConfig $projectFileName
		
		$publishProfileName = $global:Context.EnvironmentName

		Invoke-MSBuild @(
		"""$projectFileName"""
		"/p:PublishProfileName=$publishProfileName"
		)	
	}
	finally {
		Restore-AppConfig $projectFileName	
	}

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