Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\sqlserver.psm1 -DisableNameChecking

Properties{ $OptionReports=$false }
Task Deploy-Reports -precondition { return $OptionReports } -Depends `
Deploy-ReportProject, `
Replace-ReportsStoredProcs

Task Deploy-ReportProject {
	$projectDir = Join-Path $global:Context.Dir.Solution '..\ErmReports'

	$webAppInfo = Get-WebAppInfo
	Process-Rds $projectDir $webAppInfo.TransformedConfigFileName
	
	$projectPath = Join-Path $projectDir 'ErmReports.rptproj'
	if (!(Test-Path $projectPath)){
		throw "Cannot find file '$projectPath'"
	}
	
	& .\Reporting\Deploy-SSRSProject.ps1 -Path $projectPath -Configuration $global:Context.EnvironmentName
}

Task Replace-ReportsStoredProcs {
	$webAppInfo = Get-WebAppInfo
	$ermReports = Get-ConnectionString $webAppInfo.TransformedConfigFileName 'ErmReports'
	
	$erm = Get-ConnectionString $webAppInfo.TransformedConfigFileName 'Erm'
	$builder = New-Object System.Data.Common.DbConnectionStringBuilder
	$builder.set_ConnectionString($erm)

	# hack для русской локали
	$region = $global:Context.Region
	if ($region -eq 'RU'){
		$region = ''
	}
	
	Replace-StoredProcs $ermReports "\bErm$region[0-9]*\b" $builder['Initial Catalog']
	Replace-StoredProcs $ermReports '(http|https)://((?!(www.w3.org)|(schemas.microsoft.com)|(schemas.xmlsoap.org))).*?/' $webAppInfo.Uri.ToString()
}

function Process-Rds($ProjectDir, $ConfigFileName){
	$ermConString = Get-ConnectionString $ConfigFileName 'Erm'
	
	$ermRdsFileName = Join-Path $ProjectDir 'Erm.rds'
	Update-RdsConnectionString $ermRdsFileName $ermConString

	# заменяем datasource для ErmReports.rds
	$ermReportsConString = Get-ConnectionString $ConfigFileName 'ErmReports'
	$ermReportsRdsFileName = Join-Path $ProjectDir 'ErmReports.rds'
	Update-RdsConnectionString $ermReportsRdsFileName $ermReportsConString

	# заменяем datasource для MoDi.rds
	[xml]$config = Get-Content -Path $ConfigFileName -Encoding UTF8
	$modiConString = $config.SelectSingleNode('configuration/system.serviceModel/client/endpoint[contains(@address, "Reports.svc") or contains(@address, "MoneyDistributionService.svc")]')
	if ($modiConString -eq $null){
		throw "Could not find service connection for MoDi in file $ConfigFileName."
	}
	$modiRdsFileName = Join-Path $ProjectDir 'MoDi.rds'
	Update-RdsConnectionString $modiRdsFileName $modiConString.address
}

function Update-RdsConnectionString($RdsFileName, $ConnectionString){
	$rdsFile = Get-Item $RdsFileName
	$rdsFile.IsReadOnly = $false

	[xml]$xml = Get-Content -Path $RdsFileName -Encoding UTF8
	$xml.RptDataSource.ConnectionProperties.ConnectString = $ConnectionString

	$xml.Save($RdsFileName)
}