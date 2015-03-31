Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\sqlserver.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\reports.psm1" -DisableNameChecking

Properties { $OptionReports = $true }

Task Deploy-Reports -Precondition { $OptionReports } -Depends `
Deploy-ReportsDir, `
Replace-ReportsStoredProcs

Task Deploy-ReportsDir {

	$reportsDir = Join-Path $global:Context.Dir.Solution '..\..\ErmReports'
	# если отчёт начинается с '_', то он не развёртывается
	$exclude = '_*.rdl'
	Process-Rds $reportsDir
	
	$reportServerSetting = Get-AppSetting 'ReportServer'
	$uriBuilder = New-Object System.UriBuilder($reportServerSetting)
	$reportsFolder = $uriBuilder.Query.Substring(1)
	$folderMappings = @{
		'Reports' = $reportsFolder
		'DataSources' = $reportsFolder + '/DataSources'
		'DataSets' = $reportsFolder + '/DataSets'
	}
	
	Deploy-ReportsDir $reportsDir $uriBuilder.Host $folderMappings $exclude
}

Task Replace-ReportsStoredProcs {

	$ermReports = Get-ConnectionString 'ErmReports'
	$ermReportsBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$ermReportsBuilder.set_ConnectionString($ermReports)
	Replace-StoredProcs $ermReports "\bErmReports[a-zA-Z]{0,2}[0-9]*\b" $ermReportsBuilder['Initial Catalog']
	
	$erm = Get-ConnectionString 'Erm'
	$ermBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$ermBuilder.set_ConnectionString($erm)
	Replace-StoredProcs $ermReports "\bErm[a-zA-Z]{0,2}[0-9]*\b" $ermBuilder['Initial Catalog']
	
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
	$modiRdsFileName = Join-Path $ReportsDir 'MoDi.rds'
	Update-RdsConnectionString $modiRdsFileName $modiUriString
}

function Update-RdsConnectionString($RdsFileName, $ConnectionString){
	$rdsFile = Get-Item $RdsFileName
	$rdsFile.IsReadOnly = $false

	[xml]$xml = Get-Content -Path $RdsFileName -Encoding UTF8
	$connectionProperties = $xml.RptDataSource.ConnectionProperties

	$oldConnectionString = $connectionProperties.ConnectString
	switch ($connectionProperties.Extension){
		'SQL' {
			$newConnectionString = Copy-ConnectionStringProperties $oldConnectionString $ConnectionString
		}
		'XML'{
			$newConnectionString = Copy-UrlProperties $oldConnectionString $ConnectionString
		}
		default { throw "Unsupported data source extension"}
	}
	$connectionProperties.ConnectString = $newConnectionString
	
	$xml.Save($RdsFileName)
}

function Copy-ConnectionStringProperties($oldConnectionString, $ConnectionString)
{
	$oldBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$oldBuilder.set_ConnectionString($oldConnectionString)
	
	$newBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$newBuilder.set_ConnectionString($ConnectionString)
	
	$keys = [array]$oldBuilder.Keys
	foreach($key in $keys){
		$newValue = $newBuilder[$key]
		if ($newValue -ne $null){
			$oldBuilder[$key] = $newValue
		}
	}
	
	return $oldBuilder.ConnectionString
}

function Copy-UrlProperties($oldUri, $newUri){
	$oldBuilder = New-Object System.UriBuilder($oldUri)
	$newBuilder = New-Object System.UriBuilder($newUri)
	
	$oldBuilder.Scheme = $newBuilder.Scheme
	$oldBuilder.Host = $newBuilder.Host
	$oldBuilder.Port = $newBuilder.Port
	return $oldBuilder.Uri.ToString()
}