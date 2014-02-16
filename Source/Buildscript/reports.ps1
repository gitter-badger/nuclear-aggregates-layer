Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\metadata.psm1 -DisableNameChecking
Import-Module .\modules\sqlserver.psm1 -DisableNameChecking
Import-Module .\modules\config.psm1 -DisableNameChecking
Import-Module .\modules\reports.psm1 -DisableNameChecking

Properties { $OptionReports=$false }
Task Deploy-Reports -precondition { return $OptionReports } -Depends `
Deploy-ReportsDir, `
Replace-ReportsStoredProcs

Task Deploy-ReportsDir {

	$reportsDir = Join-Path $global:Context.Dir.Solution '..\..\ErmReports'
	Process-Rds $reportsDir
	
	$entryPointMetadata = Get-EntryPointMetadata 'ErmReports'
	
	foreach($serverUrl in $entryPointMetadata.ServerUrls){
	
		$folderMappings = @{
			'Reports' = $entryPointMetadata.ReportsFolder
			'DataSources' = $entryPointMetadata.ReportsFolder + '/DataSources'
			'DataSets' = $entryPointMetadata.ReportsFolder + '/DataSets'
		}

		Deploy-ReportsDir $reportsDir $serverUrl $folderMappings
	}
}

Task Replace-ReportsStoredProcs {

	$entryPointMetadata = Get-EntryPointMetadata 'ErmReports'
	
	# hack для российской бизнес-модели
	$region = $entryPointMetadata.Region
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

function Process-Rds($ReportsDir){

	# заменяем datasource для Erm.rds
	$ermConString = Get-ConnectionString 'Erm'
	$ermRdsFileName = Join-Path $ReportsDir 'Erm.rds'
	Update-RdsConnectionString $ermRdsFileName $ermConString

	# заменяем datasource для ErmReports.rds
	$ermReportsConString = Get-ConnectionString 'ErmReports'
	$ermReportsRdsFileName = Join-Path $ReportsDir 'ErmReports.rds'
	Update-RdsConnectionString $ermReportsRdsFileName $ermReportsConString

	# заменяем datasource для MoDi.rds
	$modiUriString = Get-ServiceUriString 'MoDiService'
	$uriBuilder = New-Object System.UriBuilder($modiUriString)
	$uriBuilder.Path = 'Reports.svc'
	
	$modiRdsFileName = Join-Path $ReportsDir 'MoDi.rds'
	Update-RdsConnectionString $modiRdsFileName $uriBuilder.ToString()
}

function Update-RdsConnectionString($RdsFileName, $ConnectionString){
	$rdsFile = Get-Item $RdsFileName
	$rdsFile.IsReadOnly = $false

	[xml]$xml = Get-Content -Path $RdsFileName -Encoding UTF8
	$xml.RptDataSource.ConnectionProperties.ConnectString = $ConnectionString

	$xml.Save($RdsFileName)
}