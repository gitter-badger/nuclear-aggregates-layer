# COMMENT FOR LOCAL DEBUG
param([string[]]$TaskList = @(), [hashtable]$Properties = @{})
# COMMENT FOR LOCAL DEBUG

# UNCOMMENT FOR LOCAL DEBUG
#$TaskList = @('Build-BasicOperations')
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

$Properties.Dir = @{
	'Solution' = Join-Path $PSScriptRoot '..'
	'Temp' = Join-Path $PSScriptRoot 'temp'
	'Artifacts' = Join-Path $PSScriptRoot 'artifacts'
}
$BuildFile = Join-Path $PSScriptRoot 'default.ps1'

function Restore-SolutionPackages ($solutionDir) {

	$NuGetPath = Join-Path $solutionDir '.nuget\NuGet.exe'
	$solution = Get-ChildItem $solutionDir -Filter '*.sln'
	
	& $NuGetPath @(
		'restore'
		$solution.FullName
		'-NonInteractive'
		'-Verbosity'
		'quiet'
	)
	
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

Restore-SolutionPackages $Properties.Dir.Solution

Import-Module "$PSScriptRoot\modules\buildtools.psm1" -DisableNameChecking

Run-Build $BuildFile $TaskList $Properties