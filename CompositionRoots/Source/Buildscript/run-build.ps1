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

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
Push-Location $ThisDir

Import-Module .\modules\nuget.psm1 -DisableNameChecking

function Create-GlobalContext ($Properties) {

	$solutionDir = Join-Path $ThisDir '..'
	if (!(Test-Path $solutionDir)){
		throw "Can't find solution dir $solutionDir"
	}
	
	$tempDir = Join-Path $ThisDir 'temp'
	if (Test-Path $tempDir){
		rd $tempDir -Recurse -Force | Out-Null
	}
	md $tempDir | Out-Null

	$artifactsDir = Join-Path $ThisDir 'artifacts'
	if (Test-Path $artifactsDir){
		rd $artifactsDir -Recurse -Force | Out-Null
	}
	md $artifactsDir | Out-Null

	$global:Context = @{
		'Dir' = @{
			'Solution' = $solutionDir
			'Temp' = $tempDir
			'Artifacts' = $artifactsDir
		}
	}
	
	if ($Properties.ContainsKey('Revision')){
		$global:Context.Add('Revision', $Properties['Revision'])
		$Properties.Remove('Revision')
	}
	if ($Properties.ContainsKey('Build')){
		$global:Context.Add('Build', $Properties['Build'])
		$Properties.Remove('Build')
	}
	if ($Properties.ContainsKey('Branch')){
		$global:Context.Add('Branch', $Properties['Branch'])
		$Properties.Remove('Branch')
	}
	if ($Properties.ContainsKey('EnvironmentName')){
		$global:Context.Add('EnvironmentName', $Properties['EnvironmentName'])
		$Properties.Remove('EnvironmentName')
	}
}

function Restore-SolutionPackages {

	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[progressMessage 'Restore-SolutionPackages']"
	}

	Invoke-NuGet @(
		'restore'
		$global:Context.Dir.Solution
	)
}

function Run-Build ($TaskList, $Properties) {

	$packageInfo = Get-PackageInfo 'psake'
	Import-Module "$($packageInfo.VersionedDir)\tools\psake.psm1" -DisableNameChecking -Force
	
	Invoke-psake 'default.ps1' `
	-nologo `
	-taskList $TaskList `
	-properties $Properties
	
	Exit [int]!$psake.build_success
}

if (Test-Path 'Env:\TEAMCITY_VERSION'){
	$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(8192, 50)
}

Create-GlobalContext $Properties
Restore-SolutionPackages
Run-Build $TaskList $Properties