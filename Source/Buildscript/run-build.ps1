param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------
Clear-Host

# TeamCity UI buffer fix
if (${Env:TEAMCITY_VERSION}) {
	$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(8192, 50)
}

$PathToPSake = '..\packages\psake.4.3.1.0\tools\psake.ps1'
function Run-Build ($TaskList, $Properties) {

	& $PathToPSake `
	-nologo `
	-framework '4.5.1x64' `
	-taskList $TaskList `
	-properties $Properties
}

# UNCOMMENT FOR LOCAL DEBUG

#$TaskList = @('Create-GlobalContext', 'Build-WpfClient')
#$Properties = @{
#	'OptionTaskService' = $true
#	'OptionWebApp' = $true
#	'OptionWpfClient' = $true
#	'OptionModi' = $false
#	'OptionOrderValidation' = $false
#	'OptionDynamics' = $true
#	'OptionReports' = $false
	
#	'Revision' = 1
#	'Build' = 2
#	'PackageVersion' = '1.7.9'
	
#	'EnvironmentName' = 'Test.07'
	
#	'Verbosity' = 'quiet'
#	'Region' = 'RU'
#}

# UNCOMMENT FOR LOCAL DEBUG

$scriptDir = Split-Path $MyInvocation.MyCommand.Path -Parent
Push-Location $scriptDir
	
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