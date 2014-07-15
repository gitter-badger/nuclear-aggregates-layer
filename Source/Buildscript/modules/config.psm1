Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

function Get-ConnectionString($ConnectionStringName) {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	
	Transform-WebConfig $projectFileName
	try{
		$projectDir = Split-Path $projectFileName
		$configFileName = Join-Path $projectDir 'web.config'
		[xml]$configFileContent = Get-Content $configFileName -Encoding UTF8

		$xmlNode = $configFileContent.SelectNodes("configuration/connectionStrings/add[@name = '$ConnectionStringName']")
		if ($xmlNode -eq $null){
			throw "Could not find connection string $ConnectionStringName in config file of project [$projectFileName]"
		}

		return $xmlNode.connectionString
	}
	finally{
		Restore-WebConfig $projectFileName
	}
}

function Get-ServiceUriString ($ServiceName) {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	
	Transform-WebConfig $projectFileName
	try{
		$projectDir = Split-Path $projectFileName
		$configFileName = Join-Path $projectDir 'web.config'
		[xml]$configFileContent = Get-Content $configFileName -Encoding UTF8

		$xmlNode = $configFileContent.SelectNodes("configuration/ermServicesSettings/ermServices/ermService[@name = '$ServiceName']")
		if ($xmlNode -eq $null){
			throw "Could not find service $ServiceName in config file of project [$projectFileName]"
		}

		return $xmlNode.baseUrl
	}
	finally{
		Restore-WebConfig $projectFileName
	}
}

Export-ModuleMember -Function Get-ConnectionString, Get-ServiceUriString