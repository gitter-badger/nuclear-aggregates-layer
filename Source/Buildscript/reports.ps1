Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking
Import-Module .\modules\sqlserver.psm1 -DisableNameChecking
Import-Module .\modules\config.psm1 -DisableNameChecking

Properties { $OptionReports=$false }
Task Deploy-Reports -precondition { return $OptionReports } -Depends `
Deploy-ReportProject, `
Replace-ReportsStoredProcs

Task Deploy-ReportProject {

	$projectFileName = Get-ProjectFileName '..\..\' 'ErmReports' '.rptproj'
	Process-Rds $projectFileName
	
	& .\Reporting\Deploy-SSRSProject.ps1 -Path $projectFileName -Configuration $global:Context.EnvironmentName
}

Task Replace-ReportsStoredProcs {
	# hack для российской бизнес-модели
	$region = $global:Context.Region
	if ($region -eq 'RU'){
		$region = ''
	}

	$ermReports = Get-ConnectionString 'ErmReports'
	$ermReportsBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$ermReportsBuilder.set_ConnectionString($ermReports)
	Replace-StoredProcs $ermReports "\bErmReports$region[0-9]*\b" $ermReportsBuilder['Initial Catalog']
	
	$erm = Get-ConnectionString 'Erm'
	$ermBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$ermBuilder.set_ConnectionString($erm)
	Replace-StoredProcs $ermReports "\bErm$region[0-9]*\b" $ermBuilder['Initial Catalog']
	
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	$uriBuilder = New-Object System.UriBuilder('https', $entryPointMetadata.IisAppPath)

	Replace-StoredProcs $ermReports '(http|https)://((?!(www.w3.org)|(schemas.microsoft.com)|(schemas.xmlsoap.org))).*?/' $uriBuilder.ToString()
}

function Process-Rds($ProjectFileName){

	$projectDir = Split-Path $ProjectFileName -Parent

	$ermConString = Get-ConnectionString 'Erm'
	
	$ermRdsFileName = Join-Path $projectDir 'Erm.rds'
	Update-RdsConnectionString $ermRdsFileName $ermConString

	# заменяем datasource для ErmReports.rds
	$ermReportsConString = Get-ConnectionString 'ErmReports'
	$ermReportsRdsFileName = Join-Path $projectDir 'ErmReports.rds'
	Update-RdsConnectionString $ermReportsRdsFileName $ermReportsConString

	# заменяем datasource для MoDi.rds
	$modiUriString = Get-ServiceUriString 'MoDiService'
	$uriBuilder = New-Object System.UriBuilder($modiUriString)
	$uriBuilder.Path = 'Reports.svc'
	
	$modiRdsFileName = Join-Path $projectDir 'MoDi.rds'
	Update-RdsConnectionString $modiRdsFileName $uriBuilder.ToString()
}

function Update-RdsConnectionString($RdsFileName, $ConnectionString){
	$rdsFile = Get-Item $RdsFileName
	$rdsFile.IsReadOnly = $false

	[xml]$xml = Get-Content -Path $RdsFileName -Encoding UTF8
	$xml.RptDataSource.ConnectionProperties.ConnectString = $ConnectionString

	$xml.Save($RdsFileName)
}