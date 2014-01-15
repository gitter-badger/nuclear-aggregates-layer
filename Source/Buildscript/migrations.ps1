Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Build-Migrations -Precondition { return $OptionWebApp } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.Migrator'
	Build-MigrationProject $projectFileName

	$projectFileName = Get-ProjectFileName '..\..\BLCore\Source\Data\' '2Gis.Erm.BLCore.DB.Migrations'
	Build-MigrationProject $projectFileName
	
	$projectFileName = Get-ProjectFileName '..\..\BL\Source\Data\' '2Gis.Erm.BL.DB.Migrations'
	Build-MigrationProject $projectFileName
}

Task Deploy-Migrations -Precondition { return $OptionWebApp } -Depends Build-Migrations {

	$artifactName = Get-Artifacts 'Database Migrations'
	$migrationsExePath = Join-Path $artifactName '2Gis.Erm.Migrator.exe'
	
	& $migrationsExePath @(
		"-environment=$($global:Context.EnvironmentName)"
		"-update"
	)
	
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

function Build-MigrationProject($ProjectFileName){

	Invoke-MSBuild @(
	"""$ProjectFileName"""
	)

	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Database Migrations'
}