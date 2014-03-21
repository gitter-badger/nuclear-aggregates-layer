Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\transform.psm1 -DisableNameChecking

Task Build-SearchMigrations -Precondition { return $OptionWebApp } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.Qds.Migrator'
	Transform-AppConfig $projectFileName
	try{
		Build-SearchMigrationProject $projectFileName
	}
	finally{
		Restore-AppConfig $projectFileName
	}
}

Task Deploy-SearchMigrations -Precondition { return $OptionWebApp } -Depends Build-SearchMigrations {

	$artifactName = Get-Artifacts 'Search Migrations'
	$migrationsExePath = Join-Path $artifactName '2Gis.Erm.Qds.Migrator.exe'
	
	& $migrationsExePath
	
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

function Build-SearchMigrationProject($ProjectFileName){

	Invoke-MSBuild @(
	"""$ProjectFileName"""
	)

	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Search Migrations'
}