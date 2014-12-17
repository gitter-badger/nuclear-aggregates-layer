﻿Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\transform.psm1 -DisableNameChecking

Task Build-Migrations -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.Migrator'
	$configFileName = Join-Path (Split-Path $ProjectFileName) 'app.config'
	$configXml = Transform-Config $configFileName
	Build-MigrationProject $projectFileName -Properties @{ 'AppConfig' = 'app.transformed.config' } -CustomXmls $configXml
	
	$projectFileName = Get-ProjectFileName '..\..\BLCore\Source\Data\' '2Gis.Erm.BLCore.DB.Migrations'
	Build-MigrationProject $projectFileName
	
	$projectFileName = Get-ProjectFileName '..\..\BL\Source\Data\' '2Gis.Erm.BL.DB.Migrations'
	Build-MigrationProject $projectFileName
}

Task Deploy-Migrations -Depends Build-Migrations {

	$artifactName = Get-Artifacts 'Database Migrations'
	$migrationsExePath = Join-Path $artifactName '2Gis.Erm.Migrator.exe'
	
	& $migrationsExePath @(
		"-update"
	)
	
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

function Build-MigrationProject($ProjectFileName, $Properties, $CustomXmls) {

	Invoke-MSBuild $ProjectFileName -Properties $Properties -CustomXmls $CustomXmls

	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Database Migrations'
}