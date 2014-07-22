# COMMENT FOR LOCAL DEBUG
#param([string[]]$TaskList = @(), [hashtable]$Properties = @{})
# COMMENT FOR LOCAL DEBUG

# UNCOMMENT FOR LOCAL DEBUG
$TaskList = @('Create-GlobalContext', 'Build-TaskService')
$Properties = @{
	'OptionTaskService' = $true
	'OptionWebApp' = $true
	'OptionWpfClient' = $true
	'OptionModi' = $false
	'OptionOrderValidation' = $false
	'OptionDynamics' = $true
	'OptionReports' = $false
	
	'Revision' = 1
	'Build' = 2
	'PackageVersion' = '1.7.9'
	
	'EnvironmentName' = 'Edu.Russia'
}
# UNCOMMENT FOR LOCAL DEBUG

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

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
	Import-Module "$($packageInfo.VersionedDir)\tools\psake.psm1" -DisableNameChecking

	Invoke-psake `
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