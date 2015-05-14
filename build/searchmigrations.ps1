Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

Task Build-SearchMigrations -Precondition { (Get-Metadata 'Migrations').RunElasticsearchMigrations } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.Qds.Migrator'
	$configFileName = Join-Path (Split-Path $ProjectFileName) 'app.config'
	$configXml = Transform-Config $configFileName
	Build-SearchMigrationProject $projectFileName -Properties @{ 'AppConfig' = 'app.transformed.config' } -CustomXmls $configXml
}

Task Deploy-SearchMigrations -Precondition { (Get-Metadata 'Migrations').RunElasticsearchMigrations } {
	$artifactName = Get-Artifacts 'Search Migrations'
	
	$migrationsExePath = Join-Path $artifactName '2Gis.Erm.Qds.Migrator.exe'
	& $migrationsExePath
	
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

function Build-SearchMigrationProject($ProjectFileName, $Properties, $CustomXmls){

	$buildFileName = Create-BuildFile $ProjectFileName -Properties $Properties -CustomXmls $CustomXmls
	Invoke-MSBuild $buildFileName

	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Search Migrations'
}