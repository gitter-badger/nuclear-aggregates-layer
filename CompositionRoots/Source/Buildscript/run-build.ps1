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

#	'OptionTaskService' = $true
#	'OptionWpfClient' = $false
#	'OptionDynamics' = $true
#	'OptionReports' = $true
	
#	'Revision' = '1'
#	'Build' = 2
#	'Branch' = 'local'
	
#	'EnvironmentName' = 'Production.Russia'
#}
# UNCOMMENT FOR LOCAL DEBUG

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
$Properties.Dirs = @{
	'Solution' = Join-Path $ThisDir '..'
	'Temp' = Join-Path $ThisDir 'temp'
	'Artifacts' = Join-Path $ThisDir 'artifacts'
}
$BuildFile = Join-Path $ThisDir 'default.ps1'

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

Restore-SolutionPackages $Properties.Dirs.Solution

Push-Location $ThisDir
Import-Module .\modules\buildtools.psm1 -DisableNameChecking

Run-Build $BuildFile $TaskList $Properties