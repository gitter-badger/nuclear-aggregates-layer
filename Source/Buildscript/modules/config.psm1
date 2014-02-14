Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

function Get-ConnectionString($ConnectionStringName) {
	$sourceProject = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	[xml]$config = Evaluate-Config $SourceProject 'web'

	$xmlNode = $config.SelectNodes("configuration/connectionStrings/add[@name = '$ConnectionStringName']")
	if ($xmlNode -eq $null){
		throw "Could not find connection string $ConnectionStringName in config file of project [$SourceProject]"
	}

	return $xmlNode.connectionString
}

function Get-ServiceUriString ($ServiceName) {
	$sourceProject = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	[xml]$config = Evaluate-Config $SourceProject 'web'
	
	$xmlNode = $config.SelectNodes("configuration/ermServicesSettings/ermServices/ermService[@name = '$ServiceName']")
	if ($xmlNode -eq $null){
		throw "Could not find service $ServiceName in config file of project [$SourceProject]"
	}

	return $xmlNode.baseUrl
}

Export-ModuleMember -Function Get-ConnectionString, Get-ServiceUriString