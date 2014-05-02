param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$ThisDir = Split-Path $MyInvocation.MyCommand.Path -Parent
$SolutionDir = Join-Path $ThisDir '..'
$PSakePath = Join-Path $SolutionDir 'packages\psake.4.3.1.0\tools\psake.ps1'

$ModulesDir = Join-Path $ThisDir 'modules'
Import-Module "$ModulesDir\nuget.psm1" -DisableNameChecking

function Run-Build($TaskList, $Properties) {

	& $PSakePath `
	-nologo `
	-framework '4.5.1x64' `
	-taskList $TaskList `
	-properties $Properties
}

function Restore-SolutionPackages($SolutionDir) {

	Write-Host "##teamcity[progressMessage 'Restoring Solution Packages']"

	Invoke-NuGet @(
		'restore'
		$SolutionDir
	)
}

# UNCOMMENT FOR LOCAL DEBUG

#$TaskList = @('Create-GlobalContext', 'Hello')
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

# TeamCity UI buffer fix
if (${Env:TEAMCITY_VERSION}) {
	$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(8192, 50)
}

Clear-Host
Push-Location $ThisDir
Restore-SolutionPackages $SolutionDir
Run-Build $TaskList $Properties

# TeamCity exit code fix
if (${Env:TEAMCITY_VERSION}) {
	if ($psake.build_success -eq $false){
		exit 1
	}
	else {
		exit 0 
	}
}