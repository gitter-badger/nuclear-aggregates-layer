# COMMENT FOR LOCAL DEBUG
param([string[]]$TaskList = @(), [hashtable]$Properties = @{})
# COMMENT FOR LOCAL DEBUG

# UNCOMMENT FOR LOCAL DEBUG
#$TaskList = @('Create-GlobalContext', 'Build-Plugins')
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
#	
#	'EnvironmentName' = 'Production.Russia'
#}
# UNCOMMENT FOR LOCAL DEBUG

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
Push-Location $ThisDir

Import-Module .\modules\nuget.psm1 -DisableNameChecking

function Restore-SolutionPackages {

	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[progressMessage 'Restore-SolutionPackages']"
	}

	$solutionDir = Join-Path $ThisDir '..'
	Invoke-NuGet @(
		'restore'
		$solutionDir
	)
}

function Run-Build ($TaskList, $Properties) {

	$packageInfo = Get-PackageInfo 'psake'
	Import-Module "$($packageInfo.VersionedDir)\tools\psake.psm1" -DisableNameChecking -Force
	
	Invoke-psake 'default.ps1' `
	-nologo `
	-taskList $TaskList `
	-properties $Properties
	
	exit [int]!$psake.build_success
}

if (Test-Path 'Env:\TEAMCITY_VERSION'){
	$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(8192, 50)
}

Restore-SolutionPackages
Run-Build $TaskList $Properties