param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

# TeamCity UI buffer fix
if (${Env:TEAMCITY_VERSION}) {
	$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(8192, 50)
}

$PathToPSake = '..\packages\psake.4.2.0.1\tools\psake.ps1'
function Run-Build ($TaskList, $Properties) {

	& $PathToPSake `
	-nologo `
	-taskList $TaskList `
	-properties $Properties
}

# локальная отладка
#	$TaskList = @('Build-Packages')
#	$Properties = @{
#		'OptionTaskService' = $false
#		'OptionWebApp' = $false
#		'OptionModi' = $false
#		'OptionOrderValidation' = $false
#		'OptionDynamics' = $false
#		'OptionReports' = $false
#		
#		'Revision' = 0
#		'Build' = 0
#		
#		'TargetHostName' = 'uk-erm-test01'
#		'EnvironmentName' = 'Test.01'
#		
#		'Verbosity' = 'quiet'
#	}

$scriptDir = Split-Path $MyInvocation.MyCommand.Path -Parent
Push-Location $scriptDir
	
Run-Build $TaskList $Properties

if ($psake.build_success -eq $false){
	exit 1
}
else {
	exit 0 
}