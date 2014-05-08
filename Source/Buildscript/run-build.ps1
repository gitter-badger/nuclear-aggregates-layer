# COMMENT FOR LOCAL DEBUG
param([string[]]$TaskList = @(), [hashtable]$Properties = @{})
# COMMENT FOR LOCAL DEBUG

# UNCOMMENT FOR LOCAL DEBUG
#$TaskList = @('Create-GlobalContext', 'Build-TaskService')
#$Properties = @{
#	'OptionTaskService' = $true
#	'OptionWebApp' = $true
#	'OptionWpfClient' = $true
#	'OptionModi' = $false
#	'OptionOrderValidation' = $false
#	'OptionDynamics' = $true
#	'OptionReports' = $false
#	
#	'Revision' = 1
#	'Build' = 2
#	'PackageVersion' = '1.7.9'
#	
#	'EnvironmentName' = 'Test.07'
#}
# UNCOMMENT FOR LOCAL DEBUG

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

function Restore-SolutionPackages {

	$solutionDir = Join-Path $ThisDir '..'

	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[progressMessage 'Restore-SolutionPackages']"
	}

	Import-Module '.\modules\nuget.psm1' -DisableNameChecking
	Invoke-NuGet @(
		'restore'
		$solutionDir
	)
}

function Run-Build($TaskList, $Properties) {

	Import-Module '..\packages\psake.4.3.2\tools\psake.psm1' -DisableNameChecking
	Invoke-psake `
	-nologo `
	-taskList $TaskList `
	-properties $Properties

	exit [int]!$psake.build_success
}

if (Test-Path 'Env:\TEAMCITY_VERSION'){
	$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(8192, 50)
}

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
Push-Location $ThisDir

Clear-Host
Restore-SolutionPackages
Run-Build $TaskList $Properties