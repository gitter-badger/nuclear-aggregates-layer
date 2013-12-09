param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------
Clear-Host

# TeamCity UI buffer fix
if (${Env:TEAMCITY_VERSION}) {
	$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(8192, 50)
}

$PathToPSake = '..\packages\psake.4.3.0.0\tools\psake.ps1'
function Run-Build ($TaskList, $Properties) {

	# psake bug forces to set 'framework',
	# actually 'framework' not used anywhere in buildscript

	& $PathToPSake `
	-nologo `
	-framework '4.5.1x64' `
	-taskList $TaskList `
	-properties $Properties
}

# локальная отладка
#$TaskList = @('Create-GlobalContext', 'Build-UnitTests')
#$Properties = @{
#	'OptionTaskService' = $false
#	'OptionWebApp' = $false
#	'OptionWpf' = $true
#	'OptionModi' = $false
#	'OptionOrderValidation' = $false
#	'OptionDynamics' = $false
#	'OptionReports' = $false
	
#	'Revision' = 0
#	'Build' = 0
	
#	'TargetHostName' = 'uk-erm-test01'
#	'EnvironmentName' = 'Test.01'
	
#	'Verbosity' = 'quiet'
#	'Region' = 'RU'
#}

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