Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$ThisDir = Split-Path $MyInvocation.MyCommand.Path -Parent
$GodConfigFileName = Join-Path $ThisDir '..\..\Environments.config'
[xml]$GodConfig = Get-Content $GodConfigFileName -Encoding UTF8

function Get-ConnectionString($ConnectionStringName) {
	$environmentName = $global:Context.EnvironmentName

	$xmlNode = $GodConfig.SelectNodes("configuration/ermEnvironmentsSettings/ermEnvironments/ermEnvironment[@name = '$environmentName']/connectionStrings/add[@name = '$ConnectionStringName']")
	if ($xmlNode -eq $null){
		throw "Could not find connection string $ConnectionStringName for environment $environmentName in file $GodConfigFileName"
	}

	return $xmlNode.connectionString
}

function Get-ServiceUriString ($ServiceName) {
	$environmentName = $global:Context.EnvironmentName
	
	$xmlNode = $GodConfig.SelectNodes("configuration/ermEnvironmentsSettings/ermEnvironments/ermEnvironment[@name = '$environmentName']/ermServices/ermService[@name = '$ServiceName']")
	if ($xmlNode -eq $null){
		throw "Could not find service $ServiceName for environment $environmentName in file $GodConfigFileName"
	}

	return $xmlNode.baseUrl
}

Export-ModuleMember -Function Get-ConnectionString, Get-ServiceUriString