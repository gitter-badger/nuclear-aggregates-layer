# COMMENT FOR LOCAL DEBUG
param([string[]]$TaskList = @(), [hashtable]$Properties = @{})
# COMMENT FOR LOCAL DEBUG

# UNCOMMENT FOR LOCAL DEBUG
#$TaskList = @('Hello')
#$Properties = @{
#	'OptionWebApp' = $true
#	'OptionBasicOperations' = $true
#	'OptionModi' = $true
#	'OptionMetadata' = $true
#	'OptionOrderValidation' = $true
#	'OptionFinancialOperations' = $true
#	'OptionReleasing' = $true
#
#	'OptionTaskService' = $true
#	'OptionWpfClient' = $false
#	'OptionDynamics' = $true
#	'OptionReports' = $true
#	
#	'Revision' = '1'
#	'Build' = 2
#	'Branch' = 'local'
#	
#	'EnvironmentName' = 'Production.Russia'
#}
# UNCOMMENT FOR LOCAL DEBUG

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$Properties.GlobalVersion = '2.62.0'

$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'
$Properties.Dir = @{
	'Solution' = Join-Path $PSScriptRoot '..'
	'Temp' = Join-Path $PSScriptRoot 'temp'
	'Artifacts' = Join-Path $PSScriptRoot 'artifacts'
}

Import-Module "$PSScriptRoot\metadata\metadata.psm1" -DisableNameChecking
$Properties.EnvironmentMetadata = $EnvironmentMetadata

# Restore-Packages
& {
	$NugetPath = Join-Path $Properties.Dir.Solution '.nuget\NuGet.exe'
	if (!(Test-Path $NugetPath)){
		$webClient = New-Object System.Net.WebClient
		$webClient.UseDefaultCredentials = $true
		$webClient.Proxy.Credentials = $webClient.Credentials
		$webClient.DownloadFile('https://www.nuget.org/nuget.exe', $NugetPath)
	}
	$solution = Get-ChildItem $Properties.Dir.Solution -Filter '*.sln'
	& $NugetPath @('restore', $solution.FullName, '-NonInteractive', '-Verbosity', 'quiet')
}

Import-Module "$($Properties.Dir.Solution)\packages\2GIS.NuClear.BuildTools.0.0.3\tools\buildtools.psm1" -DisableNameChecking
Run-Build $TaskList $Properties