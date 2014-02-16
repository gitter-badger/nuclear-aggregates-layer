Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$Namespace = 'DoubleGis.Erm.Reporting'

$DefaultFolderMappings = @{
	'Reports' = '/'
	'DataSources' = '/DataSources'
	'DataSets' = '/DataSets'
}

function Deploy-ReportsDir($ReportsDir, $ServerUrl, $FolderMappings = $DefaultFolderMappings){
	$service = Get-ReportService2010 $ServerUrl

	Create-FolderMappings $service $FolderMappings
	
	Deploy-DataSources $ReportsDir $service $FolderMappings
	Deploy-SharedDataSets $ReportsDir $service $FolderMappings
	Deploy-Reports $ReportsDir $service $FolderMappings
}

function Deploy-Reports($ReportsDir, $Service, $FolderMappings){
	$rdlFiles = Get-ChildItem $ReportsDir -Filter '*.rdl'
	foreach($rdlFile in $rdlFiles){
		[xml]$xml = Get-Content $rdlFile.FullName -Encoding UTF8
		
		# datasource reference
		foreach ($dataSource in $xml.Report.DataSources.ChildNodes){
			$dataSource.DataSourceReference = $FolderMappings.DataSources + '/' + $dataSource.DataSourceReference
		}
		
		# shared dataset reference
		foreach ($dataSet in ($xml.Report.DataSets.ChildNodes | where { $_.FirstChild.Name -eq 'SharedDataSet' } )){
			$dataSet.SharedDataSet.SharedDataSetReference = $FolderMappings.DataSets + '/' + $dataSet.SharedDataSet.SharedDataSetReference
		}
		
		$fileNameWithoutExtension = [System.IO.Path]::GetFileNameWithoutExtension($rdlFile.FullName)
		$warnings = @()
		$bytes = [System.Text.Encoding]::UTF8.GetBytes($xml.OuterXml)
		$withOverwrite = $true
		$Service.CreateCatalogItem('Report', $fileNameWithoutExtension, $FolderMappings.Reports, $withOverwrite, $bytes, $null, [ref]$warnings) | Out-Null
	}
}

function Deploy-SharedDataSets ($ReportsDir, $Service, $FolderMappings){
	$rsdFiles = Get-ChildItem $ReportsDir -Filter '*.rsd'
	foreach($rsdFile in $rsdFiles){
		[xml]$xml = Get-Content $rsdFile.FullName -Encoding UTF8
		
		# datasource reference
		$dataSourceName = $xml.SharedDataSet.DataSet.Query.DataSourceReference
		$xml.SharedDataSet.DataSet.Query.DataSourceReference = $FolderMappings.DataSources + '/' + $dataSourceName
		
		$fileNameWithoutExtension = [System.IO.Path]::GetFileNameWithoutExtension($rsdFile.FullName)
		$warnings = @()
		$bytes = [System.Text.Encoding]::UTF8.GetBytes($xml.OuterXml)
		$withOverwrite = $true
		$Service.CreateCatalogItem('DataSet', $fileNameWithoutExtension, $FolderMappings.DataSets, $withOverwrite, $bytes, $null, [ref]$warnings) | Out-Null
	}
}

function Deploy-DataSources ($ReportsDir, $Service, $FolderMappings){
	$rdsFiles = Get-ChildItem $ReportsDir -Filter '*.rds'
	foreach($rdsFile in $rdsFiles){
	
		$dataSourceDefinition = New-Object PowerShell.Reporting2010.DataSourceDefinition
	
		[xml]$xml = Get-Content $rdsFile.FullName -Encoding UTF8
		$dataSourceDefinition.Extension = $xml.RptDataSource.ConnectionProperties.Extension
		$dataSourceDefinition.ConnectString = $xml.RptDataSource.ConnectionProperties.ConnectString
		$dataSourceDefinition.CredentialRetrieval = 'Integrated'
		
		$dataSourceName = $xml.RptDataSource.Name
		$withOverwrite = $true
		$Service.CreateDataSource($dataSourceName, $FolderMappings.DataSources, $withOverwrite, $dataSourceDefinition, $null) | Out-Null
	}
}

function Create-FolderMappings ($Service, $FolderMappings){
	foreach($Folder in $FolderMappings.Values){
		Create-FolderRecursive $Service $Folder
	}
}

function Create-FolderRecursive($Service, $Folder){

	if ($Service.GetItemType($Folder) -eq 'Folder'){
		return
	}

	$slashIndex = $Folder.LastIndexOf('/')
	if ($slashIndex -eq 0){
		$parent = '/'
		$leaf = $Folder.TrimStart('/')
	}
	else {
		$parent = $Folder.Substring(0, $slashIndex)
		$leaf = $Folder.Substring($slashIndex, $Folder.Length - $slashIndex).TrimStart('/')
	}

	if ($Service.GetItemType($parent) -eq 'Unknown'){
		Create-FolderRecursive $Service $parent
	}
	
	$Service.CreateFolder($leaf, $parent, $null) | Out-Null
}

function Get-ReportService2010 ($ServerUrl){
	$uriBuilder = New-Object System.UriBuilder($ServerUrl)
	$uriBuilder.Path += '/ReportService2010.asmx'

	$proxyAssembly = [System.AppDomain]::CurrentDomain.GetAssemblies() | where { $_.GetType($Namespace + 'ReportingService2010') -ne $null }
	if ($proxyAssembly -eq $null){
		$webServiceProxy = New-WebServiceProxy $uriBuilder.Uri -Namespace $Namespace -UseDefaultCredential
	}else {
		$webServiceProxy = New-Object ($Namespace + 'ReportingService2010')
		$webServiceProxy.UseDefaultCredentials = $true
	}
	
	return $webServiceProxy
}

Export-ModuleMember -Function Deploy-ReportsDir