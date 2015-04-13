param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

if ($TaskList.Count -eq 0){
	$TaskList = @('Run-CoreTeamUnitTests')
}
if ($Properties.Count -eq 0){
	$Properties = @{
		'Revision' = '000000'
		'Build' = 0
		'Branch' = 'local'
		'EnvironmentName' = 'Production.Russia'
	}
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$Properties.GlobalVersion = '2.82.0'

$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'
$Properties.Dir = @{
	'Solution' = Join-Path $PSScriptRoot '..\CompositionRoots\Source'
	'Temp' = Join-Path $PSScriptRoot 'temp'
	'Artifacts' = Join-Path $PSScriptRoot 'artifacts'
}

Import-Module "$PSScriptRoot\metadata.psm1" -DisableNameChecking
$Properties.EnvironmentMetadata = $EnvironmentMetadata

# Restore-Packages
& {
	$NugetPath = Join-Path $Properties.Dir.Solution '.nuget\NuGet.exe'
	if (!(Test-Path $NugetPath)){
		$webClient = New-Object System.Net.WebClient
		$webClient.UseDefaultCredentials = $true
		$webClient.Proxy.Credentials = $webClient.Credentials
		$webClient.DownloadFile('https://www.nuget.org/nuget.exe', $NugetPath)
	}
	$solution = Get-ChildItem $Properties.Dir.Solution -Filter '*.sln'
	& $NugetPath @('restore', $solution.FullName, '-NonInteractive', '-Verbosity', 'quiet')
}

Import-Module "$($Properties.Dir.Solution)\packages\2GIS.NuClear.BuildTools.0.0.33\tools\buildtools.psm1" -DisableNameChecking
Run-Build $TaskList $Properties
