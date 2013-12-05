Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking

Task Build-Migrations -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.Migrator'
	Build-MigrationProject $projectFileName
	
	$projectFileName = Get-ProjectFileName '..\..\BL\Source\Data\' '2Gis.Erm.BL.DB.Migrations'
	Build-MigrationProject $projectFileName
}

Task Deploy-MigrationsErmOnly -Depends Build-Migrations {
	
	$server = 'uk-sql01.2gis.local'
	$database = 'Erm22'
	$connectionString = "Data Source=$server;Initial Catalog=$database;Integrated Security=True"

	Deploy-Migrations @(
		"-ermonly=$connectionString"
		"-u"
	)
}

Task Deploy-Migrations -Depends Build-Migrations {
	$webAppInfo = Get-WebAppInfo

	Deploy-Migrations @(
		"-config=""$($webAppInfo.TransformedConfigFileName)"""
		"-u"
	)
}

function Deploy-Migrations ($arguments) {
	$packageFileName = Get-PackageFileName 'migrations'
	$migrationsExePath = Join-Path $packageFileName '2Gis.Erm.Migrator.exe'
	
	Exec {
		& $migrationsExePath $arguments
	}
}

function Build-MigrationProject($ProjectFileName){

	$packageFileName = Get-PackageFileName 'migrations'

	Invoke-MSBuild -Arguments @(
	"""$ProjectFileName"""
	"/p:OutDir=$packageFileName"
	)
	
	Write-Output "##teamcity[publishArtifacts '$packageFileName => Database migrations']"
}