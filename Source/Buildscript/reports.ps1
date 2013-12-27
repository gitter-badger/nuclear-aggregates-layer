Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking
Import-Module .\modules\sqlserver.psm1 -DisableNameChecking
Import-Module .\modules\godconfig.psm1 -DisableNameChecking

Properties{ $OptionReports=$false }
Task Deploy-Reports -precondition { return $OptionReports } -Depends `
Deploy-ReportProject, `
Replace-ReportsStoredProcs

Task Deploy-ReportProject {

	$projectFileName = Get-ProjectFileName '..\..\' 'ErmReports' '.rptproj'
	Process-Rds $projectFileName
	
	& .\Reporting\Deploy-SSRSProject.ps1 -Path $projectFileName -Configuration $global:Context.EnvironmentName
}

Task Replace-ReportsStoredProcs {
	$erm = Get-ConnectionString 'Erm'
	
	$builder = New-Object System.Data.Common.DbConnectionStringBuilder
	$builder.set_ConnectionString($erm)

	# hack для русской локали
	$region = $global:Context.Region
	if ($region -eq 'RU'){
		$region = ''
	}

	$ermReports = Get-ConnectionString 'ErmReports'
	Replace-StoredProcs $ermReports "\bErm$region[0-9]*\b" $builder['Initial Catalog']
	
	$publishProfile = Get-WebAppPublishProfile
	Replace-StoredProcs $ermReports '(http|https)://((?!(www.w3.org)|(schemas.microsoft.com)|(schemas.xmlsoap.org))).*?/' $publishProfile.Uri.ToString()
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